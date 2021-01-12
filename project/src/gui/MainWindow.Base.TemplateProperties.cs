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
            if (chartTypes_[chart] == iSelected) // If the type has not changed
                return;
            chartTypes_[chart] = iSelected;
            // Clearing data
            chartSelection_[chart].Clear();
            treeTemplateObjects.Nodes.Clear();
            // Constructing dependencies
            resetDependencies();
            createDependency(ChartTypes.IMAGFRF, ChartTypes.REALFRF);
            setDependencyEnabled();
        }

        // Defining template units
        private void comboBoxTemplateUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            SignalUnits iSelected = (SignalUnits)comboBoxTemplateUnits.SelectedIndex;
            chartUnits_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Defining a template direction
        private void comboBoxTemplateDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateDirection.SelectedIndex;
            chartDirection_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Defining a template normalization
        private void numericNormalization_ValueChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            chartNormalization_[listBoxTemplateCharts.SelectedItem.ToString()] = (double)numericTemplateNormalization.Value;
        }

        // Deciding along which axis take coordinates
        private void comboBoxTemplateAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            ChartDirection iSelected = (ChartDirection)comboBoxTemplateAxis.SelectedIndex;
            chartAxis_[listBoxTemplateCharts.SelectedItem.ToString()] = iSelected;
        }

        // Check if it is needed to swap axes
        private void checkBoxSwapAxes_CheckedChanged(object sender, EventArgs e)
        {
            if (listBoxTemplateCharts.Items.Count == 0)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            chartSwapAxes_[chart] = checkBoxSwapAxes.Checked;
        }

        // Fill out normalization value automatically
        private void numericTemplateNormalization_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.F1)
                return;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            List<ISelection> selection = chartSelection_[chart];
            if (selection.Count == 0 || chartTypes_[chart] != ChartTypes.MODESET || chartAxis_[chart] == ChartDirection.UNKNOWN)
                return;
            int direction = (int)chartAxis_[chart] - 1;
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
