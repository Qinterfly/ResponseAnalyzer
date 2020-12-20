using System;
using System.Drawing;
using OpenTK;

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
            position_[0] += dX;
            position_[1] += dY;
            modelTranslation_ = Matrix4.CreateTranslation(position_);
        }

        public void setRotation(float dRotX, float dRotY)
        {
            modelRotation_ *= Matrix4.CreateRotationX(dRotX)
                            * Matrix4.CreateRotationY(dRotY);
        }

        public void setView(Views view)
        {
            position_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelRotation_ = Matrix4.Identity;
            switch (view)
            {
                case Views.FRONT:
                    setRotation(0.0f, 0.0f);
                    break;
                case Views.BACK:
                    setRotation(MathHelper.TwoPi, 0.0f);
                    break;
                case Views.UP:
                    setRotation(MathHelper.PiOver2, 0.0f);
                    break;
                case Views.DOWN:
                    setRotation(-MathHelper.PiOver2, 0.0f);
                    break;
                case Views.LEFT:
                    setRotation(0.0f, MathHelper.PiOver2);
                    break;
                case Views.RIGHT:
                    setRotation(0.0f, -MathHelper.PiOver2);
                    break;
            }
        }

        public void select(int mouseX, int mouseY, bool isNewSelection)
        {
            Matrix4 model = modelScale_ * modelRotation_ * modelTranslation_;
            float[] vertices = (float[])componentSet_.vertices["W"];
            Vector3 vector = new Vector3(vertices[0], vertices[1], vertices[2]);
            float dist = ObjectPicker.DistanceFromPoint(new Point(mouseX, mouseY), vector, model, projection_);
            Console.WriteLine(dist.ToString());
        }
    }
}
