using System;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        private void buttonSelectTestLab_Click(object sender, EventArgs e)
        {
            // Check if all the fields are correct
            if (!project.isProjectOpened() || !excelTemplate_.isOpened())
                return;
            int nSelected = project.selectSignals(modelRenderer_.componentSet_);
            labelSelectionInfo.Text = "Selected signals: " + nSelected.ToString();
            listBoxFoundSignals.Items.Clear();
            listBoxFrequencies.Items.Clear();
            if (nSelected < 1)
            {
                if (nSelected == -1)
                    setStatus("Wrong TestLab selection");
                return;
            }
            ResponseHolder response = null;
            foreach (string nodeName in project.signals_.Keys)
            {
                foreach (ChartDirection dir in project.signals_[nodeName].Keys)
                {
                    response = project.signals_[nodeName][dir];
                    listBoxFoundSignals.Items.Add(response.signalName);
                }
            }
            int k = 0;
            foreach (double freq in response.frequency)
            {
                listBoxFrequencies.Items.Add(freq.ToString("G4", CultureInfo.InvariantCulture));
                listBoxFrequencies.SetSelected(k++, true);
            }
            listBoxFrequencies.TopIndex = 0;
            // Resonance frequency
            textBoxResonanceFrequency.Clear();
            textBoxResonanceFrequency.Tag = -1;
        }

        private void listBoxFrequencies_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                selectAllItems(listBoxFrequencies);
        }

        private void selectAllItems(ListBox listBox)
        {
            int nItems = listBox.Items.Count;
            for (int i = 0; i != nItems; ++i)
                listBox.SetSelected(i, true);
            listBox.TopIndex = 0;
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                textBoxDirectoryExcel.Text = openFolderDialog.SelectedPath;
        }

        private void buttonProcess_Click(object sender = null, EventArgs e = null)
        {
            int iError = 0;
            ExcelObject excelResult = new ExcelObject(excelTemplate_, textBoxDirectoryExcel.Text, textBoxNameExcel.Text);
            // Checking the project, template and selected signals
            if (!project.isProjectOpened() || !excelTemplate_.isOpened() || listBoxFoundSignals.Items.Count == 0)
                return;
            // Retrieve selected frequencies
            List<int> selectedFreqIndicies = new List<int>();
            foreach (int index in listBoxFrequencies.SelectedIndices)
                selectedFreqIndicies.Add(index);
            int nSelectedFrequency = selectedFreqIndicies.Count;
            ChartPosition.lastRow = 0;
            // Creating series
            foreach (string chart in listBoxTemplateCharts.Items) // Charts
            {
                // Nodes selection
                List<ISelection> selectedObjects = chartSelection_[chart];
                // Type and direction
                ChartTypes type = chartTypes_[chart];
                ChartDirection direction = chartDirection_[chart];
                SignalUnits units = chartUnits_[chart];
                if (type == ChartTypes.UNKNOWN || direction == ChartDirection.UNKNOWN || units == SignalUnits.UNKNOWN)
                    continue;
                if (selectedObjects.Count == 0)
                {
                    setStatus("Template objects for " + chart + " were not specified");
                    iError = 1;
                    continue;
                }
                // Norm
                double norm = chartNormalization_[chart];
                ChartDirection axis = chartAxis_[chart];
                // Frequency response function: real and imaginary parts
                if (type == ChartTypes.REALFRF || type == ChartTypes.IMAGFRF)
                {
                    List<string> chartNodes = new List<string>();
                    foreach (ISelection item in selectedObjects)
                        chartNodes.Add((string)item.retrieveSelection());
                    foreach (string node in chartNodes) // Node
                    {
                        // Check if there is an appropriate signal
                        if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                        {
                            iError = 2;
                            setStatus("The chosen signals do not contain the node" + node);
                            continue;
                        }
                        ResponseHolder response = project.signals_[node][direction];
                        // Slice data by the selected index
                        double[,] refFullData = response.data[units];
                        double[,] data = new double[nSelectedFrequency, 2];
                        int iSelected;
                        int iType = (int)type - 1;
                        for (int i = 0; i != nSelectedFrequency; ++i)
                        {
                            iSelected = selectedFreqIndicies[i];
                            data[i, 0] = response.frequency[iSelected];
                            data[i, 1] = refFullData[iSelected, iType];
                        }
                        string ptrNode = "т. " + node.Split(selectionDelimiter_)[1];
                        excelResult.addSeries(chart, data, ptrNode);
                    }
                }
                // Modeset
                if (type == ChartTypes.MODESET)
                {
                    int indResonance = (int)textBoxResonanceFrequency.Tag;
                    if (axis == ChartDirection.UNKNOWN || indResonance < 0)
                        continue;
                    // For each line
                    foreach (ISelection item in selectedObjects)
                    {
                        Lines currentLine = (Lines) item;
                        string nameLine = currentLine.lineName_;
                        List<string> lineNodes = (List<string>)currentLine.retrieveSelection();
                        List<double> coordinates = new List<double>();
                        List<double> values = new List<double>();
                        foreach (string node in lineNodes)
                        {
                            // Check if there is an appropriate signal
                            if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                            {
                                iError = 2;
                                setStatus("The chosen signals do not contain the node " + node);
                                continue;
                            }
                            // Retreiving the coordinate along the choosen axis
                            string[] selectionInfo = node.Split(selectionDelimiter_);
                            uint indNode = modelRenderer_.componentSet_.mapNodeNames[selectionInfo[0]][selectionInfo[1]];
                            double[,] componentCoordinates = (double[,])modelRenderer_.componentSet_.nodeCoordinates[selectionInfo[0]];
                            int tInd = (int)axis - 1;
                            coordinates.Add(componentCoordinates[indNode, tInd]);
                            // Retreiving the function value
                            ResponseHolder response = project.signals_[node][direction];
                            double[,] refFullData = response.data[units];
                            values.Add(refFullData[indResonance, 1]); // Imaginary part of the signal
                        }
                        int nNodes = coordinates.Count;
                        double[,] data = new double[nNodes, 2];
                        for (int i = 0; i != nNodes; ++i)
                        {
                            data[i, 0] = coordinates[i] / norm;
                            data[i, 1] = values[i];
                        }
                        excelResult.addSeries(chart, data, nameLine);
                    }
                }
            }
            ChartPosition.lastRow = 0;
            excelResult.open();
            if (iError == 0)
                setStatus("The results were successfully processed");
        }

        private void buttonSelectResonanceFrequency_Click(object sender, EventArgs e)
        {
            if (listBoxFrequencies.SelectedIndices.Count == 0)
                return;
            textBoxResonanceFrequency.Tag = listBoxFrequencies.SelectedIndices[0];
            textBoxResonanceFrequency.Text = listBoxFrequencies.Items[(int)textBoxResonanceFrequency.Tag].ToString();
            selectAllItems(listBoxFrequencies);
        }
    }
}
