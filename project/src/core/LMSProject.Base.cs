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
            forces_ = new ResponseArray();
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
            signals_.Clear();
        }

        // Clear accumulated signals
        public void clearAccumulatedSignals()
        {
            multiSignals_.Clear();
        }

        // Select a folder through the navigator
        public int selectSignals(in ComponentGeometry componentSet, bool isMulti)
        {
            DataWatch dataWatch = app_.ActiveBook.FindDataWatch("Navigator_SelectedOIDs");
            IData dataSelected = dataWatch.Data;
            int iSelected = 0;
            bool isResolveReferences = false;
            try
            {
                AttributeMap attributeMap = dataSelected.AttributeMap;
                int nSelected = attributeMap.Count;
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
                    AttributeMap properties = signal.Properties;
                    string measuredQuantity = properties["Measured quantity"];
                    Response currentResponse = null;
                    bool isAdded = false;
                    if (measuredQuantity.Equals("Acceleration")) {
                        currentResponse = retrieveAcceleration(path, componentSet, signal, properties);
                        ResponseArray resultSignals;
                        if (isMulti)
                            resultSignals = multiSignals_;
                        else
                            resultSignals = signals_;
                        isAdded = addResponse(properties, currentResponse, resultSignals);
                    }
                    else if (measuredQuantity.Equals("Force"))
                    {
                        currentResponse = retrieveForce(path, componentSet, signal, properties);
                        isAdded = addResponse(properties, currentResponse, forces_);
                        isResolveReferences = true;
                    }
                    if (!isAdded)
                        continue;
                    ++iSelected;
                }
                if (isResolveReferences)
                {
                    resolveReferences(signals_);
                    resolveReferences(multiSignals_);
                }
                return iSelected;
            }
            catch
            {
                return -1;
            }
        }

        public Response retrieveAcceleration(in string path, in ComponentGeometry componentSet, in IBlock2 signal, in AttributeMap properties)
        {
            const int kFRFType = 12;
            double[] frequency = (double[])signal.XValues;
            double[,] responseMS2 = (double[,])signal.YValues; // Units: m/s^2 or (m/s^2)/N 
            // Retrieving additional info
            double sign = 1.0;
            if (properties["Point direction sign"] == "-")
                sign = -1.0;
            string componentName = properties["Point id component"];
            string nodeName = properties["Point id node"];
            if (!componentSet.mapNodeNames.ContainsKey(componentName) || !componentSet.mapNodeNames[componentName].ContainsKey(nodeName))
                return null;
            uint indNode = componentSet.mapNodeNames[componentName][nodeName];
            double[,] refAngles = (double[,])componentSet.nodeAngles[componentName];
            double multAngles = 1.0;
            for (int k = 0; k != 3; ++k)
                multAngles *= Math.Cos(refAngles[indNode, k]);
            // Integrating the acceleration twice
            IBlock2 signalIntergral2 = app_.cmd.BLOCK_INTEGRATE(signal, 2, CONST_EnumIntegrationMethod.IntegrationMethodBode);
            double[,] responseMM = (double[,])signalIntergral2.YValues; // Units: m or m/N (!)
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
            currentResponse.originalRun = properties["Original run"];
            currentResponse.signalName = signal.Label;
            currentResponse.path = path;
            currentResponse.frequency = frequency;
            // Checking the reference point in case of FRF                        
            int iFunctionClass = properties["Function class"].AttributeMap["EnumValue"];
            bool isFRF = iFunctionClass == kFRFType;
            if (isFRF)
            {
                ResponseReference reference = new ResponseReference();
                reference.nodeFullName = properties["Reference point id"];
                reference.direction = getDirection(properties["Reference point direction absolute"]);
                reference.directionSign = properties["Reference point direction sign"] == "-" ? -1.0 : 1.0;
                currentResponse.reference = reference;
                currentResponse.data.Add(SignalUnits.MILLIMETERS_PER_FORCE, responseMM);
                currentResponse.data.Add(SignalUnits.METERS_PER_SECOND2_PER_FORCE, responseMS2);
            }
            else
            {
                currentResponse.data[SignalUnits.MILLIMETERS] = responseMM;
                currentResponse.data[SignalUnits.METERS_PER_SECOND2] = responseMS2;
            }
            return currentResponse;
        }

        public Response retrieveForce(in string path, in ComponentGeometry componentSet, in IBlock2 signal, in AttributeMap properties)
        {
            double[] frequency = (double[])signal.XValues;
            double[,] force = (double[,])signal.YValues; // Units: N
            // Retrieving additional info
            double sign = 1.0;
            if (properties["Point direction sign"] == "-")
                sign = -1.0;
            string componentName = properties["Point id component"];
            string nodeName = properties["Point id node"];
            if (!componentSet.mapNodeNames.ContainsKey(componentName) || !componentSet.mapNodeNames[componentName].ContainsKey(nodeName))
                return null;
            uint indNode = componentSet.mapNodeNames[componentName][nodeName];
            double[,] refAngles = (double[,])componentSet.nodeAngles[componentName];
            double multAngles = sign;
            for (int k = 0; k != 3; ++k)
                multAngles *= Math.Cos(refAngles[indNode, k]);
            // Normalizing force
            int nResponse = force.GetLength(0);
            for (int k = 0; k != nResponse; ++k)
            {
                force[k, 0] *= multAngles;
                force[k, 1] *= multAngles * (-1.0); 
            }
            Response currentResponse = new Response();
            currentResponse.originalRun = properties["Original run"];
            currentResponse.signalName = signal.Label;
            currentResponse.path = path;
            currentResponse.frequency = frequency;
            currentResponse.data.Add(SignalUnits.NEWTON, force);
            return currentResponse;
        }

        public bool addResponse(in AttributeMap properties, in Response currentResponse, in ResponseArray resultSignals)
        {
            if (currentResponse == null)
                return false;
            // Setting the direction
            string direction = properties["Point direction absolute"];
            ChartDirection chartDir = getDirection(direction);
            // Adding the results
            string nodeFullName = properties["Point id"];
            if (!resultSignals.ContainsKey(nodeFullName))
            {
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
            return !isDuplicate;
        }

        public ChartDirection getDirection(string direction)
        {
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
            return chartDir;
        }

        void resolveReferences(in ResponseArray signals)
        {
            var nodeKeys = signals.Keys;
            foreach (string node in nodeKeys)
            {
                Dictionary<ChartDirection, List<Response>> directionalResponses = signals[node];
                var directionKeys = directionalResponses.Keys;
                foreach (ChartDirection direction in directionKeys)
                {
                    List<Response> listResponses = directionalResponses[direction];
                    foreach (Response response in listResponses)
                    {
                        ResponseReference reference = response.reference;
                        if (reference != null && !reference.isResolved)
                        {
                            Response force = findForce(response.originalRun, reference);
                            if (force == null)
                                continue;
                            double[,] forceData = force.data[SignalUnits.NEWTON];
                            double[,] frfAccelerationData = response.data[SignalUnits.METERS_PER_SECOND2_PER_FORCE];
                            int nResponse = frfAccelerationData.GetLength(0);
                            if (nResponse != forceData.GetLength(0))
                                continue;
                            double[,] frfDisplacementData = response.data[SignalUnits.MILLIMETERS_PER_FORCE];
                            double realForce, imagForce;
                            for (int i = 0; i != nResponse; ++i)
                            {
                                realForce = forceData[i, 0];
                                imagForce = forceData[i, 1];
                                // Acceleration
                                frfAccelerationData[i, 0] *= realForce;
                                frfAccelerationData[i, 1] *= imagForce;
                                // Displacement
                                frfDisplacementData[i, 0] *= realForce;
                                frfDisplacementData[i, 1] *= imagForce;
                            }
                            reference.isResolved = true;
                        }
                    }
                }
            }
        }

        public Response findForce(in string originalRun, in ResponseReference reference)
        {
            string refNode = reference.nodeFullName;
            ChartDirection refDirection = reference.direction;
            if (forces_.ContainsKey(refNode))
            {
                if (forces_[refNode].ContainsKey(refDirection))
                {
                    List<Response> listForces = forces_[refNode][refDirection];
                    foreach (Response force in listForces)
                    {
                        if (force.originalRun.Equals(originalRun))
                            return force;
                    }
                }
            }
            return null;
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
        public ResponseArray forces_ { get; set; }
        public ResponseArray signals_ { get; set; }
        public ResponseArray multiSignals_ { get; set; } // Signals from several folders
        private string path_;
    }

}
