using System;
using System.Windows.Forms;
using System.Collections.Generic;
using OpenTK;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        // Defining a template type
        private void comboBoxTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            ChartTypes iSelected = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            if (charts_.type[chart] == iSelected) // If the type has not changed
                return;
            charts_.type[chart] = iSelected;
            // Clearing data
            charts_.selection[chart].Clear();
            treeTemplateObjects.Nodes.Clear();
            // Constructing dependencies
            resetDependencies();
            createDependency(ChartTypes.IMAGFRF, ChartTypes.REALFRF);
            createDependency(ChartTypes.MULTIIMAGFRF, ChartTypes.MULTIREALFRF);
            setDependencyEnabled();
        }

        // Defining template units
        private void comboBoxTemplateUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            SignalUnits iSelected = (SignalUnits)comboBoxTemplateUnits.SelectedIndex;
            charts_.units[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Defining a template direction
        private void comboBoxTemplateDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateDirection.SelectedIndex;
            charts_.direction[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Defining a template normalization
        private void numericNormalization_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            charts_.normalization[listBoxTemplateCharts.SelectedItem.ToString()] = (double)numericTemplateNormalization.Value;
        }

        // Deciding along which axis take coordinates
        private void comboBoxTemplateAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateAxis.SelectedIndex;
            charts_.axis[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Check if it is needed to swap axes
        private void checkBoxSwapAxes_CheckedChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            charts_.swapAxes[chart] = checkBoxSwapAxes.Checked;
        }

        // Fill out normalization value automatically
        private void numericTemplateNormalization_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.F1)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            List<ISelection> selection = charts_.selection[chart];
            if (selection.Count == 0 || charts_.type[chart] != ChartTypes.MODESET || charts_.axis[chart] == ChartDirection.UNKNOWN)
                return;
            int direction = (int)charts_.axis[chart] - 1;
            string[] selectionInfo;
            Vector3d tCoordinates;
            double tempValue;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            foreach (ISelection line in selection)
            {
                List<string> nodes = (List<string>)line.retrieveSelection();
                foreach (string node in nodes)
                {
                    selectionInfo = node.Split(selectionDelimiter_);
                    tCoordinates = modelRenderer_.getNodeCoordinates(selectionInfo[0], selectionInfo[1]);
                    tempValue = tCoordinates[direction];
                    if (tempValue > maxValue)
                        maxValue = tempValue;
                    if (tempValue < minValue)
                        minValue = tempValue;
                }

            }
            tempValue = maxValue - minValue;
            if (tempValue >= (double)numericTemplateNormalization.Minimum && tempValue <= (double)numericTemplateNormalization.Maximum)
                numericTemplateNormalization.Value = (decimal)tempValue;
        }
    }
}
