using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace ResponseAnalyzer
{
    class ExcelObject
    {
        public ExcelObject(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists && fileInfo.Extension.Equals(".xlsx"))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                package_ = new ExcelPackage(fileInfo);
                charts_ = new List<ExcelDrawing>();
                retrieveCharts();
            }
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
                        charts_.Add(drawing);
                }
            }
        }

        ExcelPackage package_;
        List<ExcelDrawing> charts_;

    }
}
