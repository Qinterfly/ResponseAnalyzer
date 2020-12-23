using System;
using System.Collections.Generic;
using LMSTestLabAutomation;

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
        public bool isProjectOpened()
        { 
            return app_ != null; 
        }

        // Select a folder through the navigator
        public int selectSignals() 
        {
            DataWatch dataWatch = app_.ActiveBook.FindDataWatch("Navigator_SelectedOIDs");
            IData dataSelected = dataWatch.Data;
            int iSelected = 0;
            try
            {
                AttributeMap attributeMap = dataSelected.AttributeMap;
                int nSelected = attributeMap.Count;
                Dictionary<string, ResponseHolder> signals_ = new Dictionary<string, ResponseHolder>();
                for (int iSignal = 0; iSignal != nSelected; ++iSignal)
                {
                    DataWatch blockWatch = app_.FindDataWatch(attributeMap[iSignal]);
                    string t = blockWatch.Data.Type;
                    if (blockWatch.Data.Type != "LmsHq::DataModelI::Expression::CBufferIBlock")
                        continue;
                    IBlock2 signal = blockWatch.Data;
                    //IBlock2 signalIntegral2 = ICmd
                    IBlock2 signalIntergral2 = app_.cmd.BLOCK_INTEGRATE(signal, 2, CONST_EnumIntegrationMethod.IntegrationMethodBode);
                    Array frequency = signal.XValues;
                    Array responseMS2  = signal.YValues; // Units: m / s ^ 2
                    Array responseM = signalIntergral2.YValues; // Units: m (!)
                    double[,] responseMM = (double[,])responseM.Clone();
                    int nResponse = responseMM.GetLength(0);
                    for (int k = 0; k != nResponse; ++k)
                        for (int m = 0; m != 2; ++m )
                            responseMM[k, m] *= METERS_TO_MILLIMETERS; // m -> mm
                    ResponseHolder responseHolder = new ResponseHolder();
                    responseHolder.frequency = (double[]) frequency;
                    responseHolder.data[SignalUnits.MILLIMETERS] = (double[,])responseMM;
                    responseHolder.data[SignalUnits.METERS_PER_SECOND2] = (double[,]) responseMS2;
                    signals_.Add(signal.Label, responseHolder);
                    ++iSelected;
                }
                return iSelected;
        }
            catch
            {
                return 0;
            }
        }

        // Properties and fields
        // Constants
        private const int DEPTH_SEARCH = 100;
        private const char PATH_DELIMITER = '/';
        private const double METERS_TO_MILLIMETERS = 1000.0;
        // Project info
        private readonly LMSTestLabAutomation.Application app_ = null;
        private readonly IDatabase database_;
        public LMSTestLabAutomation.IGeometry geometry_ { get; }
        public Dictionary<string, ResponseHolder> signals_ { get; }
        private string path_;
    }

    public class ResponseHolder 
    {
        public ResponseHolder()
        {
            data = new Dictionary<SignalUnits, double[,]>();
            data.Add(SignalUnits.MILLIMETERS, null);
            data.Add(SignalUnits.METERS_PER_SECOND2, null);
        }
        public Dictionary<SignalUnits, double[,]> data { get; set; }
        public double[] frequency { get; set; }
    }

}
