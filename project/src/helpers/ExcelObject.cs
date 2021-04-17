using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace ResponseAnalyzer
{
    class ExcelObject
    {
        public ExcelObject(ExcelObject template, string path, string name)
        {
            const string extension = ".xlsx";
            string originalPath = template.path_;
            string resPath = path + "\\" + name + extension;
            // Looking for an available name if a template equals a resulting file
            if (Path.GetFileNameWithoutExtension(originalPath).Equals(name))
            {
                bool isAvailable = false;
                while (!isAvailable)
                {
                    name += "-copy";
                    resPath = path + "\\" + name + extension;
                    isAvailable = !(new FileInfo(name).Exists);
                }
            }
            // Try creating an Excel instance
            createExcelInstance(originalPath, resPath, name, extension);
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
				indLines_ = new Dictionary<string, int>();
				customLines_ = new Dictionary<string, List<LineProperty>>();
                path_ = path;
                retrieveCharts();
                createMarkers(); // Specifying the styles sequence of markers
				createLines();   // Specifying the styles of lines
            }
        }
		
		public void createLines()
		{
			// Standard lines
            standardLines_ = new List<LineProperty>();
            standardLines_.Add(new LineProperty { lineStyle = eLineStyle.Solid, isMarkersEnabled = true, isTransparent = false });
            List<string> chartNames = getChartNames();
            foreach (string chart in chartNames)
                customLines_.Add(chart, null);
			ExcelWorksheet linesSheet = null;
            foreach (ExcelWorksheet sheet in package_.Workbook.Worksheets)
				if (sheet.Name == linesSheetName_)
                    linesSheet = sheet;
			if (linesSheet == null)
				return;
			int nColumns = linesSheet.Dimension.Columns;
			int nRows = linesSheet.Dimension.Rows;
			for (int iColumn = 1; iColumn <= nColumns; ++iColumn)
			{
				string chart = linesSheet.Cells[1, iColumn].Text;
				if (String.IsNullOrEmpty(chart))
					break;
				if (!customLines_.ContainsKey(chart))
					continue;
				List<LineProperty> lineProperties = new List<LineProperty>();
				for (int iRow = 2; iRow <= nRows; ++iRow)
				{
					string description = linesSheet.Cells[iRow, iColumn].Text.ToLower();
					LineProperty line = new LineProperty();
                    bool isCorrect = true;
					switch (description)
					{
						case "━":
							line.lineStyle = eLineStyle.Solid;
							line.isMarkersEnabled = false;
                            line.isTransparent = false;
                            break;
                        case "--":
                            line.lineStyle = eLineStyle.Dash;
                            line.isMarkersEnabled = false;
                            line.isTransparent = false;
                            break;
                        case "x":
                            line.lineStyle = eLineStyle.Solid;
                            line.isMarkersEnabled = true;
                            line.isTransparent = true;
                            break;
						case "x━":
                            line.lineStyle = eLineStyle.Solid;
                            line.isMarkersEnabled = true;
                            line.isTransparent = false;
                            break;
                        case "x--":
                            line.lineStyle = eLineStyle.Dash;
                            line.isMarkersEnabled = true;
                            line.isTransparent = false;
                            break;
                        default:
                            isCorrect = false;
                            break;
                    }
					// If a marker fits the set of the predefined words
					if (isCorrect)
						lineProperties.Add(line);
					else
						break;
				}
				customLines_[chart] = lineProperties;
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
            foreach (ExcelWorksheet sheet in package_.Workbook.Worksheets)
			{
                if (sheet.Name == markersSheetName_)
                    markersSheet = sheet;
            }
			// Reading the defined sequence of markers
            if (markersSheet == null)
				return;
			int nColumns = markersSheet.Dimension.Columns;
			int nRows = markersSheet.Dimension.Rows;
			for (int iColumn = 1; iColumn <= nColumns; ++iColumn)
			{
				string chart = markersSheet.Cells[1, iColumn].Text;
				if (String.IsNullOrEmpty(chart))
					break;
				if (!customMarkers_.ContainsKey(chart))
					continue;
				List<MarkerProperty> markerProperties = new List<MarkerProperty>();
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
						markerProperties.Add(marker);
					else
						break;
				}
				customMarkers_[chart] = markerProperties;
			}
        }

        public void addSeries(string chartName, double[,] data, string dataName, string infoChart = "")
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
                    availablePosition = new Position { row = ChartPosition.lastRow + 3, col = 1 }
                };
                // Write the header
                workSheet_.Cells[pos.header.row, pos.header.col].Value = objChart.Name + infoChart;
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
                workSheet_.Cells[iRow + k, jCol].Value = data[k, 0];
                workSheet_.Cells[iRow + k, jCol + 1].Value = data[k, 1];
            }
            workSheet_.Cells[pos.header.row + 1, jCol + 1].Value = dataName; // Set the name
            // Retrieving the data address
            ExcelScatterChart scatterChart = (ExcelScatterChart)objChart;
            string xVals = ExcelRange.GetAddress(iRow, jCol,
                                                 iRow + nData - 1, jCol);
            string yVals = ExcelRange.GetAddress(iRow, jCol + 1,
                                                 iRow + nData - 1, jCol + 1);
            xVals = ExcelRange.GetFullAddress(workSheetName_, xVals);
            yVals = ExcelRange.GetFullAddress(workSheetName_, yVals);
            // Creating the serie
            ExcelScatterChartSerie serie = scatterChart.Series.Add(yVals, xVals);
            // Using the standard markers when custom ones are not available
            List<MarkerProperty> markers = customMarkers_[chartName];
            if (markers == null || markers.Count == 0)
                markers = standardMarkers_;
            MarkerProperty markerProperties = markers[indMarkers_[chartName]];
            // Using the standard lines when custom ones are not available
            List<LineProperty> lines = customLines_[chartName];
            if (lines == null || lines.Count == 0)
                lines = standardLines_;
            LineProperty lineProperties = lines[indLines_[chartName]];
            int transparency = lineProperties.isTransparent ? 100 : 0; // Perecentage
            // Specifying the properties
            serie.Border.Fill.Color = Color.Black;              // Line color
            serie.Border.LineStyle = lineProperties.lineStyle;  // Line style
            serie.Border.Fill.Transparancy = transparency;      // Line transparency
            serie.Border.Width = 1.0;                           // Line width
            if (serie.Marker != null)
            {
                serie.Marker.Border.Fill.Color = Color.Black;          // Marker border color
                serie.Marker.Border.Width = 0.75;                      // Marker border width
                serie.Marker.Size = 5;                                 // Marker size
                serie.Marker.Fill.Color = markerProperties.fillColor;  // Marker fill color
                // Marker style
                if (lineProperties.isMarkersEnabled)
                    serie.Marker.Style = markerProperties.style;
                else
                    serie.Marker.Style = eMarkerStyle.None;
                // Increment markers and lines indices
                ++indMarkers_[chartName];
                if (indMarkers_[chartName] >= markers.Count)
                    indMarkers_[chartName] = 0;
            }
            ++indLines_[chartName];
            if (indLines_[chartName] >= lines.Count)
                indLines_[chartName] = 0;
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
            try 
            {
                package_.Save();
            }
            catch
            {
                finishAllProcesses();
                package_.Save();
            }
        }

        public void open()
        {
            try
            {
                Excel.Workbook book = application_.Workbooks.Open(path_);
                application_.Visible = true;
                try
                {
                    if (!string.IsNullOrEmpty(lastSheet_))
                        book.Worksheets[lastSheet_].Activate();
                }
                catch
                {

                }
            }
            catch
            {
                const string kQuotes = "\"";
                finishAllProcesses();
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = "EXCEL.EXE";
                processInfo.Arguments = kQuotes + path_ + kQuotes;
                Process.Start(processInfo);
            }
        }

        public void createExcelInstance(string originalPath, string resPath, string name, string extension)
        {
            int nTryCopy = 100;
            try
            {
                // Retrieving a copy of an already running application
                try
                {
                    application_ = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                    application_.Visible = true;
                }
                catch
                {
                    application_ = new Excel.Application();
                    application_.Visible = true;
                }
                // Copying the template
                try
                {
                    File.Copy(originalPath, resPath, true);
                }
                catch
                {
                    application_.Workbooks.Close();
                    File.Copy(originalPath, resPath, true);
                }
            }
            catch
            {
                finishAllProcesses();
                while (--nTryCopy > 0)
                {
                    try
                    {
                        File.Copy(originalPath, resPath, true);
						break;
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void finishAllProcesses()
        {
            var processes = Process.GetProcessesByName("Excel");
            foreach (var proc in processes)
            { 
                proc.CloseMainWindow();
                proc.Close();
            }
        }

        private void Copy(ExcelObject another)
        {
            package_ = another.package_;
            charts_ = another.charts_;
            chartSheets_ = another.chartSheets_;
            path_ = another.path_;
			// Markers
            indMarkers_ = another.indMarkers_;
            standardMarkers_ = another.standardMarkers_;
            customMarkers_ = another.customMarkers_;
			// Lines
			indLines_ = another.indLines_;
			standardLines_ = another.standardLines_;
			customLines_ = another.customLines_;
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
						indLines_.Add(drawing.Name, 0);
                    }
                }
            }
        }

        Excel.Application application_;
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
        // Marker styles
        Dictionary<string, int> indMarkers_;
        List<MarkerProperty> standardMarkers_;
        Dictionary<string, List<MarkerProperty>> customMarkers_;
        const string markersSheetName_ = "Markers";
		// Line styles
	    Dictionary<string, int> indLines_;
		List<LineProperty> standardLines_;
		Dictionary<string, List<LineProperty>> customLines_;
		const string linesSheetName_ = "Lines";
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
	
	public class LineProperty
    {
        public eLineStyle lineStyle;
        public bool isMarkersEnabled;
        public bool isTransparent;
    }
}
