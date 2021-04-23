using System;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace ResponseAnalyzer
{
    using ResponseArray = Dictionary<string, Dictionary<ChartDirection, List<Response>>>;
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
                textBoxResonanceFrequency.Text = listBoxFrequency.Items[indexSingleResonanceFrequency_].ToString();
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
            textBoxResonanceFrequency.Text = listBoxFrequency.Items[multiResonanceFrequencyIndices_[label]].ToString();
        }

        // Changing selection mode
        private void comboBoxTestlabSelectionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (project == null)
                return;
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            // Specifying the states of the controls
            if (mode == SelectionMode.SINGLE)
                listBoxFoundSignals.SelectionMode = System.Windows.Forms.SelectionMode.None;
            else if (mode == SelectionMode.MULTI)
            {
                listBoxFoundSignals.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
            // Acquiring the Testlab selection
            retrieveTestLabSelection();
            textBoxResonanceFrequency.Text = "";
            if (mode == SelectionMode.SINGLE && project.signals_.Count != 0)
            {
                selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged, singleFrequencyIndices_);
                textBoxResonanceFrequency.Text = listBoxFrequency.Items[indexSingleResonanceFrequency_].ToString();
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
            textBoxResonanceFrequency.Text = "";
            switch (mode)
            {
                case SelectionMode.SINGLE:
                    project.clearSignals();
                    indexSingleResonanceFrequency_ = 0;
                    break;
                case SelectionMode.MULTI:
                    project.clearMultiSignals();
                    multiFrequency_.Clear();
                    multiFrequencyIndices_.Clear();
                    multiResonanceFrequencyIndices_.Clear();
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
            if (mode == SelectionMode.SINGLE)
            {
                indexSingleResonanceFrequency_ = listBoxFrequency.SelectedIndices[0];
                textBoxResonanceFrequency.Text = listBoxFrequency.Items[indexSingleResonanceFrequency_].ToString();
            }
            else
            {
                int iSelectedSignal = listBoxFoundSignals.SelectedIndices[0];
                string label = listBoxFoundSignals.Items[iSelectedSignal].ToString();
                int indexMultiResonanceFrequency = listBoxFrequency.SelectedIndices[0];
                multiResonanceFrequencyIndices_[label] = indexMultiResonanceFrequency;
                textBoxResonanceFrequency.Text = listBoxFrequency.Items[indexMultiResonanceFrequency].ToString();
            }
            selectItems(listBoxFrequency, listBoxFrequency_SelectedIndexChanged);
            listBoxFrequency_SelectedIndexChanged();
        }

        // Processing all the data
        private void buttonProcess_Click(object sender = null, EventArgs e = null)
        {
            if (String.IsNullOrEmpty(textBoxNameExcel.Text))
            {
                MessageBox.Show("Filename to save is not specified", "Empty filename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (String.IsNullOrEmpty(textBoxDirectoryExcel.Text))
            {
                MessageBox.Show("Directory to save is not specified", "Empty directory name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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
                        if (!response.data.ContainsKey(units) || response.data[units] == null)
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
                    if (axis == ChartDirection.UNKNOWN)
                    {
                        throwError(ref iError, "The coordinate axis for '" + chart + "' is not specified");
                        continue;
                    }
                    if (indexSingleResonanceFrequency_ < 0)
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
                        double resonanceFrequency = -1.0;
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
                            // Retrieving the function value
                            Response response = project.signals_[node][direction][0];
                            if (!response.data.ContainsKey(units) || response.data[units] == null)
                                continue;
                            double[,] refFullData = response.data[units];
                            values.Add(refFullData[indexSingleResonanceFrequency_, 1]); // Imaginary part of the signal
                            if (resonanceFrequency < 0)
                                resonanceFrequency = response.frequency[indexSingleResonanceFrequency_];
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
                            string frequencyInfo = null;
                            if (resonanceFrequency > 0 )
                                frequencyInfo = $" - {resonanceFrequency.ToString("F3", CultureInfo.InvariantCulture)} Гц";
                            excelResult.addSeries(chart, data, nameLine, frequencyInfo);
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
                            if (!response.data.ContainsKey(units) || response.data[units] == null)
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
                                string force = $"𝐹 = {getForceValue(response.path)} Н";
                                string info = $"[{node}]";
                                excelResult.addSeries(chart, data, force, info);
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
                            if (!response.data.ContainsKey(units) || response.data[units] == null)
                                continue;
                            string label = mapResponses_[response.path];
                            int indexMultiResonance = multiResonanceFrequencyIndices_[label];
                            double resonanceFrequency = multiFrequency_[label][indexMultiResonance];
                            double[,] responseData = response.data[units];
                            double realPart = responseData[indexMultiResonance, 0];
                            double imagPart = responseData[indexMultiResonance, 1];
                            data[k, indX] = resonanceFrequency;                                          
                            data[k, indY] = Math.Sqrt(Math.Pow(realPart, 2.0) + Math.Pow(imagPart, 2.0)); 
                            ++k;
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
            const string forcePrefix = "Force ";
            listBoxFoundSignals.Items.Clear();
            listBoxFrequency.Items.Clear();
            SelectionMode mode = (SelectionMode)comboBoxTestlabSelectionMode.SelectedIndex;
            switch (mode)
            {
                case SelectionMode.SINGLE:
                    Response response = retrieveSingleSelection(project.signals_);
                    // If signals have been selected
                    if (response != null) 
                    {
                        foreach (double freq in response.frequency)
                            listBoxFrequency.Items.Add(freq.ToString("G4", CultureInfo.InvariantCulture));
                        textBoxResonanceFrequency.Text = listBoxFrequency.Items[indexSingleResonanceFrequency_].ToString();
                    }
                    retrieveSingleSelection(project.forces_, forcePrefix);
                    break;
                case SelectionMode.MULTI:
                    multiFrequency_.Clear();
                    multiFrequencyIndices_.Clear();
                    mapResponses_.Clear();
                    multiResonanceFrequencyIndices_.Clear();
                    retrieveMultiSelection(project.multiSignals_); 
                    retrieveMultiSelection(project.multiForces_, forcePrefix); 
                    if (listBoxFoundSignals.Items.Count > 0)
                        listBoxFoundSignals.SelectedIndex = listBoxFoundSignals.Items.Count - 1;
                    break;
            }
        }

        private Response retrieveSingleSelection(ResponseArray signals, string prefixName = "")
        {
            Response response = null;
            foreach (string nodeName in signals.Keys)
            {
                var nodeSignal = signals[nodeName];
                foreach (ChartDirection dir in nodeSignal.Keys)
                {
                    response = nodeSignal[dir][0];
                    listBoxFoundSignals.Items.Add(prefixName + response.signalName);
                }
            }
            if (response != null) {
                double resonanceFrequency = getFrequencyValue(response.path);
                indexSingleResonanceFrequency_ = findIndexClosest(resonanceFrequency, response.frequency);
            }
            return response;
        }

        private void retrieveMultiSelection(ResponseArray multiSignals, string prefixName = "")
        {
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
                        // Retrieve resonance frequency
                        double resonanceFrequency = getFrequencyValue(multiResponse.path);
                        int indexMultiResonance = findIndexClosest(resonanceFrequency, multiResponse.frequency);
                        multiResonanceFrequencyIndices_.Add(label, indexMultiResonance);
                        mapResponses_.Add(multiResponse.path, label);
                    }
                }
            }
        }

        private int findIndexClosest(double findValue, double[] vector)
        {
            int indClosest = 0;
            int lenVector = vector.Length;
            for (int i = 0; i != lenVector; ++i)
            {
                if (vector[i] >= findValue)
                {
                    double previousDistance = Math.Abs(vector[i - 1] - findValue);
                    double currentDistance  = Math.Abs(vector[i    ] - findValue);
                    if (previousDistance < currentDistance)
                        indClosest = i - 1;
                    else
                        indClosest = i;
                    break;
                }
            }
            return indClosest;
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
