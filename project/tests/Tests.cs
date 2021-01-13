using System.IO;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void testRender()
        {
            //string path = Path.GetFullPath(@"..\..\..\examples\Plate.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Rib.lms");
            //string path = Path.GetFullPath(@"..\..\..\examples\Airplane.lms");
            string path = Path.GetFullPath(@"..\..\..\examples\Yak130.lms");
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
            string path = Path.GetFullPath(templateDir + "Base.xlsx");
            excelTemplate_ = new ExcelObject(path);
            textBoxExcelTemplatePath.Text = path;
            textBoxDirectoryExcel.Text = @"C:\Users\qinterfly\Desktop";
            textBoxNameExcel.Text = "TestMe";
            updateExcelTemplateList();
            setProjectEnabled();
            //charts_.read(templateDir + "Base.rep", modelRenderer_.containesNode, selectionDelimiter_);
            //listBoxTemplateCharts_SelectedIndexChanged();
        }
    }
}
