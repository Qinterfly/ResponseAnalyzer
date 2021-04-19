using System;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
		// Converting selection type
        private void convertSelection(string baseChart, string refChart, ChartTypes baseType, ChartTypes refType)
        {
            List<ISelection> refObjects = charts_.selection[refChart];
            List<ISelection> tempObjects = refObjects.GetRange(0, refObjects.Count);
            // Convert to nodes
            if (isNodeType(baseType))
            {
                // Nodes -> Nodes
                if (isNodeType(refType))
                    charts_.selection[baseChart].AddRange(tempObjects);
                // Lines -> Nodes
                if (isLineType(refType))
                {
                    foreach (ISelection item in tempObjects)
                    {
                        List<string> nodeNames = (List<string>)item.retrieveSelection();
                        foreach (string name in nodeNames)
                            charts_.selection[baseChart].Add(new Nodes { nodeName_ = name });
                    }
                }
            }
            // Convert to lines
            if (isLineType(baseType))
            {
                // Lines -> Lines
                if (isLineType(refType))
                    charts_.selection[baseChart].AddRange(tempObjects);
                // Nodes -> Line
                if (isNodeType(refType))
                {
                    Lines line = new Lines();
                    line.nodeNames_ = new List<string>();
                    line.lineName_ = "Line" + indLine_.ToString();
                    foreach (ISelection item in tempObjects)
                        line.nodeNames_.Add((string)item.retrieveSelection());
                    List<ISelection> res = new List<ISelection>();
                    res.Add((ISelection)line);
                    charts_.selection[baseChart] = res;
                }

            }
        }
		
		// Check if a value type of a graph is nodal
        private bool isNodeType(ChartTypes type)
        {
            return type == ChartTypes.REAL_FRF || type == ChartTypes.IMAG_FRF 
                   || type == ChartTypes.MULTI_REAL_FRF || type == ChartTypes.MULTI_IMAG_FRF
                   || type == ChartTypes.REAL_FREQUENCY || type == ChartTypes.IMAG_FREQUENCY;
        }

		// Check if a value type of a graph is linear
        private bool isLineType(ChartTypes type)
        {
            return type == ChartTypes.MODESET;
        }

        // Select items in a listbox
        private void selectItems(ListBox listBox, EventHandler method, List<int> indices = null)
        {
            int nItems = listBox.Items.Count;
            listBox.SelectedIndexChanged -= method;
            if (indices == null)
            {
                for (int i = 0; i != nItems; ++i)
                    listBox.SetSelected(i, true);
            }
            else
            {
                foreach (int index in indices)
                    listBox.SetSelected(index, true);
            }
            listBox.SelectedIndexChanged += method;
            listBox.TopIndex = 0;
        }

        // Creating dependency between charts with specified types
        private void createDependency(ChartTypes masterType, ChartTypes slaveType)
        {
            string slaveChart = null;
            string masterChart = null;
            // Check if there are several charts with the same type. 
            // If it is the case, we do not have enough information to create dependencies
            var listCharts = charts_.type.Keys;
            int iMasterType = 0;
            int iSlaveType = 0;
            foreach (string chart in listCharts)
            {
                if (charts_.type[chart] == masterType)
                    ++iMasterType;
                if (charts_.type[chart] == slaveType)
                    ++iSlaveType;
            }
            if (iMasterType > 1 || iSlaveType > 1)
                return;
            // Create dependencies
            foreach (string chart in listCharts)
            {
                if (charts_.type[chart] == masterType)
                    masterChart = chart;
                if (charts_.type[chart] == slaveType)
                    slaveChart = chart;
                if (masterChart != null && slaveChart != null)
                {
                    charts_.dependency[slaveChart] = masterChart;
                    break;
                }
            }
        }
		
		// Reset all dependencies between charts
        private void resetDependencies()
        {
            List<string> keys = new List<string>(charts_.dependency.Keys);
            foreach (string chart in keys)
                charts_.dependency[chart] = null;
        }
		
		// Copy template properties in accordance with the dependency map
        private void resolveDependencies()
        {
            foreach (string chart in charts_.dependency.Keys)
            {
                string masterChart = charts_.dependency[chart];
                if (masterChart == null)
                    continue;
                // Properties
                charts_.direction[chart] = charts_.direction[masterChart];
                charts_.axis[chart] = charts_.axis[masterChart];
                charts_.normalization[chart] = charts_.normalization[masterChart];
                charts_.units[chart] = charts_.units[masterChart];
                charts_.swapAxes[chart] = charts_.swapAxes[masterChart];
                // Data
                charts_.selection[chart] = charts_.selection[masterChart];
            }
        }

        // Retrieve force value from a signal path
        private string getForceValue(string path)
        {
            int lenPath = path.Length;
            bool isFound = false;
            bool isValueStarted = false;
            string force = null;
            for (int k = lenPath - 1; k >= 0; --k)
            {
                if (isFound)
                {
                    // Copying value
                    if (double.TryParse(path[k].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double tempVal))
                    {
                        force = path[k] + force;
                        isValueStarted = true;
                    }
                    else if (isValueStarted)
                    {
                        break;
                    }
                }
                // Looking for force units (En / Ru)
                if (path[k] == 'Н' || path[k] == 'Н')
                    isFound = true;
            }
            return force;
        }
    }

    [Serializable]
    public class ChartsData
    {
        public ChartsData()
        {
            type = new Dictionary<string, ChartTypes>();
            units = new Dictionary<string, SignalUnits>();
            selection = new Dictionary<string, List<ISelection>>();
            direction = new Dictionary<string, ChartDirection>();
            normalization = new Dictionary<string, double>();
            axis = new Dictionary<string, ChartDirection>();
            swapAxes = new Dictionary<string, bool>();
            dependency = new Dictionary<string, string>();
        }

        public void write(string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                formatter.Serialize(stream, this);
        }

        public void read(string fileName, Func<string, string, bool> checkNode, char nodeNameDelimiter)
        {
            // Reading another instance
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                ChartsData another = (ChartsData)formatter.Deserialize(stream);
                List<string> anotherCharts = new List<string>(another.type.Keys);
                // Copying fields
                string[] selectionInfo;
                bool isLineCorrect;
                foreach (string chart in anotherCharts)
                {
                    if (!type.ContainsKey(chart))
                        continue;
                    // Properties
                    type[chart] = another.type[chart];
                    units[chart] = another.units[chart];
                    direction[chart] = another.direction[chart];
                    normalization[chart] = another.normalization[chart];
                    axis[chart] = another.axis[chart];
                    swapAxes[chart] = another.swapAxes[chart];
                    dependency[chart] = another.dependency[chart];
                    // Selection
                    List<ISelection> checkedSelection = new List<ISelection>();
                    foreach (ISelection item in another.selection[chart])
                    {
                        // Nodes
                        if (item.GetType() == typeof(Nodes))
                        {
                            string fullName = (string)item.retrieveSelection();
                            selectionInfo = fullName.Split(nodeNameDelimiter);
                            if (checkNode(selectionInfo[0], selectionInfo[1]))
                                checkedSelection.Add(item); 
                        }
                        // Lines
                        if (item.GetType() == typeof(Lines))
                        {
                            List<string> nodes = (List<string>)item.retrieveSelection();
                            isLineCorrect = true;
                            foreach (string fullName in nodes)
                            {
                                selectionInfo = fullName.Split(nodeNameDelimiter);
                                // If the current geometry contains all the specified nodes 
                                if (!checkNode(selectionInfo[0], selectionInfo[1]))
                                {
                                    isLineCorrect = false;
                                    break;
                                }
                            }
                            if (isLineCorrect)
                                checkedSelection.Add(item);
                        }
                    }
                    selection[chart] = checkedSelection;
                }
            }
        }

        public Dictionary<string, List<ISelection>> selection;
        public Dictionary<string, ChartTypes> type;
        public Dictionary<string, SignalUnits> units;
        public Dictionary<string, ChartDirection> direction;
        public Dictionary<string, double> normalization;
        public Dictionary<string, ChartDirection> axis;
        public Dictionary<string, bool> swapAxes;
        public Dictionary<string, string> dependency;
        public const string binaryExtension = ".rep";
    }
}
