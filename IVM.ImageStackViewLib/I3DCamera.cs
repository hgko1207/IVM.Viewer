using System;
using System.Windows;
using System.Windows.Input;
using GlmNet;
using System.Timers;
using System.Windows.Threading;

namespace ivm
{
    public class I3DCamera
    {
        ImageStackView view = null;

        Point lastbtnPt = new Point(0, 0);
        
        public I3DCamera(ImageStackView v)
        {
            view = v;
        }

        public void Update()
        {
            Rotate(view.param.CAMERA_VELOCITY.x, view.param.CAMERA_VELOCITY.y);
        }

        public void Reset()
        {
            view.param.CAMERA_VELOCITY = new vec2(0, 0);
            view.param.CAMERA_ANGLE = new vec2(0, 0);
    }

        public void Control_MouseButtonDown(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(view.RenderTarget);

            if (e.LeftButton == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {
                lastbtnPt = pt;
            }

            view.RenderTarget.CaptureMouse();
        }

        public void Control_MouseButtonUp(object sender, MouseEventArgs e)
        {
            view.RenderTarget.ReleaseMouseCapture();
        }

        public void Rotate(float x, float y)
        {
            view.param.CAMERA_ANGLE.x += x * 1.0f;
            view.param.CAMERA_ANGLE.y += y * 1.0f;

            if (view.param.CAMERA_ANGLE.x < -360.0f)
                view.param.CAMERA_ANGLE.x += 360.0f;
            if (view.param.CAMERA_ANGLE.x > 360.0f)
                view.param.CAMERA_ANGLE.x -= 360.0f;

            if (view.param.CAMERA_ANGLE.y < -360.0f)
                view.param.CAMERA_ANGLE.y += 360.0f;
            if (view.param.CAMERA_ANGLE.y > 360.0f)
                view.param.CAMERA_ANGLE.y -= 360.0f;

            view.scene.UpdateModelviewMatrix();
        }

        public void Translate(float x, float y)
        {
            float aw = (float)view.ActualWidth;
            float ah = (float)view.ActualHeight;
            float z = -view.param.CAMERA_POS.z;

            view.param.CAMERA_POS.x += x * (1.0f / aw * z);
            view.param.CAMERA_POS.y -= y * (1.0f / ah * z);

            view.scene.UpdateModelviewMatrix();
        }

        public void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (view.RenderTarget.IsMouseCaptured == false)
                return;

            Point pt = e.GetPosition(view.RenderTarget);
            Point delta = new Point(pt.X - lastbtnPt.X, pt.Y - lastbtnPt.Y);

            lastbtnPt = pt;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Math.Abs(delta.X) > Math.Abs(delta.Y))
                    delta.Y = 0;
                else
                    delta.X = 0;

                Rotate((float)delta.X, (float)delta.Y);
            }
            else
            {
                Translate((float)delta.X, (float)delta.Y);
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
        }
    }
}
