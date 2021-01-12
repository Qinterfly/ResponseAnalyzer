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
                    charts_.selection[baseChart] = tempObjects;
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
                    charts_.selection[baseChart] = tempObjects;
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
            return type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF || type == ChartTypes.FORCE;
        }

		// Check if a value type of a graph is linear
        private bool isLineType(ChartTypes type)
        {
            return type == ChartTypes.MODESET;
        }

        // Select all items in a listbox
        private void selectAllItems(ListBox listBox)
        {
            int nItems = listBox.Items.Count;
            for (int i = 0; i != nItems; ++i)
                listBox.SetSelected(i, true);
            listBox.TopIndex = 0;
        }

        // Creating dependency between charts with specified types
        private void createDependency(ChartTypes masterType, ChartTypes slaveType)
        {
            string slaveChart = null;
            string masterChart = null;
            foreach (string chart in charts_.type.Keys)
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
    }

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

        public Dictionary<string, ChartTypes> type;
        public Dictionary<string, SignalUnits> units;
        public Dictionary<string, List<ISelection>> selection;
        public Dictionary<string, ChartDirection> direction;
        public Dictionary<string, double> normalization;
        public Dictionary<string, ChartDirection> axis;
        public Dictionary<string, bool> swapAxes;
        public Dictionary<string, string> dependency;
    }
}
