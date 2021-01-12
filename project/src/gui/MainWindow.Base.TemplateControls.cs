using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer
    {
        // Add a template object to the tree
        private void buttonAddTemplateObject_Click(object sender = null, EventArgs e = null)
        {
            // Check the type of the current chart
            ChartTypes type = (ChartTypes)comboBoxTemplateType.SelectedIndex;
            if (type == ChartTypes.UNKNOWN)
                return;
            var selection = modelRenderer_.getSelection();
            int nSelection = selection.Count;
            if (nSelection == 0)
                return;
            // Adding the object to the tree
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            if (charts_.dependency[chart] != null) // Cannot add items to the dependent chart
                return;
            if (isLineType(type))
            {
                TreeNode line = treeTemplateObjects.Nodes.Add("Line" + indLine_.ToString());
                foreach (string item in selection)
                    line.Nodes.Add(item);
                ++indLine_;
                Lines selLines = new Lines();
                selLines.lineName_ = line.Text;
                selLines.nodeNames_ = selection;
                charts_.selection[chart].Add(selLines);
            }
            if (isNodeType(type))
            {
                foreach (string node in selection)
                {
                    treeTemplateObjects.Nodes.Add(node);
                    Nodes selNode = new Nodes();
                    selNode.nodeName_ = node;
                    charts_.selection[chart].Add(selNode);
                }
            }
        }

        // Remove a template object from the tree
        private void buttonRemoveTemplateObject_Click(object sender = null, EventArgs e = null)
        {
            if (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Parent != null)
                return;
            int iSelected = treeTemplateObjects.SelectedNode.Index;
            string chart = listBoxTemplateCharts.SelectedItem.ToString();
            charts_.selection[chart].RemoveAt(iSelected);
            treeTemplateObjects.Nodes.RemoveAt(iSelected);
        }

        // Add/remove template objects to/from the selected line
        private void buttonEditTemplateSelection_Click(object sender = null, EventArgs e = null)
        {
            // If not a line was selected
            if (!isEditSelection_ && (treeTemplateObjects.SelectedNode == null || treeTemplateObjects.SelectedNode.Nodes.Count == 0))
                return;
            if (isEditSelection_)
            {
                TreeNode line = treeTemplateObjects.Nodes[iSelectedSet_];
                line.Nodes.Clear();
                var selection = modelRenderer_.getSelection();
                string chart = listBoxTemplateCharts.SelectedItem.ToString();
                List<ISelection> objects = charts_.selection[chart];
                Lines selLine = null;
                int iSelectedLine = 0;
                bool isFound = false;
                foreach (ISelection item in objects)
                {
                    selLine = (Lines)item;
                    if (selLine.lineName_ == line.Text)
                    {
                        isFound = true;
                        break;
                    }
                    ++iSelectedLine;
                }
                if (isFound == false)
                    return;
                if (selection.Count < 1)
                {
                    charts_.selection[chart].RemoveAt(iSelectedLine);
                    line.Remove();
                }
                else
                {
                    selLine.nodeNames_.Clear();
                    foreach (string item in selection)
                    {
                        line.Nodes.Add(item);
                        selLine.nodeNames_.Add(item);
                    }
                }
                iSelectedSet_ = -1;
            }
            else
            {
                iSelectedSet_ = treeTemplateObjects.SelectedNode.Index;
                modelRenderer_.clearSelection();
                // Select all the nodes in the set
                foreach (TreeNode item in treeTemplateObjects.SelectedNode.Nodes)
                {
                    string[] selectionInfo = item.Text.Split(selectionDelimiter_);
                    modelRenderer_.select(selectionInfo[0], selectionInfo[1], false);
                }
                modelRenderer_.draw();
            }
            // Invert the states of the controls
            buttonAddTemplateObject.Enabled = isEditSelection_;
            buttonRemoveTemplateObject.Enabled = isEditSelection_;
            listBoxTemplateCharts.Enabled = isEditSelection_;
            isEditSelection_ = !isEditSelection_;
        }

        // Copy template objects from the selected chart
        private void buttonCopyTemplateObjects_Click(object sender = null, EventArgs e = null)
        {
            if (listBoxTemplateCharts.SelectedIndex >= 0)
            {
                buttonCopyTemplateObjects.Enabled = false;
                buttonCopyTemplateObjects.Tag = listBoxTemplateCharts.SelectedItem.ToString();
            }
        }
		
		// Modify template objects
        private void treeTemplateObjects_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                // Delete a selected template object
                case Keys.D:
                    buttonRemoveTemplateObject_Click();
                    break;
            }
        }
    }
}
