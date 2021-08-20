using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlmNet;

namespace IVM.Studio.I3D
{
    /// <summary>
    /// ImageStackViewApp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += Control_loaded;
			MouseMove += Control_MouseMove;
        }

        private void Control_loaded(object sender, RoutedEventArgs e)
        {
            vw.Open(@"..\..\..\..\data\t");

            AxisChkBtn.IsChecked = true;
            BoxChkBtn.IsChecked = true;
            GridChkBtn.IsChecked = true;
            ColocalChkBtn.IsChecked = false;

            vw.param.AXIS_TEXT_SIZE = 12;
            vw.param.GRID_TEXT_SIZE = 12;
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(vw);
            Fname.Text = string.Format("{0}", vw.scene.tex3D.GetImagePath());
            RotX.Text = string.Format("{0}", vw.param.CAMERA_ANGLE.x);
            RotY.Text = string.Format("{0}", vw.param.CAMERA_ANGLE.y);

            vw.InvalidateVisual();
        }

        private void IntensityRSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.THRESHOLD_INTENSITY_MIN.x = (float)e.NewValue;
            IntensityR.Text = string.Format("{0:0.00} ", vw.param.THRESHOLD_INTENSITY_MIN.x);
        }

        private void IntensityGSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.THRESHOLD_INTENSITY_MIN.y = (float)e.NewValue;
            IntensityG.Text = string.Format("{0:0.00} ", vw.param.THRESHOLD_INTENSITY_MIN.y);
        }

        private void IntensityBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.THRESHOLD_INTENSITY_MIN.z = (float)e.NewValue;
            IntensityB.Text = string.Format("{0:0.00} ", vw.param.THRESHOLD_INTENSITY_MIN.z);
        }

        private void AxisChkBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_AXIS = true;
        }

        private void AxisChkBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_AXIS = false;
        }

        private void BoxChkBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_BOX = true;
        }

        private void BoxChkBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_BOX = false;
        }

        private void PerPixIterSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.PER_PIXEl_ITERATION = (float)e.NewValue;
            PerPixIter.Text = string.Format("{0}", vw.param.PER_PIXEl_ITERATION);
        }

        private void BoxHeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.BOX_HEIGHT = (float)e.NewValue / 10.0f;
            BoxHeight.Text = string.Format("{0:0.00}", vw.param.BOX_HEIGHT);

            vw.UpdateBoxHeight();
        }

        private void GridChkBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_GRID = true;
        }

        private void GridChkBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.SHOW_GRID = false;
        }

        private void AlphaWeightRSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.ALPHA_WEIGHT.x = (float)e.NewValue;
            vw.param.ALPHA_BLEND.x = vw.param.ALPHA_WEIGHT.x * 0.01f;
            AlphaWeightR.Text = string.Format("{0:0.00 }", vw.param.ALPHA_WEIGHT.x);
        }

        private void AlphaWeightGSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.ALPHA_WEIGHT.y = (float)e.NewValue;
            vw.param.ALPHA_BLEND.y = vw.param.ALPHA_WEIGHT.y * 0.01f;
            AlphaWeightG.Text = string.Format("{0:0.00 }", vw.param.ALPHA_WEIGHT.y);
        }

        private void AlphaWeightBSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.ALPHA_WEIGHT.z = (float)e.NewValue;
            vw.param.ALPHA_BLEND.z = vw.param.ALPHA_WEIGHT.z * 0.01f;
            AlphaWeightB.Text = string.Format("{0:0.00 }", vw.param.ALPHA_WEIGHT.z);
        }

        private void AutoRotXBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.CAMERA_VELOCITY.x = 1;
        }

        private void AutoRotXBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.CAMERA_VELOCITY.x = 0;
        }

        private void AutoRotYBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.CAMERA_VELOCITY.y = 1;
        }

        private void AutoRotYBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.CAMERA_VELOCITY.y = 0;
        }

        private void CameraReset_Click(object sender, RoutedEventArgs e)
        {
            vw.camera.Reset();
        }

        private void ColocalChkBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.IS_COLOCALIZATION = 1;
        }

        private void ColocalChkBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.IS_COLOCALIZATION = 0;
        }

        private void ObliqueDepthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.OBLIQUE_DEPTH = (float)e.NewValue / 100.0f;
            ObliqueDepth.Text = string.Format("{0:0.00 }", vw.param.OBLIQUE_DEPTH);
        }

        private void RenderModeBlend_Click(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.RENDER_MODE = I3DRenderMode.BLEND;
            vw.scene.SetRenderMode(I3DRenderMode.BLEND);
        }

        private void RenderModeAdded_Click(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.RENDER_MODE = I3DRenderMode.ADDED;
            vw.scene.SetRenderMode(I3DRenderMode.ADDED);
        }

        private void RenderModeOblique_Click(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.RENDER_MODE = I3DRenderMode.OBLIQUE;
            vw.scene.SetRenderMode(I3DRenderMode.OBLIQUE);
        }

        private void RenderModeSlice_Click(object sender, RoutedEventArgs e)
        {
            if (vw.scene == null)
                return;

            vw.param.RENDER_MODE = I3DRenderMode.SLICE;
            vw.scene.SetRenderMode(I3DRenderMode.SLICE);
        }

        private void SliceXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.SLICE_DEPTH.x = (float)e.NewValue / 100.0f;
            SliceX.Text = string.Format("{0:0.00 }", vw.param.SLICE_DEPTH.x);
        }

        private void SliceYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.SLICE_DEPTH.y = (float)e.NewValue / 100.0f;
            SliceY.Text = string.Format("{0:0.00 }", vw.param.SLICE_DEPTH.y);
        }

        private void SliceZSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (vw.scene == null)
                return;

            vw.param.SLICE_DEPTH.z = (float)e.NewValue / 100.0f;
            SliceZ.Text = string.Format("{0:0.00 }", vw.param.SLICE_DEPTH.z);
        }

        int testFidx = 0;

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            List<string> flst = new List<string>();
            //flst.Add(@"..\..\..\..\data\01");
            //flst.Add(@"..\..\..\..\data\02");
            //flst.Add(@"..\..\..\..\data\03");
            //flst.Add(@"..\..\..\..\data\04");
            //flst.Add(@"..\..\..\..\data\t");
            //flst.Add(@"..\..\..\..\data\v");
            //flst.Add(@"..\..\..\..\data\4d");
            flst.Add(@"..\..\..\..\data\01");

            vw.Open(flst[testFidx], -1, -1, true);

            testFidx += 1;
            if (testFidx >= flst.Count)
                testFidx = 0;
        }
    }
}
