using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ResponseAnalyzer
{
    public class ObjectPicker
    {

        public static float DistanceFromPoint(Point mouseLocation, Vector3 testPoint, Matrix4 modelView, Matrix4 projection)
        {

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            Vector3 near = UnProject(viewport, new Vector3(mouseLocation.X, mouseLocation.Y, 0), modelView, projection); // Near plane
            Vector3 far = UnProject(viewport, new Vector3(mouseLocation.X, mouseLocation.Y, 1), modelView, projection);  // Far plane
            Vector3 pt = ClosestPoint(near, far, testPoint); // Finding the nearest point
            return Vector3.Distance(pt, testPoint); 
        }

        private static Vector3 ClosestPoint(Vector3 A, Vector3 B, Vector3 P)
        {
            Vector3 AB = B - A;
            float ab_square = Vector3.Dot(AB, AB);
            Vector3 AP = P - A;
            float ap_dot_ab = Vector3.Dot(AP, AB);
            float t = ap_dot_ab / ab_square;
            Vector3 Q = A + Vector3.Multiply(AB, t);
            return Q;
        }

        private static Vector3 UnProject(int[] viewport, Vector3 screen, Matrix4 modelView, Matrix4 projection)
        {
            Vector4 pos = new Vector4();
            pos.X = (screen.X - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            pos.Y = 1.0f - (screen.Y - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            pos.Z = screen.Z * 2.0f - 1.0f;
            pos.W = 1.0f;
            Vector4 pos2 = Vector4.Transform(pos, Matrix4.Invert(projection) * Matrix4.Invert(modelView));
            Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);
            return pos_out / pos2.W;
        }

    }
}
