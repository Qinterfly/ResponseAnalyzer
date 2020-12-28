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
            workSheet_ = package_.Workbook.Worksheets.Add(workSheetName_);
            posCharts_ = new Dictionary<ExcelDrawing, ChartPosition>();
            path_ = resPath;
            // Clear all the series
            foreach (ExcelDrawing objChart in charts_)
            {
                ExcelScatterChart chart = (ExcelScatterChart)objChart;
                while (chart.Series.Count > 0)
                    chart.Series.Delete(0);
            }
            // Save
            package_.Save();
        }

        public ExcelObject(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                package_ = new ExcelPackage(fileInfo);
                charts_ = new List<ExcelDrawing>();
                indMarkers_ = new Dictionary<ExcelDrawing, int>();
                path_ = path;
                retrieveCharts();
                // Specifying the sequence of the styles
                markersProperties_ = new List<MarkerProperties>();
                markersProperties_.Add(new MarkerProperties { fillColor = Color.White, style = eMarkerStyle.Square });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.Black, style = eMarkerStyle.Square });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.White, style = eMarkerStyle.Circle });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.Black, style = eMarkerStyle.Circle });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.White, style = eMarkerStyle.Triangle });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.Black, style = eMarkerStyle.Triangle });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.White, style = eMarkerStyle.Diamond });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.Black, style = eMarkerStyle.Diamond });
                markersProperties_.Add(new MarkerProperties { fillColor = Color.Black, style = eMarkerStyle.X });
            }
        }

        public void addSeries(string chartName, double[,] data, string dataName)
        {
            // Finding the chart
            ExcelDrawing objChart = null;
            foreach (ExcelDrawing chart in charts_)
            {
                if (chart.Name == chartName)
                {
                    objChart = chart;
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
            string xVals = ExcelRange.GetAddress(iRow, jCol,
                                                 iRow + nData - 1, jCol);
            string yVals = ExcelRange.GetAddress(iRow, jCol + 1,
                                                 iRow + nData - 1, jCol + 1);
            xVals = ExcelRange.GetFullAddress(workSheetName_, xVals);
            yVals = ExcelRange.GetFullAddress(workSheetName_, yVals);
            // Creating the serie
            ExcelScatterChartSerie serie = scatterChart.Series.Add(yVals, xVals);
            // Specifying the style
            MarkerProperties properties = markersProperties_[indMarkers_[objChart]];
            serie.Border.Fill.Color = Color.Black;          // Line color
            serie.Border.Width = 1;                         // Line width
            serie.Marker.Border.Fill.Color = Color.Black;   // Marker border color
            serie.Marker.Border.Width = 0.75;               // Marker border width
            serie.Marker.Size = 5;                          // Marker size
            serie.Marker.Fill.Color = properties.fillColor; // Fill color
            serie.Marker.Style = properties.style;          // Style
            ++indMarkers_[objChart];
            if (indMarkers_[objChart] > markersProperties_.Count)
                indMarkers_[objChart] = 0;
            // Legend
            serie.Header = dataName;
            // Shifting data locations
            pos.availablePosition.col = pos.availablePosition.col + 2;
            pos.length = Math.Max(pos.length, nData);
            package_.Save();
        }

        public void open()
        {
            if (excelApplication_ == null)
                return;
            excelApplication_.Workbooks.Open(path_);
            excelApplication_.Visible = true;
        }

        private void Copy(ExcelObject another)
        {
            package_ = another.package_;
            charts_ = another.charts_;
            path_ = another.path_;
            indMarkers_ = another.indMarkers_;
            markersProperties_ = another.markersProperties_;
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
                        indMarkers_.Add(drawing, 0);
                    }
                }
            }
        }

        Application excelApplication_;
        ExcelPackage package_;
        string path_ { get; set;  }
        // Charts
        List<ExcelDrawing> charts_;
        Dictionary<ExcelDrawing, ChartPosition> posCharts_;
        // Work sheet
        ExcelWorksheet workSheet_ = null;
        const string workSheetName_ = "ChartData";
        // Style
        Dictionary<ExcelDrawing, int> indMarkers_;
        List<MarkerProperties> markersProperties_;
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

    public class MarkerProperties
    {
        public Color fillColor;
        public eMarkerStyle style;
    }
}
