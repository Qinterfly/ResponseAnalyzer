using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void convertSelection(string baseChart, string refChart, ChartTypes baseType, ChartTypes refType)
        {
            List<ISelection> refObjects = chartSelection_[refChart];
            List<ISelection> tempObjects = refObjects.GetRange(0, refObjects.Count);
            // Convert to nodes
            if (isNodeType(baseType))
            {
                // Nodes -> Nodes
                if (isNodeType(refType))
                    chartSelection_[baseChart] = tempObjects;
                // Lines -> Nodes
                if (isLineType(refType))
                {
                    foreach (ISelection item in tempObjects)
                    {
                        List<string> nodeNames = (List<string>)item.retrieveSelection();
                        foreach (string name in nodeNames)
                            chartSelection_[baseChart].Add(new Nodes { nodeName_ = name });
                    }
                }
            }
            // Convert to lines
            if (isLineType(baseType))
            {
                // Lines -> Lines
                if (isLineType(refType))
                    chartSelection_[baseChart] = tempObjects;
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
                    chartSelection_[baseChart] = res;
                }

            }
        }

        private bool isNodeType(ChartTypes type)
        {
            return type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF || type == ChartTypes.FORCE;
        }

        private bool isLineType(ChartTypes type)
        {
            return type == ChartTypes.MODESET;
        }

        private void createDependency(ChartTypes masterType, ChartTypes slaveType)
        {
            string slaveChart = null;
            string masterChart = null;
            foreach (string chart in chartTypes_.Keys)
            {
                if (chartTypes_[chart] == masterType)
                    masterChart = chart;
                if (chartTypes_[chart] == slaveType)
                    slaveChart = chart;
                if (masterChart != null && slaveChart != null)
                {
                    chartDependency_[slaveChart] = masterChart;
                    break;
                }
            }
        }

        private void resetDependencies()
        {
            List<string> keys = new List<string>(chartDependency_.Keys);
            foreach (string chart in keys)
                chartDependency_[chart] = null;
        }

        private void resolveDependencies()
        {
            foreach (string chart in chartDependency_.Keys)
            {
                string masterChart = chartDependency_[chart];
                if (masterChart == null)
                    continue;
                // Properties
                chartDirection_[chart] = chartDirection_[masterChart];
                chartAxis_[chart] = chartAxis_[masterChart];
                chartNormalization_[chart] = chartNormalization_[masterChart];
                chartUnits_[chart] = chartUnits_[masterChart];
                chartSwapAxes_[chart] = chartSwapAxes_[masterChart];
                // Data
                chartSelection_[chart] = chartSelection_[masterChart];
            }
        }
    }
}
