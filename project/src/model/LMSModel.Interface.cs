﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace ResponseAnalyzer
{

    public partial class LMSModel
    {

        // Check if it's all the data is correct
        public bool isCongruent()
        {
            return componentNames_ != null && glControl_ != null;
        }

        public void setControl(OpenTK.GLControl glControl)
        {
            glControl_ = glControl;
            initializeGL();
        }

        public void setScale(float scale)
        {
            modelScale_ *= Matrix4.CreateScale(scale);
        }

        public void setTranslation(float dX, float dY)
        {
            location_[0] += dX;
            location_[1] += dY;
            modelTranslation_ = Matrix4.CreateTranslation(location_);
        }

        public void setRotationXY(float dRotX, float dRotY)
        {
            modelRotation_ *= Matrix4.CreateRotationX(dRotX)
                            * Matrix4.CreateRotationY(dRotY);
        }

        public void setRotationAxis(Vector3 axis, float dRot)
        {
            modelRotation_ *= Matrix4.CreateFromAxisAngle(axis, dRot);
        } 

        public void setPolygonMode(PolygonMode mode)
        {
            polygonMode_ = mode;
        }
        
        public void setView(Views view)
        {
            location_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelRotation_ = Matrix4.Identity;
            switch (view)
            {
                case Views.FRONT:
                    setTranslation(0.0f, DrawOptions.defaultY);
                    setRotationXY(0.0f, 0.0f);
                    break;
                case Views.BACK:
                    setTranslation(0.0f, DrawOptions.defaultY);
                    setRotationAxis(Vector3.UnitX, MathHelper.TwoPi);
                    break;
                case Views.UP:
                    setRotationAxis(Vector3.UnitX, MathHelper.PiOver2);
                    break;
                case Views.DOWN:
                    setRotationAxis(Vector3.UnitX, -MathHelper.PiOver2);
                    break;
                case Views.LEFT:
                    setTranslation(0.0f, DrawOptions.defaultY);
                    setRotationAxis(Vector3.UnitY, MathHelper.PiOver2);
                    break;
                case Views.RIGHT:
                    setTranslation(0.0f, DrawOptions.defaultY);
                    setRotationAxis(Vector3.UnitY, -MathHelper.PiOver2);
                    break;
                case Views.ISOMETRIC:
                    setTranslation(0.0f, DrawOptions.defaultY);
                    setRotationAxis(isoVector_, isoAngle_);
                    break;
            }
        }

        public List<string> getSelection()
        {
            List<string> selected = new List<string>();
            foreach (string component in selection_.Keys)
            {
                string[] nearestComponentNodes = (string[])componentSet_.nodeNames[component];
                List<uint> selectedNodes = selection_[component];
                foreach (int iNode in selectedNodes)
                {
                    string nearestNode = nearestComponentNodes[iNode];
                    selected.Add(component + ":" + nearestNode);
                }
            }
            return selected;
        }

        public void clearSelection()
        {
            selection_.Clear();
        }

        public void select(int mouseX, int mouseY, bool isNewSelection)
        {
            Matrix4 modelView = modelRotation_ * modelScale_ * modelTranslation_ * view_;
            Point mouseLocation = new Point(mouseX, mouseY);
            float minDist = float.MaxValue;
            float dist = 0.0f;
            Vector3 tempVector = Vector3.Zero;
            string minComponent = string.Empty;
            uint minNode = 0;
            // Finding the nearest model node to the picked one
            foreach (string component in componentNames_)
            {
                float[] vertices = (float[]) componentSet_.vertices[component];
                int lenVertices = vertices.Length;
                for (int i = 0; i != lenVertices; i = i + 3)
                {
                    tempVector.X = vertices[i    ];
                    tempVector.Y = vertices[i + 1];
                    tempVector.Z = vertices[i + 2];
                    dist = ObjectPicker.DistanceFromPoint(mouseLocation, tempVector, modelView, projection_);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minComponent = component;
                        minNode = (uint) i / 3;
                    }
                }
            }
            addToSelection(minComponent, minNode, isNewSelection);
        }

        public void select(string component, string nodeName, bool isNewSelection)
        {
            if (!componentSet_.mapNodeNames.ContainsKey(component)
                || !componentSet_.mapNodeNames[component].ContainsKey(nodeName))
                return;
            uint iSelectedNode = componentSet_.mapNodeNames[component][nodeName];
            addToSelection(component, iSelectedNode, isNewSelection);
        }

        private void addToSelection(string component, uint iNode, bool isNewSelection)
        {
            if (isNewSelection)
                selection_.Clear();
            List<uint> indSel;
            // Adding to the selection set
            if (selection_.ContainsKey(component))
            {
                indSel = selection_[component];
                // Deselect if it has been already selected
                if (indSel.Remove(iNode))
                    return;
            }
            else
            {
                indSel = new List<uint>();
                selection_.Add(component, indSel);
            }
            indSel.Add(iNode);
        }

        public void removeSelection(string component, string node)
        {
            if (selection_.ContainsKey(component) && componentSet_.mapNodeNames[component].ContainsKey(node))
                selection_[component].Remove(componentSet_.mapNodeNames[component][node]);
        }

        // Control
        private OpenTK.GLControl glControl_ = null;
        // Selection
        private Dictionary<string, List<uint>> selection_;
    }
}