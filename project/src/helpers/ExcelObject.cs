using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using Microsoft.Office.Interop.Excel;

namespace ResponseAnalyzer
{
    class ExcelObject
    {
        public ExcelObject(ExcelObject template, string path, string name)
        {
            string originalPath = template.path_;
            string resPath = path + "\\" + name + ".xlsx";
            // Looking for an available name if a template equals a resulting file
            if (Path.GetFileNameWithoutExtension(originalPath).Equals(name))
            {
                bool isAvailable = false;
                while (!isAvailable)
                {
                    name += "-copy";
                    resPath = path + "\\" + name + ".xlsx";
                    isAvailable = !(new FileInfo(name).Exists);
                }
            }
            // Retreiving a copy of an already running application
            try 
            { 
                excelApplication_ = (Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                excelApplication_.Visible = true;
            }
            catch
            {
                excelApplication_ = new Application();
                excelApplication_.Visible = true;
            }
            // Copying the template
            try
            { 
                File.Copy(originalPath, resPath, true);
            }
            catch
            {
                excelApplication_.Workbooks.Close();
                File.Copy(originalPath, resPath, true);
            }
            Copy(new ExcelObject(resPath));
            // Delete the data worksheet if existed
            ExcelWorkbook book = package_.Workbook;
            foreach (ExcelWorksheet sheet in book.Worksheets)
            {
                if (sheet.Name.Equals(workSheetName_))
                {
                    book.Worksheets.Delete(workSheetName_);
                    break;
                }
            }
            // Add a new one
            workSheet_ = book.Worksheets.Add(workSheetName_);
            posCharts_ = new Dictionary<ExcelDrawing, ChartPosition>();
            path_ = resPath;
            // Clear all the series
            foreach (ExcelDrawing objChart in charts_)
            {
                ExcelScatterChart chart = (ExcelScatterChart)objChart;
                while (chart.Series.Count > 0)
                    chart.Series.Delete(0);
            }
        }

        public ExcelObject(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                package_ = new ExcelPackage(fileInfo);
                charts_ = new List<ExcelDrawing>();
                chartSheets_ = new List<string>();
                indMarkers_ = new Dictionary<string, int>();
                customMarkers_ = new Dictionary<string, List<MarkerProperty>>();
                path_ = path;
                retrieveCharts();
                createMarkers(); // Specifying the styles sequence
            }
        }

