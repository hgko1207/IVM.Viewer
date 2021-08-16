using System;
using System.Windows;
using System.Windows.Input;
using GlmNet;
using System.Timers;
using System.Windows.Threading;
using SharpGL.SceneGraph;

namespace IVM.Studio.I3D
{
    public class I3DCamera
    {
        I3DViewer view = null;

        // for translate
        Point? lastTrnPt = null;

        // for rotate
        Vertex? lastRotVec = null;

        public delegate void UpdateDelegate();
        public UpdateDelegate updateFunc = null;

        public bool thisFrameChanged = false;
        Timer timer;

        public I3DCamera(I3DViewer v)
        {
            view = v;

            timer = new Timer();
            timer.Interval = 1000 / 30; // 30 FPS
            timer.Elapsed += new ElapsedEventHandler(UpdateTick);
            timer.Start();
        }

        private void UpdateTick(object sender, ElapsedEventArgs e)
        {
            //Rotate(view.param.CAMERA_VELOCITY.x, view.param.CAMERA_VELOCITY.y);

            if (thisFrameChanged)
            {
                if (updateFunc != null)
                    updateFunc();
         
                thisFrameChanged = false;
            }
        }

        public void Reset()
        {
            view.param.CAMERA_VELOCITY = new vec2(0, 0);
            view.param.CAMERA_ANGLE = new vec3(0, 0, 0);
        }

        private Vertex MapToSphere(float x, float y)
        {
            // hyperboloid mapping taken from https://www.opengl.org/wiki/Object_Mouse_Trackball

            float adjustWidth = 2.0f / (float)view.ActualWidth;
            float adjustHeight = 2.0f / (float)view.ActualHeight;

            float pX = x * adjustWidth - 1.0f;
            float pY = y * adjustHeight - 1.0f;

            Vertex P = new Vertex(pX, -pY, 0);

            //sphere radius
            const float radius = .5f;
            const float radius_squared = radius * radius;

            float XY_squared = P.X * P.X + P.Y * P.Y;

            if (XY_squared <= .5f * radius_squared)
                P.Z = (float)Math.Sqrt(radius_squared - XY_squared);  // Pythagore
            else
                P.Z = (float)(0.5f * (radius_squared)) / (float)Math.Sqrt(XY_squared);  // hyperboloid

            //Console.WriteLine("MapToSphere: {0:0.00} {1:0.00} {2:0.00}", P.X, P.Y, P.Z);
            return P;
        }

        private float[] CalculateQuaternion(Vertex sv, Vertex cv)
        {
            // Compute the cross product of the begin and end vectors.
            Vertex cross = cv.VectorProduct(sv);

            // Is the perpendicular length essentially non-zero?
            if (cross.Magnitude() > 1.0e-5)
            {
                // The quaternion is the transform.
                return new float[] { cross.X, cross.Y, cross.Z, sv.ScalarProduct(sv) };
            }
            else
            {
                // Begin and end coincide, return identity.
                return new float[] { 0, 0, 0, 0 };
            }
        }

        private Matrix Matrix3fSetRotationFromQuat4f(float[] q1)
        {
            float n, s;
            float xs, ys, zs;
            float wx, wy, wz;
            float xx, xy, xz;
            float yy, yz, zz;
            n = (q1[0] * q1[0]) + (q1[1] * q1[1]) + (q1[2] * q1[2]) + (q1[3] * q1[3]);
            s = (n > 0.0f) ? (2.0f / n) : 0.0f;

            xs = q1[0] * s; ys = q1[1] * s; zs = q1[2] * s;
            wx = q1[3] * xs; wy = q1[3] * ys; wz = q1[3] * zs;
            xx = q1[0] * xs; xy = q1[0] * ys; xz = q1[0] * zs;
            yy = q1[1] * ys; yz = q1[1] * zs; zz = q1[2] * zs;

            Matrix matrix = new Matrix(3, 3);

            matrix[0, 0] = 1.0f - (yy + zz); matrix[1, 0] = xy - wz; matrix[2, 0] = xz + wy;
            matrix[0, 1] = xy + wz; matrix[1, 1] = 1.0f - (xx + zz); matrix[2, 1] = yz - wx;
            matrix[0, 2] = xz - wy; matrix[1, 2] = yz + wx; matrix[2, 2] = 1.0f - (xx + yy);

            return matrix;
        }

        public mat4 Matrix4fSetRotationFromMatrix3f(Matrix matrix)
        {
            Matrix t = new Matrix(Matrix.Identity(4));

            float scale = t.TempSVD();
            t.FromOtherMatrix(matrix, 3, 3);
            t.Multiply(scale, 3, 3);

            double[,] a = t.AsArray;

            vec4[] v = new vec4[4];
            v[0] = new vec4((float)a[0, 0], (float)a[1, 0], (float)a[2, 0] , (float)a[3, 0]);
            v[1] = new vec4((float)a[0, 1], (float)a[1, 1], (float)a[2, 1] , (float)a[3, 1]);
            v[2] = new vec4((float)a[0, 2], (float)a[1, 2], (float)a[2, 2] , (float)a[3, 2]);
            v[3] = new vec4((float)a[0, 3], (float)a[1, 3], (float)a[2, 3] , (float)a[3, 3]);

            mat4 m = new mat4(v);
            return m;
        }

