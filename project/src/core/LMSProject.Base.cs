using System;
using System.Collections.Generic;
using LMSTestLabAutomation;

namespace ResponseAnalyzer
{
    using ResponseArray = Dictionary<string, Dictionary<ChartDirection, List<Response>>>;
    using PairDouble = Tuple<double, double>;

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
                    Array frequency = signal.XValues;
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
                    Array responseM = signalIntergral2.YValues; // Units: m (!)
                    double[,] responseMM = (double[,])responseM.Clone();
                    int nResponse = responseMM.GetLength(0);
                    // Normalizing signals
                    double resMultMS2 = sign * multAngles;
                    double resMultMM = resMultMS2 * METERS_TO_MILLIMETERS;
                    for (int k = 0; k != nResponse; ++k)
                    {
                        // Meters per seconds2
                        responseMS2[k, 0] *= resMultMS2;
                        responseMS2[k, 1] *= resMultMS2 * (-1.0);
                        // Millimeters
                        responseMM[k, 0] *= resMultMM;
                        responseMM[k, 1] *= resMultMM * (-1.0);
                    }
                    // Creating the holder for the response
                    Response response = new Response();
                    response.signalName = signalName;
                    response.path = path;
                    response.frequencies = (double[]) frequency;
                    response.data[SignalUnits.MILLIMETERS] = responseMM;
                    response.data[SignalUnits.METERS_PER_SECOND2] = responseMS2;
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
                    dictDirections[chartDir].Add(response); // Each node may have several responses along one direction (selection from several folders)
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

    public class Response 
    {
        public Response()
        {
            data = new Dictionary<SignalUnits, double[,]>();
            data.Add(SignalUnits.MILLIMETERS, null);
            data.Add(SignalUnits.METERS_PER_SECOND2, null);
        }

        public PairDouble evaluateResonanceFrequency(ChartTypes type, SignalUnits units)
        {
            switch (type)
            {
                case ChartTypes.REAL_FREQUENCY:
                    return findRoot(frequencies, data[units], 0);
                case ChartTypes.IMAG_FREQUENCY:
                    return findPeak(frequencies, data[units], 1);
            }
            return null;
        }

        private PairDouble findPeak(double[] X, double[,] Y, int iColumn)
        {
            int nY = Y.Length;
            double prevValue = Y[0, iColumn];
            double sign = Y[1, iColumn] - prevValue;
            double root = -1.0;
            double amplitude = 0.0;
            bool isFound = false;
            for (int i = 1; i != nY; ++i)
            {
                isFound = sign * (Y[i, iColumn] - prevValue) < 0.0;
                if (isFound)
                {
                    root = (X[i] + X[i - 1]) / 2.0;
                    // Calculating amplitude
                    amplitude = Math.Sqrt(Math.Pow(Y[i - 1, 0], 2.0) + Math.Pow(Y[i - 1, 1], 2.0)); // First
                    amplitude += Math.Sqrt(Math.Pow(Y[i, 0], 2.0) + Math.Pow(Y[i, 1], 2.0));        // Second
                    amplitude /= 2.0;                                                               // Mean
                    break;
                }
                prevValue = Y[i, iColumn];
            }
            return isFound ? new PairDouble(root, amplitude) : null;
        }

        private PairDouble findRoot(double[] X, double[,] Y, int iColumn)
        {
            int nY = Y.Length;
            double prevValue = Y[0, iColumn];
            double root = -1.0;
            double amplitude = 0.0;
            bool isFound = false;
            for (int i = 1; i != nY; ++i)
            {
                isFound = Y[i, iColumn] * prevValue < 0.0;
                if (isFound)
                {
                    root = (X[i] + X[i - 1]) / 2.0;
                    // Calculating amplitude
                    amplitude = Math.Sqrt(Math.Pow(Y[i - 1, 0], 2.0) + Math.Pow(Y[i - 1, 1], 2.0)); // First
                    amplitude += Math.Sqrt(Math.Pow(Y[i, 0], 2.0) + Math.Pow(Y[i, 1], 2.0));        // Second
                    amplitude /= 2.0;                                                               // Mean
                    break;
                }
                prevValue = Y[i, iColumn];
            }
            return isFound ? new PairDouble(root, amplitude) : null;
        }

        // Data
        public Dictionary<SignalUnits, double[,]> data { get; set; }
        public double[] frequencies { get; set; }
        // Info
        public string signalName { get; set; }
        public string path { get; set; }
    }

}