        public void createMarkers()
        {
            // Standard markers
            standardMarkers_ = new List<MarkerProperty>();
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.White, style = eMarkerStyle.Square   });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.Black, style = eMarkerStyle.Square   });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.White, style = eMarkerStyle.Circle   });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.Black, style = eMarkerStyle.Circle   });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.White, style = eMarkerStyle.Triangle });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.Black, style = eMarkerStyle.Triangle });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.White, style = eMarkerStyle.Diamond  });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.Black, style = eMarkerStyle.Diamond  });
            standardMarkers_.Add(new MarkerProperty { fillColor = Color.Black, style = eMarkerStyle.X        });
            // Check if the sequence is defined in the template file
            List<string> chartNames = getChartNames();
            foreach (string chart in chartNames)
                customMarkers_.Add(chart, null);
            ExcelWorksheet markersSheet = null;
            foreach (ExcelWorksheet sheet in package_.Workbook.Worksheets){
                if (sheet.Name == markersSheetName_)
                    markersSheet = sheet;
            }
            if (markersSheet == null)
                return;
            // Reading the defined sequence
            int nColumns = markersSheet.Dimension.Columns;
            int nRows = markersSheet.Dimension.Rows;
            for (int iColumn = 1; iColumn <= nColumns; ++iColumn)
            {
                string chart = markersSheet.Cells[1, iColumn].Text;
                if (String.IsNullOrEmpty(chart))
                    break;
                if (!customMarkers_.ContainsKey(chart))
                    continue;
                List<MarkerProperty> properties = new List<MarkerProperty>();
                for (int iRow = 2; iRow <= nRows; ++iRow)
                {
                    string description = markersSheet.Cells[iRow, iColumn].Text.ToLower();
                    MarkerProperty marker = new MarkerProperty();
                    marker.style = eMarkerStyle.None;
                    marker.fillColor = Color.White;
                    switch (description)
                    {
                        case "□":
                            marker.style = eMarkerStyle.Square;
                            break;
                        case "■":
                            marker.style = eMarkerStyle.Square;
                            marker.fillColor = Color.Black;
                            break;
                        case "○":
                            marker.style = eMarkerStyle.Circle;
                            break;
                        case "●":
                            marker.style = eMarkerStyle.Circle;
                            marker.fillColor = Color.Black;
                            break;
                        case "△":
                            marker.style = eMarkerStyle.Triangle;
                            break;
                        case "▲":
                            marker.style = eMarkerStyle.Triangle;
                            marker.fillColor = Color.Black;
                            break;
                        case "◇":
                            marker.style = eMarkerStyle.Diamond;
                            break;
                        case "◆":
                            marker.style = eMarkerStyle.Diamond;
                            marker.fillColor = Color.Black;
                            break;
                        case "x":
                            marker.style = eMarkerStyle.X;
                            marker.fillColor = Color.Black;
                            break;
                    }
                    // If a marker fits the set of the predefined words
                    if (marker.style != eMarkerStyle.None)
                        properties.Add(marker);
                    else
                        break;
                }
                customMarkers_[chart] = properties;
            }
        }

        public void addSeries(string chartName, double[,] data, string dataName)
        {
            // Finding the chart
            ExcelDrawing objChart = null;
            int nCharts = charts_.Count;
            for (int i = 0; i != nCharts; ++i)
            {
                ExcelDrawing chart = charts_[i];
                if (chart.Name == chartName)
                {
                    objChart = chart;
                    lastSheet_ = chartSheets_[i];
                    break;
                }
            }
            if (objChart == null)
                return;
            // Check if the chart is currently in use
            ChartPosition pos;
            int iRow, jCol;
            if (!posCharts_.ContainsKey(objChart))
            {
                pos = new ChartPosition()
                {
                    header = new Position { row = ChartPosition.lastRow + 1, col = 1 },
                    length = data.GetLength(0),
                    availablePosition = new Position { row = ChartPosition.lastRow + 3, col = 1}
                };
                // Write the header
                workSheet_.Cells[pos.header.row, pos.header.col].Value = objChart.Name;
                posCharts_.Add(objChart, pos);
                ChartPosition.lastRow += pos.length + 3;
            }
            else
            {
                pos = posCharts_[objChart];
            }
            // Add the function values
            iRow = pos.availablePosition.row;
            jCol = pos.availablePosition.col;
            int nData = data.GetLength(0);
            for (int k = 0; k != nData; ++k)
            {
                workSheet_.Cells[iRow + k, jCol    ].Value = data[k, 0];
                workSheet_.Cells[iRow + k, jCol + 1].Value = data[k, 1];
            }
            workSheet_.Cells[pos.header.row + 1, jCol + 1].Value = dataName; // Set the name
            // Retrieving the data address
            ExcelScatterChart scatterChart = (ExcelScatterChart) objChart;
            string xVals = ExcelRange.GetAddress(iRow,             jCol,
                                                 iRow + nData - 1, jCol);
            string yVals = ExcelRange.GetAddress(iRow,             jCol + 1,
                                                 iRow + nData - 1, jCol + 1);
            xVals = ExcelRange.GetFullAddress(workSheetName_, xVals);
            yVals = ExcelRange.GetFullAddress(workSheetName_, yVals);
            // Creating the serie
            ExcelScatterChartSerie serie = scatterChart.Series.Add(yVals, xVals);
            // Using the standard markers when custom ones are not available
            List<MarkerProperty> markers = customMarkers_[chartName]; 
            if (markers == null || markers.Count == 0)
                markers = standardMarkers_;
            MarkerProperty properties = markers[indMarkers_[chartName]];
            serie.Border.Fill.Color = Color.Black;          // Line color
            serie.Border.Width = 1;                         // Line width
            serie.Marker.Border.Fill.Color = Color.Black;   // Marker border color
            serie.Marker.Border.Width = 0.75;               // Marker border width
            serie.Marker.Size = 5;                          // Marker size
            serie.Marker.Fill.Color = properties.fillColor; // Fill color
            serie.Marker.Style = properties.style;          // Style
            ++indMarkers_[chartName];
            if (indMarkers_[chartName] >= markers.Count)
                indMarkers_[chartName] = 0;
            // Legend
            serie.Header = dataName;
            // Shifting data locations
            pos.availablePosition.col = pos.availablePosition.col + 2;
            pos.length = Math.Max(pos.length, nData);
            int lastRowColumn = pos.availablePosition.row + pos.length;
            if (lastRowColumn > ChartPosition.lastRow)
                ChartPosition.lastRow = lastRowColumn;
        }

        // Save changes
        public void save()
        {
            package_.Save();
        }

        public void open()
        {
            if (excelApplication_ == null)
                return;
            Workbook book = excelApplication_.Workbooks.Open(path_);
            excelApplication_.Visible = true;
            try
            {
                if (!string.IsNullOrEmpty(lastSheet_))
                    book.Worksheets[lastSheet_].Activate();
            }
            catch
            {

            }
        }

        private void Copy(ExcelObject another)
        {
            package_ = another.package_;
            charts_ = another.charts_;
            chartSheets_ = another.chartSheets_;
            path_ = another.path_;
            indMarkers_ = another.indMarkers_;
            standardMarkers_ = another.standardMarkers_;
            customMarkers_ = another.customMarkers_;
        }

        public bool isOpened()
        {
            return package_.File != null;
        }

        public List<string> getChartNames()
        {
            List<string> chartNames = new List<string>();
            foreach (ExcelDrawing chart in charts_)
                chartNames.Add(chart.Name);
            return chartNames;
        }

        private void retrieveCharts()
        {
            ExcelWorksheets worksheets = package_.Workbook.Worksheets;
            foreach (ExcelWorksheet worksheet in worksheets)
            {
                foreach (ExcelDrawing drawing in worksheet.Drawings)
                {
                    if (drawing.DrawingType == eDrawingType.Chart)
                    {
                        charts_.Add(drawing);
                        chartSheets_.Add(worksheet.Name);
                        indMarkers_.Add(drawing.Name, 0);
                    }
                }
            }
        }

        Application excelApplication_;
        ExcelPackage package_;
        string path_ { get; set;  }
        string lastSheet_; 
        // Charts
        List<ExcelDrawing> charts_;
        List<string> chartSheets_;
        Dictionary<ExcelDrawing, ChartPosition> posCharts_;
        // Worksheet
        ExcelWorksheet workSheet_ = null;
        const string workSheetName_ = "ChartData";
        // Style
        Dictionary<string, int> indMarkers_;
        List<MarkerProperty> standardMarkers_;
        Dictionary<string, List<MarkerProperty>> customMarkers_;
        const string markersSheetName_ = "Markers";
    }

    public struct ChartPosition
    {
        public Position header;
        public int length;
        public Position availablePosition;
        static public int lastRow = 0;
    }

    public class Position
    {
        public int row;
        public int col;
    }

    public class MarkerProperty
    {
        public Color fillColor;
        public eMarkerStyle style;
    }
}
