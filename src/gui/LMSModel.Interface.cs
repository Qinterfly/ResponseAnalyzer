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
            initGL();
        }

        public void setScale(float scale)
        {
            modelScale_ *= Matrix4.CreateScale(scale, scale, 0.0f);
        }

        public void setTranslation(Vector3 displacement)
        {
            position_ += displacement;
            modelTranslation_ = Matrix4.CreateTranslation(position_);
        }

        public void setRotation(Vector3 diffRotation)
        {
            modelRotation_ *= Matrix4.CreateRotationX(diffRotation[0])
                           * Matrix4.CreateRotationY(diffRotation[1])
                           * Matrix4.CreateRotationZ(diffRotation[2]);
        }

        public void setView(Views view)
        {
            position_ = Vector3.Zero;
            modelTranslation_ = Matrix4.Identity;
            modelScale_ = Matrix4.CreateScale(DrawOptions.defaultScale, DrawOptions.defaultScale, 0.0f);
            modelRotation_ = Matrix4.Identity;
            switch (view)
            {
                case Views.FRONT:
                    setRotation(new Vector3(0.0f, 0.0f, 0.0f));
                    break;
                case Views.BACK:
                    setRotation(new Vector3(MathHelper.TwoPi, 0.0f, 0.0f));
                    break;
                case Views.UP:
                    setRotation(new Vector3(MathHelper.PiOver2, 0.0f, 0.0f));
                    break;
                case Views.DOWN:
                    setRotation(new Vector3(-MathHelper.PiOver2, 0.0f, 0.0f));
                    break;
                case Views.LEFT:
                    setRotation(new Vector3(0.0f, MathHelper.PiOver2, 0.0f));
                    break;
                case Views.RIGHT:
                    setRotation(new Vector3(0.0f, -MathHelper.PiOver2, 0.0f));
                    break;
                case Views.ISOMETRIC:
                    setRotation(new Vector3(MathHelper.PiOver4, MathHelper.PiOver6, -1.0f * MathHelper.PiOver4));
                    break;
            }
        }
    }
}
