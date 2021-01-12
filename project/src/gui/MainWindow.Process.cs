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
            int nSelected = project.selectSignals(modelRenderer_.componentSet_, false);
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
                    response = project.signals_[nodeName][dir][0];
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

        // Select all the frequencies via ctrl+A
        private void listBoxFrequencies_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                selectAllItems(listBoxFrequencies);
        }

        // Select the resulting directory
        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            DialogResult dialogResult = openFolderDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                textBoxDirectoryExcel.Text = openFolderDialog.SelectedPath;
        }

        // Select the resonance frequency
        private void buttonSelectResonanceFrequency_Click(object sender, EventArgs e)
        {
            if (listBoxFrequencies.SelectedIndices.Count == 0)
                return;
            textBoxResonanceFrequency.Tag = listBoxFrequencies.SelectedIndices[0];
            textBoxResonanceFrequency.Text = listBoxFrequencies.Items[(int)textBoxResonanceFrequency.Tag].ToString();
            selectAllItems(listBoxFrequencies);
        }

        // Processing all the data
        private void buttonProcess_Click(object sender = null, EventArgs e = null)
        {
            ExcelObject excelResult = new ExcelObject(excelTemplate_, textBoxDirectoryExcel.Text, textBoxNameExcel.Text);
            // Checking the project, template and selected signals
            if (!project.isProjectOpened() || !excelTemplate_.isOpened() || listBoxFoundSignals.Items.Count == 0)
                return;
            // Resolving dependencies
            resolveDependencies();
            // Retrieve selected frequencies
            List<int> selectedFreqIndicies = new List<int>();
            foreach (int index in listBoxFrequencies.SelectedIndices)
                selectedFreqIndicies.Add(index);
            int nSelectedFrequency = selectedFreqIndicies.Count;
            ChartPosition.lastRow = 0;
            // Error handling
            errorMessage_ = null;
            int iError = 0;
            // Creating series
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
                            throwError(ref iError, "The chosen signals do not contain the node '" + node + "'");
                            continue;
                        }
                        ResponseHolder response = project.signals_[node][direction][0];
                        // Slice data by the selected index
                        double[,] refFullData = response.data[units];
                        double[,] data = new double[nSelectedFrequency, 2];
                        int iSelected;
                        int iType = (int)type - 1;
                        for (int i = 0; i != nSelectedFrequency; ++i)
                        {
                            iSelected = selectedFreqIndicies[i];
                            data[i, indX] = response.frequency[iSelected];
                            data[i, indY] = refFullData[iSelected, iType];
                        }
                        string ptrNode = "т. " + node.Split(selectionDelimiter_)[1];
                        excelResult.addSeries(chart, data, ptrNode);
                    }
                }
                // Modeset
                if (type == ChartTypes.MODESET)
                {
                    int indResonance = (int) textBoxResonanceFrequency.Tag;
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
                            ResponseHolder response = project.signals_[node][direction][0];
                            double[,] refFullData = response.data[units];
                            values.Add(refFullData[indResonance, 1]); // Imaginary part of the signal
                        }
                        int nNodes = coordinates.Count;
                        double[,] data = new double[nNodes, 2];
                        for (int i = 0; i != nNodes; ++i)
                        {
                            data[i, indX] = coordinates[i] / norm;
                            data[i, indY] = values[i];
                        }
                        excelResult.addSeries(chart, data, nameLine);
                    }
                }
            }
            ChartPosition.lastRow = 0;
            excelResult.open();
            if (iError == 0)
                setStatus("The results were successfully processed");
            else
                showErrors(iError);
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
