using LMSTestLabAutomation;
using System;

namespace ResponseAnalyzer
{
    public partial class LMSProject
    {

        // Create a project
        public LMSProject(in String filePath)
        {
            try
            {
                app_ = new LMSTestLabAutomation.Application();
                if (app_.Name == "")
                    app_.Init("-w DesktopStandard ");
                app_.OpenProject(filePath);
            }
            catch (System.Runtime.InteropServices.COMException exc)
            {
                app_ = null;
                throw exc;
            }
            // Store the database
            path_ = filePath;
            database_ = app_.ActiveBook.Database();
            geometry_ = (IGeometry)database_.GetItem("Geometry");
        }

        // User's methods
        public bool isProjectOpened() { return app_ != null; }

        // Properties and fields
        // Constants
        private const int DEPTH_SEARCH = 100;
        private const char PATH_DELIMITER = '/';
        // Project info
        private readonly LMSTestLabAutomation.Application app_ = null;
        private readonly IDatabase database_;
        public LMSTestLabAutomation.IGeometry geometry_ { get; }
        private string path_;
    }
}
