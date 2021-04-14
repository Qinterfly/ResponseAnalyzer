﻿using System;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace ResponseAnalyzer
{
    public enum SelectionMode { SINGLE, MULTI }

    public partial class ResponseAnalyzer
    {
        // Select new set of signals
        private void buttonSelectTestLab_Click(object sender, EventArgs e)
        {
            // Check if all the fields are correct
            if (!project.isProjectOpened() || !excelTemplate_.isOpened())
                return;
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            int nSelected = project.selectSignals(modelRenderer_.componentSet_, mode == SelectionMode.MULTI);
            if (nSelected < 1)
            {
                if (nSelected == -1)
                    setStatus("Wrong TestLab selection");
                return;
            }
            retrieveTestLabSelection();
            if (mode == SelectionMode.SINGLE)
            {
                selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged);
                // Resonance frequency
                textBoxResonanceFrequency.Clear();
                textBoxResonanceFrequency.Tag = -1;
            }
            nSelected = listBoxFoundSignals.Items.Count;
            labelSelectionInfo.Text = "Selected signals: " + nSelected.ToString();
            // Update the list of the selected frequencies
            if (mode == SelectionMode.SINGLE)
                listBoxFrequency_SelectedIndexChanged(); 
        }

        // Selecting frequencies
        private void listBoxFrequency_SelectedIndexChanged(object sender = null, EventArgs e = null)
        {
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            List<int> indices = null;
            if (mode == SelectionMode.SINGLE)
                indices = singleFrequencyIndices_;
            else if (mode == SelectionMode.MULTI)
                indices = multiFrequencyIndices_[listBoxFoundSignals.SelectedItem.ToString()];
            if (indices != null)
            {
                indices.Clear();
                foreach (int index in listBoxFrequency.SelectedIndices)
                    indices.Add(index);
            }
        }

        // Selecting signals
        private void listBoxFoundSignals_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex != SelectionMode.MULTI)
                return;
            if (listBoxFoundSignals.SelectedIndex < 0 || listBoxFoundSignals.Items.Count == 0)
                return;
            listBoxFrequency.Items.Clear();
            string label = listBoxFoundSignals.SelectedItem.ToString();
            double[] frequency = multiFrequency_[label];
            // Add frequencies for the specified signal
            foreach (double freq in frequency)
                listBoxFrequency.Items.Add(freq.ToString("G4", CultureInfo.InvariantCulture));
            selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged, multiFrequencyIndices_[label]);
        }

        // Changing selection mode
        private void comboBoxTestlabSelectionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (project == null)
                return;
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            // Specifying the states of the controls
            buttonSelectResonanceFrequency.Enabled = mode != SelectionMode.MULTI;
            if (mode == SelectionMode.SINGLE)
                listBoxFoundSignals.SelectionMode = System.Windows.Forms.SelectionMode.None;
            else if (mode == SelectionMode.MULTI)
            {
                listBoxFoundSignals.SelectionMode = System.Windows.Forms.SelectionMode.One;
                textBoxResonanceFrequency.Text = "";
            }
            // Acquiring the Testlab selection
            retrieveTestLabSelection();
            if (mode == SelectionMode.SINGLE && project.signals_.Count != 0)
            {
                selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged, singleFrequencyIndices_);
                int indResonanceFrequency = (int)textBoxResonanceFrequency.Tag;
                if (indResonanceFrequency > 0)
                    textBoxResonanceFrequency.Text = listBoxFrequency.Items[indResonanceFrequency].ToString();
            }
            int nSelected = listBoxFoundSignals.Items.Count;
            labelSelectionInfo.Text = "Selected signals: " + nSelected.ToString();
        }

        // Select all the frequencies via ctrl+A
        private void listBoxFrequencies_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged);
                listBoxFrequency_SelectedIndexChanged();
            }
        }

        // Select the resulting directory
        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                textBoxDirectoryExcel.Text = openFolderDialog.SelectedPath;
        }

        // Clear Testlab selection
        private void buttonClearTestlabSelection_Click(object sender, EventArgs e)
        {
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            switch (mode)
            {
                case SelectionMode.SINGLE:
                    project.clearSignals();
                    textBoxResonanceFrequency.Text = "";
                    textBoxResonanceFrequency.Tag = -1;
                    break;
                case SelectionMode.MULTI:
                    project.clearAccumulatedSignals();
                    multiFrequency_.Clear();
                    multiFrequencyIndices_.Clear();
                    mapResponses_.Clear();
                    break;
            }
            listBoxFoundSignals.Items.Clear();
            listBoxFrequency.Items.Clear();
            labelSelectionInfo.Text = "Selected signals: ";
        }

        // Select the resonance frequency
        private void buttonSelectResonanceFrequency_Click(object sender, EventArgs e)
        {
            if (listBoxFrequency.SelectedIndices.Count == 0)
                return;
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            if (mode != SelectionMode.SINGLE)
                return;
            textBoxResonanceFrequency.Tag = listBoxFrequency.SelectedIndices[0];
            textBoxResonanceFrequency.Text = listBoxFrequency.Items[(int)textBoxResonanceFrequency.Tag].ToString();
            selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged);
            listBoxFrequency_SelectedIndexChanged();
        }

        // Processing all the data
        private void buttonProcess_Click(object sender = null, EventArgs e = null)
        {
            ExcelObject excelResult = new ExcelObject(excelTemplate_, textBoxDirectoryExcel.Text, textBoxNameExcel.Text);
            // Checking the project, template and selected signals
            if (!project.isProjectOpened() || !excelTemplate_.isOpened())
                return;
            // Resolving dependencies
            resolveDependencies();
            // Retrieve selected frequencies
            int nSingleFrequency = singleFrequencyIndices_.Count;
            ChartPosition.lastRow = 0;
            // Error handling
            errorMessage_ = null;
            int iError = 0;
            // Creating series
            double signData = 1.0;
            if (checkBoxInverseResults.Checked)
                signData = -1.0;
            foreach (string chart in listBoxTemplateCharts.Items)
            {
                // Nodes selection
                List<ISelection> selectedObjects = charts_.selection[chart];
                // Type and direction
                ChartTypes type = charts_.type[chart];
                ChartDirection direction = charts_.direction[chart];
                SignalUnits units = charts_.units[chart];
                // Error handling
                if (type == ChartTypes.UNKNOWN || selectedObjects.Count == 0)
                    continue;
                if (direction == ChartDirection.UNKNOWN)
                { 
                    throwError(ref iError, "The direction for '" + chart + "' is not specified");
                    continue;
                }
                if (units == SignalUnits.UNKNOWN) 
                { 
                    throwError(ref iError, "The units for '" + chart + "' are not specified");
                    continue;
                }
                // Norm
                double norm = charts_.normalization[chart];
                ChartDirection axis = charts_.axis[chart];
                int indX = 0, indY = 1;
                if (charts_.swapAxes[chart]) {
                    indX = 1;
                    indY = 0;
                }
                // Frequency response function: real and imaginary parts
                if ((type == ChartTypes.REAL_FRF || type == ChartTypes.IMAG_FRF) && project.signals_.Count != 0)
                {
                    List<string> chartNodes = new List<string>();
                    foreach (ISelection item in selectedObjects)
                        chartNodes.Add((string)item.retrieveSelection());
                    int iType = (int)type - 1;
                    foreach (string node in chartNodes) // Node
                    {
                        // Check if there is an appropriate signal
                        if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                        {
                            throwError(ref iError, "The chosen signals do not contain the node '" + node + "'");
                            continue;
                        }
                        Response response = project.signals_[node][direction][0];
                        // Slice data by the selected index
                        if (!response.data.ContainsKey(units))
                            continue;
                        double[,] refFullData = response.data[units];
                        double[,] data = new double[nSingleFrequency, 2];
                        int iSelected;
                        for (int i = 0; i != nSingleFrequency; ++i)
                        {
                            iSelected = singleFrequencyIndices_[i];
                            data[i, indX] = response.frequency[iSelected];
                            data[i, indY] = refFullData[iSelected, iType] * signData;
                        }
                        string ptrNode = "т. " + node.Split(selectionDelimiter_)[1];
                        excelResult.addSeries(chart, data, ptrNode);
                    }
                }
                // Modeset
                else if (type == ChartTypes.MODESET && project.signals_.Count != 0)
                {
                    int indResonance = (int)textBoxResonanceFrequency.Tag;
                    if (axis == ChartDirection.UNKNOWN)
                    {
                        throwError(ref iError, "The coordinate axis for '" + chart + "' is not specified");
                        continue;
                    }
                    if (indResonance < 0)
                    {
                        throwError(ref iError, "The resonance frequency is not chosen");
                        continue;
                    }
                    // For each line
                    foreach (ISelection item in selectedObjects)
                    {
                        Lines currentLine = (Lines)item;
                        string nameLine = currentLine.lineName_;
                        List<string> lineNodes = (List<string>)currentLine.retrieveSelection();
                        List<double> coordinates = new List<double>();
                        List<double> values = new List<double>();
                        foreach (string node in lineNodes)
                        {
                            // Check if there is an appropriate signal
                            if (!project.signals_.ContainsKey(node) || !project.signals_[node].ContainsKey(direction))
                            {
                                throwError(ref iError, "The chosen signals do not contain the node '" + node + "'");
                                continue;
                            }
                            // Retreiving the coordinate along the choosen axis
                            string[] selectionInfo = node.Split(selectionDelimiter_);
                            uint indNode = modelRenderer_.componentSet_.mapNodeNames[selectionInfo[0]][selectionInfo[1]];
                            double[,] componentCoordinates = (double[,])modelRenderer_.componentSet_.nodeCoordinates[selectionInfo[0]];
                            int tInd = (int)axis - 1;
                            coordinates.Add(componentCoordinates[indNode, tInd]);
                            // Retreiving the function value
                            Response response = project.signals_[node][direction][0];
                            if (!response.data.ContainsKey(units))
                                continue;
                            double[,] refFullData = response.data[units];
                            values.Add(refFullData[indResonance, 1]); // Imaginary part of the signal
                        }
                        if (values.Count > 0)
                        { 
                            int nNodes = coordinates.Count;
                            double[,] data = new double[nNodes, 2];
                            for (int i = 0; i != nNodes; ++i)
                            {
                                data[i, indX] = coordinates[i] / norm;
                                data[i, indY] = values[i] * signData;
                            }
                            excelResult.addSeries(chart, data, nameLine);
                        }
                    }
                }
                // Multi-FRF
                else if ((type == ChartTypes.MULTI_REAL_FRF || type == ChartTypes.MULTI_IMAG_FRF) && project.multiSignals_.Count != 0)
                {
                    List<string> chartNodes = new List<string>();
                    int iType = 0;
                    if (type == ChartTypes.MULTI_IMAG_FRF)
                        iType = 1;
                    foreach (ISelection item in selectedObjects)
                        chartNodes.Add((string)item.retrieveSelection());
                    foreach (string node in chartNodes) // Node
                    {
                        // Check if there is an appropriate signal
                        if (!project.multiSignals_.ContainsKey(node) || !project.multiSignals_[node].ContainsKey(direction))
                        {
                            throwError(ref iError, "The chosen signals do not contain the node '" + node + "'");
                            continue;
                        }
                        var dirNodeSignals = project.multiSignals_[node][direction];
                        foreach (Response response in dirNodeSignals)
                        {
                            // Slice data by the selected index
                            if (!response.data.ContainsKey(units))
                                continue;
                            double[,] refFullData = response.data[units];
                            List<int> indices = multiFrequencyIndices_[mapResponses_[response.path]];
                            int nIndices = indices.Count;
                            double[,] data = new double[nIndices, 2];
                            int iSelected;
                            for (int i = 0; i != nIndices; ++i)
                            {
                                iSelected = indices[i];
                                data[i, indX] = response.frequency[iSelected];
                                data[i, indY] = refFullData[iSelected, iType] * signData;
                            }
                            // Retrieving force value
                            if (data.GetLength(0) > 0)
                            {
                                string force = "F = " + getForceValue(response.path) + " Н";
                                excelResult.addSeries(chart, data, force);
                            }
                        }
                    }
                }
                // Frequency function
                else if ((type == ChartTypes.REAL_FREQUENCY || type == ChartTypes.IMAG_FREQUENCY) && project.multiSignals_.Count != 0)
                {
                    List<string> chartNodes = new List<string>();
                    foreach (ISelection item in selectedObjects)
                        chartNodes.Add((string)item.retrieveSelection());
                    foreach (string node in chartNodes) // Node
                    {
                        // Check if there is an appropriate signal
                        if (!project.multiSignals_.ContainsKey(node) || !project.multiSignals_[node].ContainsKey(direction))
                        {
                            throwError(ref iError, "The chosen signals do not contain the node '" + node + "'");
                            continue;
                        }
                        var dirNodeSignals = project.multiSignals_[node][direction];
                        double[,] data = new double[dirNodeSignals.Count, 2];
                        int k = 0;
                        foreach (Response response in dirNodeSignals)
                        {
                            if (!response.data.ContainsKey(units))
                                continue;
                            Tuple<double, double> pair = response.evaluateResonanceFrequency(type, units);
                            if (pair != null) { 
                                data[k, indX] = pair.Item1; // Frequency
                                data[k, indY] = pair.Item2; // Amplitude
                                ++k;
                            }
                        }
                        if (data.GetLength(0) > 0)
                        {
                            string ptrNode = "т. " + node.Split(selectionDelimiter_)[1];
                            excelResult.addSeries(chart, data, ptrNode);
                        }
                    }
                }
            }
            ChartPosition.lastRow = 0;
            // Saving and opening the results
            excelResult.save();
            excelResult.open();
            if (iError == 0)
                setStatus("The results were successfully processed");
            else
                showErrors(iError);
        }

        // Retrieve all the selected sets
        public void retrieveTestLabSelection()
        {
            listBoxFoundSignals.Items.Clear();
            listBoxFrequency.Items.Clear();
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            switch (mode)
            {
                case SelectionMode.SINGLE:
                    Response response = null;
                    var signals = project.signals_;
                    foreach (string nodeName in signals.Keys)
                    {
                        var nodeSignal = signals[nodeName];
                        foreach (ChartDirection dir in nodeSignal.Keys)
                        {
                            response = nodeSignal[dir][0];
                            listBoxFoundSignals.Items.Add(response.signalName);
                        }
                    }
                    // If signals have been selected
                    if (response != null) 
                    {
                        foreach (double freq in response.frequency)
                            listBoxFrequency.Items.Add(freq.ToString("G4", CultureInfo.InvariantCulture));
                    }
                    break;
                case SelectionMode.MULTI:
                    multiFrequency_.Clear();
                    multiFrequencyIndices_.Clear();
                    mapResponses_.Clear();
                    var multiSignals = project.multiSignals_;
                    string label;
                    foreach (string nodeName in multiSignals.Keys)
                    {
                        var nodeSignal = multiSignals[nodeName];
                        foreach (ChartDirection dir in nodeSignal.Keys)
                        {
                            var dirNodeSignal = multiSignals[nodeName][dir];
                            foreach (Response multiResponse in dirNodeSignal)
                            {
                                label = multiResponse.signalName + " (" + getForceValue(multiResponse.path) + "H)";
                                // Check if a label is a dubplicate
                                foreach (string item in listBoxFoundSignals.Items)
                                {
                                    if (item.Equals(label))
                                        label += "-Copy";
                                }
                                listBoxFoundSignals.Items.Add(label);
                                multiFrequency_.Add(label, multiResponse.frequency);
                                multiFrequencyIndices_.Add(label, Enumerable.Range(0, multiResponse.frequency.Length).ToList());
                                mapResponses_.Add(multiResponse.path, label);
                            }
                        }
                    }
                    if (listBoxFoundSignals.Items.Count > 0)
                        listBoxFoundSignals.SelectedIndex = listBoxFoundSignals.Items.Count - 1;
                    break;
            }
        }

        // Accumulate errors
        private void throwError(ref int iError, string message)
        {
            ++iError;
            errorMessage_ += "● " + message + "\n";
        }

        // Show all the errors at once
        private void showErrors(int iError)
        {
            MessageBox.Show(errorMessage_, "The number of errors occured: " + iError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        string errorMessage_;
    }
}
