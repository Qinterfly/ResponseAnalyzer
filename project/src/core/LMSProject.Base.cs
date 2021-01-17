using System;
using System.Collections.Generic;
using LMSTestLabAutomation;

namespace ResponseAnalyzer
{
    using ResponseArray = Dictionary<string, Dictionary<ChartDirection, List<Response>>>;

    public partial class LMSProject
    {

        // Create a project
        public LMSProject(in String filePath)
        {
            try
            {
                app_ = new Application();
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
            // Allocating memory for further responses
            signals_ = new ResponseArray();
            multiSignals_ = new ResponseArray();
        }

        // User's methods
        public bool isProjectOpened()
        { 
            return app_ != null; 
        }

        // Clear signals selected at once
        public void clearSignals()
        {
            signals_ = new ResponseArray();
        }

        // Clear accumulated signals
        public void clearAccumulatedSignals()
        {
            multiSignals_ = new ResponseArray();
        }

        // Select a folder through the navigator
        public int selectSignals(in ComponentGeometry componentSet, bool isAccumulate) 
        {
            DataWatch dataWatch = app_.ActiveBook.FindDataWatch("Navigator_SelectedOIDs");
            IData dataSelected = dataWatch.Data;
            int iSelected = 0;
            try
            {
                AttributeMap attributeMap = dataSelected.AttributeMap;
                int nSelected = attributeMap.Count;
                ResponseArray resultSignals;
                if (isAccumulate)
                    resultSignals = multiSignals_;
                else
                {
                    signals_ = new ResponseArray();
                    resultSignals = signals_;
                }             
                for (int iSignal = 0; iSignal != nSelected; ++iSignal)
                {
                    DataWatch blockWatch = app_.FindDataWatch(attributeMap[iSignal]);
                    if (blockWatch.Data.Type != "LmsHq::DataModelI::Expression::CBufferIBlock")
                        continue;
                    // Retrieving path
                    IData dataOID = attributeMap[iSignal].AttributeMap["OID"];
                    string path = dataOID.AttributeMap["Path"].AttributeMap["PathString"];
                    // Retreiving signals
                    IBlock2 signal = blockWatch.Data;
                    double[] frequencies = (double[])signal.XValues;
                    double[,] responseMS2  = (double[,])signal.YValues; // Units: m / s ^ 2
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
                    double[,] responseMM = (double[,])signalIntergral2.YValues; // Units: m (!)
                    // Normalizing signals
                    double resMultMS2 = sign * multAngles;
                    double resMultMM = resMultMS2 * METERS_TO_MILLIMETERS;
                    int nResponse = responseMM.GetLength(0);
                    for (int k = 0; k != nResponse; ++k)
                    {
                        // Meters per seconds2
                        responseMS2[k, 0] *= resMultMS2;
                        responseMS2[k, 1] *= resMultMS2 * (-1.0);
                        // Meters -> Millimeters
                        responseMM[k, 0] *= resMultMM;
                        responseMM[k, 1] *= resMultMM * (-1.0);
                    }
                    // Creating the holder for the response
                    Response currentResponse = new Response();
                    currentResponse.signalName = signalName;
                    currentResponse.path = path;
                    currentResponse.frequencies = frequencies;
                    currentResponse.data[SignalUnits.MILLIMETERS] = responseMM;
                    currentResponse.data[SignalUnits.METERS_PER_SECOND2] = responseMS2;
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
                    string nodeFullName = properties["Point id"];
                    if (!resultSignals.ContainsKey(nodeFullName)) {
                        resultSignals.Add(nodeFullName, new Dictionary<ChartDirection, List<Response>>());
                    }
                    Dictionary<ChartDirection, List<Response>> dictDirections = null;
                    dictDirections = resultSignals[nodeFullName];
                    if (!dictDirections.ContainsKey(chartDir))
                        dictDirections.Add(chartDir, new List<Response>());
                    // Check if the signal duplicates an already added one
                    List<Response> existedResponses = dictDirections[chartDir];
                    bool isDuplicate = false;
                    foreach (Response tempResponse in existedResponses)
                    {
                        isDuplicate = tempResponse.equals(currentResponse);
                        if (isDuplicate)
                            break;
                    }
                    // Each node may have several responses along one direction (selection from several folders)
                    if (!isDuplicate)
                        existedResponses.Add(currentResponse); 
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
        private readonly Application app_ = null;
        private readonly IDatabase database_;
        public IGeometry geometry_ { get; }
        public ResponseArray signals_ { get; set; }
        public ResponseArray multiSignals_ { get; set; } // Signals from several folders
        private string path_;
    }

}