        private vec3 EulerFromMatrix3(Matrix m)
        {
            vec3 e = new vec3();

            double[,] a = m.AsArray;

            // Pitch
            e.x = (float)Math.Asin(-a[2, 1]);

            if (Math.Cos(e.x) > 0.0001f) // not at poles
            {
                e.y = (float)Math.Atan2(a[2, 0], a[2, 2]); // Yaw
                e.z = (float)Math.Atan2(a[0, 1], a[1, 1]); // Roll
            }
            else
            {
                e.y = 0; // Yaw
                e.z = (float)Math.Atan2(-a[1, 0], a[0, 0]); // Roll
            }

            //Console.WriteLine(" Euler: {0:0.00} {1:0.00} {2:0.00}", e.x, e.y, e.z);

            return e;
        }

        public Matrix Matrix3FromEuler(vec3 e)
        {
            Matrix m = new Matrix(Matrix.Identity(3));

            double[,] a = m.AsArray;

            float cosY = (float)Math.Cos(e.y); // Yaw
            float sinY = (float)Math.Sin(e.y);

            float cosP = (float)Math.Cos(e.x); // Pitch
            float sinP = (float)Math.Sin(e.x);

            float cosR = (float)Math.Cos(e.z); // Roll
            float sinR = (float)Math.Sin(e.z);

            a[0, 0] = cosY * cosR + sinY * sinP * sinR;
            a[1, 0] = cosR * sinY * sinP - sinR * cosY;
            a[2, 0] = cosP * sinY;

            a[0, 1] = cosP * sinR;
            a[1, 1] = cosR * cosP;
            a[2, 1] = -sinP;

            a[0, 2] = sinR * cosY * sinP - sinY * cosR;
            a[1, 2] = sinY * sinR + cosR * cosY * sinP;
            a[2, 2] = cosP * cosY;

            m = new Matrix(a);
            return m;
        }

        public void Control_MouseButtonDown(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(view.RenderTarget);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                lastRotVec = MapToSphere((float)pt.X, (float)pt.Y);

                view.RenderTarget.CaptureMouse();
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                lastTrnPt = pt;

                view.RenderTarget.CaptureMouse();
            }
        }

        public void Control_MouseButtonUp(object sender, MouseEventArgs e)
        {
            lastTrnPt = null;
            lastRotVec = null;

            view.RenderTarget.ReleaseMouseCapture();
        }

        public void Rotate(float x, float y)
        {
            Vertex curRotVec = MapToSphere(x, y);

            if (lastRotVec != null)
            {
                // todo need solid tuple types.
                // Calculate the quaternion.
                float[] quaternion = CalculateQuaternion(lastRotVec.Value, curRotVec);
                //Console.WriteLine("q: {0:0.00} {1:0.00} {2:0.00} {3:0.00}",
                //    quaternion[0], quaternion[1], quaternion[2], quaternion[3]);

                Matrix rotLast = Matrix3FromEuler(view.param.CAMERA_ANGLE);

                // Set Our Final Transform's Rotation From This One
                Matrix rotThis = Matrix3fSetRotationFromQuat4f(quaternion);
                rotThis = rotThis * rotLast;

                view.param.CAMERA_ANGLE = EulerFromMatrix3(rotThis);

                view.scene.UpdateModelviewMatrix();

                thisFrameChanged = true;
            }

            lastRotVec = curRotVec;
        }

        public void Translate(float x, float y)
        {
            if (lastTrnPt != null)
            {
                Point delta = new Point(x - lastTrnPt.Value.X, y - lastTrnPt.Value.Y);
                //Console.WriteLine("{0:0.00} {1:0.00}", delta.X, delta.Y);

                float aw = (float)view.ActualWidth;
                float ah = (float)view.ActualHeight;
                float z = -view.param.CAMERA_POS.z;

                view.param.CAMERA_POS.x += (float)delta.X * (1.0f / aw * z);
                view.param.CAMERA_POS.y -= (float)delta.Y * (1.0f / ah * z);

                view.scene.UpdateModelviewMatrix();

                thisFrameChanged = true;
            }

            lastTrnPt = new Point(x, y);
        }

        public void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (view.RenderTarget.IsMouseCaptured == false)
                return;

            Point pt = e.GetPosition(view.RenderTarget);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Rotate((float)pt.X, (float)pt.Y);
            }
            
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Translate((float)pt.X, (float)pt.Y);
            }
        }

        public void Control_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                view.param.CAMERA_SCALE_FACTOR *= 1.1f;
            }
            else
            {
                view.param.CAMERA_SCALE_FACTOR /= 1.1f;
            }

            view.scene.UpdateModelviewMatrix();

            thisFrameChanged = true;
        }
    }
}
