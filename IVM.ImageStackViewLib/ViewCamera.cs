using System;
using System.Windows;
using System.Windows.Input;
using GlmNet;
using System.Timers;
using System.Windows.Threading;

namespace ivm
{
    public class ViewCamera
    {
        ImageStackView view = null;

        Point lastbtnPt = new Point(0, 0);
        
        public ViewCamera(ImageStackView v)
        {
            view = v;
        }

        public void Update()
        {
            Rotate(ViewParam.CAMERA_VELOCITY.x, ViewParam.CAMERA_VELOCITY.y);
        }

        public void Reset()
        {
            ViewParam.CAMERA_VELOCITY = new vec2(0, 0);
            ViewParam.CAMERA_ANGLE = new vec2(0, 0);
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
            ViewParam.CAMERA_ANGLE.x += x * 1.0f;
            ViewParam.CAMERA_ANGLE.y += y * 1.0f;

            if (ViewParam.CAMERA_ANGLE.x < -360.0f)
                ViewParam.CAMERA_ANGLE.x += 360.0f;
            if (ViewParam.CAMERA_ANGLE.x > 360.0f)
                ViewParam.CAMERA_ANGLE.x -= 360.0f;

            if (ViewParam.CAMERA_ANGLE.y < -360.0f)
                ViewParam.CAMERA_ANGLE.y += 360.0f;
            if (ViewParam.CAMERA_ANGLE.y > 360.0f)
                ViewParam.CAMERA_ANGLE.y -= 360.0f;

            view.scene.UpdateModelviewMatrix();
        }

        public void Translate(float x, float y)
        {
            float aw = (float)view.ActualWidth;
            float ah = (float)view.ActualHeight;
            float z = -ViewParam.CAMERA_POS.z;

            ViewParam.CAMERA_POS.x += x * (1.0f / aw * z);
            ViewParam.CAMERA_POS.y -= y * (1.0f / ah * z);

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
                ViewParam.CAMERA_SCALE_FACTOR *= 1.1f;
            }
            else
            {
                ViewParam.CAMERA_SCALE_FACTOR /= 1.1f;
            }

            view.scene.UpdateModelviewMatrix();
        }
    }
}
