using System;
using System.IO;
using System.Drawing;
using OpenTK;
using OpenTK.Input;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace ResponseAnalyzer
{
    public partial class ResponseAnalyzer : Form
    {
        public ResponseAnalyzer()
        {
            InitializeComponent();
            comboBoxSelection.SelectedIndex = 0;
            modelRender_ = new LMSRender();
        }

        // Opening project
        private void buttonOpenProject_Click(object sender, EventArgs e)
        {
            clearStatus();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LMS Test.Lab 15A Project (*.lms)|*.lms";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                project = new LMSProject(filePath);
                clearProjectControls();
                if (!project.isProjectOpened()) {
                    textBoxProjectPath.Text = filePath;
                    setStatus("The project was successfully opened");
                }
                modelRender_.setGeometry(project.geometry_);
            }
            else if (dialogResult != DialogResult.Cancel)
            {
                setStatus("An error occured while choosing a project file");
                return;
            }
            else
            {
                return;
            }
        }

        // Set the status label
        private void setStatus(string status)
        {
            statusStripLabel.Text = status;
        }

        // Clear the status label
        private void clearStatus()
        {
            statusStripLabel.Text = null;
        }

        // Clear project controls
        private void clearProjectControls()
        {
            textBoxProjectPath.Clear();
        }

        private void buttonAddSelection_Click(object sender, EventArgs e)
        {

        }

        private void glWindow_Load(object sender, EventArgs e)
        {
            glWindow.MakeCurrent();
            modelRender_.setControl(glWindow);
            // -- Debug only --
            testRender();
            // ----------------
        }

        private void glWindow_Paint(object sender, PaintEventArgs e)
        {
            modelRender_.draw();
        }

        private void glWindow_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float scale = 1.0f + MouseWeights.scaling * e.Delta;
            modelRender_.setScale(scale);
            modelRender_.draw();
        }
        private void glWindow_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (e.Button == MouseButtons.Middle) {
                baseMouseState_ = Mouse.GetState();
                if (keyboard.IsKeyDown(Key.ControlLeft))
                    isTranslation_ = true;
                else
                    isRotation_ = true;
            }
        }

        private void glWindow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();
            if (isTranslation_)
            {
                float dX = (mouse.X - baseMouseState_.X) / (float) glWindow.Width * MouseWeights.translation; 
                float dY = (mouse.Y - baseMouseState_.Y) / (float) glWindow.Height * MouseWeights.translation;
                Vector3 displacement = new Vector3(dX, -dY, 0.0f);
                modelRender_.setTranslation(displacement);
                modelRender_.draw();
                baseMouseState_ = mouse;
            }
            if (isRotation_)
            {
                float dRotX = (mouse.Y - baseMouseState_.Y);
                float dRotY = (mouse.X - baseMouseState_.X) ;
                dRotX = MathHelper.DegreesToRadians(dRotX) * MouseWeights.rotation;
                dRotY = MathHelper.DegreesToRadians(dRotY) * MouseWeights.rotation;
                Vector3 diffRot = new Vector3(dRotX, dRotY, 0.0f);
                modelRender_.setRotation(diffRot);
                modelRender_.draw();
                baseMouseState_ = mouse;
            }
        }
        private void glWindow_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var keyboard = Keyboard.GetState();
            if (e.Button == MouseButtons.Middle) {
                isTranslation_ = false;
                isRotation_ = false;
            }
            if (keyboard.IsKeyUp(Key.ControlLeft)) 
                isTranslation_ = false;
        }

        private void testRender()
        {
            //string path = Path.GetFullPath(@"..\..\examples\Plate.lms");
            //string path = Path.GetFullPath(@"..\..\examples\Rib.lms");
            //string path = Path.GetFullPath(@"..\..\examples\Airplane.lms");
            string path = Path.GetFullPath(@"..\..\examples\Yak130.lms");
            LMSProject project = new LMSProject(path);
            modelRender_.setGeometry(project.geometry_);
            modelRender_.setView(Views.UP);
        }

        private void glWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.F))
            {
                modelRender_.setView(Views.FRONT);
                modelRender_.draw();
            }
        }
        private void glWindow_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glWindow.Width, glWindow.Height);
        }

        static class MouseWeights
        {
            public const float scaling = 0.001f;
            public const float translation = 1.2f;
            public const float rotation = 1.0f;
        }
    }
}
