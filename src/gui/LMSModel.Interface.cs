using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
        
        public void setView(Views view)
        {
            location_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelRotation_ = Matrix4.Identity;
            switch (view)
            {
                case Views.FRONT:
                    setRotationXY(0.0f, 0.0f);
                    break;
                case Views.BACK:
                    setRotationAxis(Vector3.UnitX, MathHelper.TwoPi);
                    break;
                case Views.UP:
                    setRotationAxis(Vector3.UnitX, MathHelper.PiOver2);
                    break;
                case Views.DOWN:
                    setRotationAxis(Vector3.UnitX, -MathHelper.PiOver2);
                    break;
                case Views.LEFT:
                    setRotationAxis(Vector3.UnitY, MathHelper.PiOver2);
                    break;
                case Views.RIGHT:
                    setRotationAxis(Vector3.UnitY, -MathHelper.PiOver2);
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

        public void select(int mouseX, int mouseY, bool isNewSelection)
        {
            Matrix4 modelView = modelRotation_ * modelScale_ * modelTranslation_ * view_;
            Point mouseLocation = new Point(mouseX, mouseY);
            float minDist = float.MaxValue;
            float dist = 0.0f;
            Vector3 tempVector = Vector3.Zero;
            string minComponent = string.Empty;
            uint minNode = 0;
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
                    // .. TODO.. 
                    // Check if the node is already selected
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minComponent = component;
                        minNode = (uint) i / 3;
                    }
                }
            }
            if (isNewSelection)
                selection_.Clear();
            List<uint> indSel;
            if (selection_.ContainsKey(minComponent))
            {
                indSel = selection_[minComponent];
            }
            else
            {
                indSel = new List<uint>();
                selection_.Add(minComponent, indSel);
            }
            indSel.Add(minNode);
        }

        // Control
        private OpenTK.GLControl glControl_ = null;
        // Selection
        private Dictionary<string, List<uint>> selection_;
    }
}
