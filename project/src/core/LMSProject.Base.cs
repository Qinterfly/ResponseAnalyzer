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
        public int selectSignals(in ComponentGeometry componentSet) 
        {
            DataWatch dataWatch = app_.ActiveBook.FindDataWatch("Navigator_SelectedOIDs");
            IData dataSelected = dataWatch.Data;
            int iSelected = 0;
            try
            {
                AttributeMap attributeMap = dataSelected.AttributeMap;
                int nSelected = attributeMap.Count;
                signals_ = new Dictionary<string, Dictionary<ChartDirection, ResponseHolder>>();
                for (int iSignal = 0; iSignal != nSelected; ++iSignal)
                {
                    DataWatch blockWatch = app_.FindDataWatch(attributeMap[iSignal]);
                    string t = blockWatch.Data.Type;
                    if (blockWatch.Data.Type != "LmsHq::DataModelI::Expression::CBufferIBlock")
                        continue;
                    // Retreiving signals
                    IBlock2 signal = blockWatch.Data;
                    Array frequency = signal.XValues;
                    Array responseMS2  = signal.YValues; // Units: m / s ^ 2
                    // Retrieving additional info
                    AttributeMap properties = signal.Properties;
                    double sign = 1.0;
                    if (properties["Point direction sign"] == "-")
                        sign = -1.0;
                    string componentName = properties["Point id component"];
                    string nodeName = properties["Point id node"];
                    uint indNode = componentSet.mapNodeNames[componentName][nodeName];
                    double[,] refAngles = (double[,])componentSet.nodeAngles[componentName];
                    double multAngles = 1.0;
                    string signalName = signal.Label;
                    for (int k = 0; k != 3; ++k)
                        multAngles *= Math.Cos(refAngles[indNode, k]);
                    // Integrating the acceleration twice
                    IBlock2 signalIntergral2 = app_.cmd.BLOCK_INTEGRATE(signal, 2, CONST_EnumIntegrationMethod.IntegrationMethodBode);
                    Array responseM = signalIntergral2.YValues; // Units: m (!)
                    double[,] responseMM = (double[,])responseM.Clone();
                    int nResponse = responseMM.GetLength(0);
                    // Normalizing signals
                    double resMultMS2 = sign * multAngles;
                    double resMultMM = resMultMS2 * METERS_TO_MILLIMETERS;
                    for (int k = 0; k != nResponse; ++k)
                    {
                        // Meters per seconds2
                        responseMM[k, 0] *= resMultMS2;
                        responseMM[k, 1] *= resMultMS2 * (-1.0);
                        // Millimeters
                        responseMM[k, 0] *= resMultMM;
                        responseMM[k, 1] *= resMultMM * (-1.0);
                    }
                    // Creating the holder for the response
                    ResponseHolder responseHolder = new ResponseHolder();
                    responseHolder.signalName = signalName;
                    responseHolder.frequency = (double[]) frequency;
                    responseHolder.data[SignalUnits.MILLIMETERS] = (double[,])responseMM;
                    responseHolder.data[SignalUnits.METERS_PER_SECOND2] = (double[,]) responseMS2;
                    string direction = properties["Point direction absolute"];
                    ChartDirection chartDir = ChartDirection.UNKNOWN;
                    switch (direction)
                    {
                        case "X":
                            chartDir = ChartDirection.X;
                            break;
                        case "Y":
                            chartDir = ChartDirection.Y;
                            break;
                        case "Z":
                            chartDir = ChartDirection.Z;
                            break;
                    }
                    // Adding the results
                    Dictionary<ChartDirection, ResponseHolder> ptrResponse = null;
                    string nodeFullName = properties["Point id"];
                    if (!signals_.ContainsKey(nodeFullName))
                        signals_.Add(nodeFullName, new Dictionary<ChartDirection, ResponseHolder>());
                    ptrResponse = signals_[nodeFullName];
                    ptrResponse.Add(chartDir, responseHolder);
                    ++iSelected;
                }
                return iSelected;
        }
            catch
            {
                return -1;
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
        public Dictionary<string, Dictionary<ChartDirection, ResponseHolder>> signals_ { get; set; }
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
        public string signalName { get; set; }
    }

}
