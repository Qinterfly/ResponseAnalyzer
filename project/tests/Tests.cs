using System.IO;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void testRender()
        {
            //string path = Path.GetFullPath(@"..\..\..\examples\Plate.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Rib.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Airplane.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Yak130.lms");
            string path = Path.GetFullPath(@"..\..\..\examples\MC-21PoslePV.lms");
            project = new LMSProject(path);
            textBoxProjectPath.Text = path;
            modelRenderer_.setGeometry(project.geometry_);
            modelRenderer_.setView(LMSModel.Views.ISOMETRIC);
            createComponentStrips();
            setProjectEnabled();
        }
        private void testExcel()
        {
            string templateDir = @"..\..\..\templates\";
            string path = Path.GetFullPath(templateDir + "MC-21.xlsx");
            excelTemplate_ = new ExcelObject(path);
            textBoxExcelTemplatePath.Text = path;
            textBoxDirectoryExcel.Text = @"C:\Users\qinterfly\Desktop";
            textBoxNameExcel.Text = "TestMe";
            singleFrequencyIndices_ = new List<int>();
            multiFrequency_ = new Dictionary<string, double[]>();
            multiFrequencyIndices_ = new Dictionary<string, List<int>>();
            mapResponses_ = new Dictionary<string, string>();
            updateExcelTemplateList();
            setProjectEnabled();
            charts_.read(templateDir + "MC-21.rep", modelRenderer_.containesNode, selectionDelimiter_);
            listBoxTemplateCharts_SelectedIndexChanged();
        }
    }
}
