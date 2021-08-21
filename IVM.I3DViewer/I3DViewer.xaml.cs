using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.WPF;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using System.Drawing;
using System.Drawing.Imaging;
using FFMediaToolkit.Graphics;
using System.Windows.Threading;
using System.Timers;

namespace IVM.Studio.I3D
{
    /// <summary>
    /// StackView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class I3DViewer : UserControl
    {
        public I3DScene scene = null;
        public I3DCamera camera = null;
        public I3DParam param = null;
        public OpenGL gl = null;

        MediaOutput mediaFile = null;

        DispatcherTimer timer; // 업데이트 타이머

        public delegate void LoadedDelegate();
        public LoadedDelegate LoadedFunc = null;

        public I3DViewer()
        {
            InitializeComponent();

            Loaded += Control_loaded;

            int sec = 10000000; // 1-second

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(sec / 30);
            timer.Tick += UpdateTick;
            timer.Start();
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            UpdateRecordVideo();
        }

        private void Control_loaded(object sender, RoutedEventArgs e)
        {
            RenderTarget.MouseDown += camera.Control_MouseButtonDown;
            RenderTarget.MouseUp += camera.Control_MouseButtonUp;
            RenderTarget.MouseMove += camera.Control_MouseMove;
            RenderTarget.MouseWheel += camera.Control_MouseWheel;
        }

        private void OpenGLControl_Initialized(object sender, OpenGLRoutedEventArgs args)
        {
            camera = new I3DCamera(this);
            param = new I3DParam();

            // init Scene
            gl = args.OpenGL;
            scene = new I3DScene(this);
            scene.Init();
            scene.UpdateModelviewMatrix();
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            scene.UpdateProjectionMatrix();
        }

        private void OpenGLControl_Draw(object sender, OpenGLRoutedEventArgs args)
        {
            scene.Render(); // scene render
        }

        public async void Open(string imgPath, int lower = -1, int upper = -1, bool reverse = false)
        {
            this.Background = new SolidColorBrush(System.Windows.Media.Color.FromScRgb(param.BG_COLOR.x, param.BG_COLOR.y, param.BG_COLOR.z, 1));

            if (scene.tex3D.Loading)
                return;

            Invalid.Visibility = Visibility.Hidden;

            Loading.Visibility = Visibility.Visible;
            bool res = await scene.Open(imgPath, lower, upper, reverse);
            Loading.Visibility = Visibility.Hidden;

            if (res == false)
                Invalid.Visibility = Visibility.Visible;

            if (LoadedFunc != null)
                LoadedFunc();
        }

        public void UpdateBoxHeight()
        {
            scene.UpdateMesh();
        }

        public bool CaptureScreen(string path)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(this);

            BitmapEncoder enc;

            if (path.ToLower().Contains(".png"))
                enc = new PngBitmapEncoder();
            else if (path.ToLower().Contains(".jpg"))
                enc = new JpegBitmapEncoder();
            else if (path.ToLower().Contains(".tif"))
                enc = new TiffBitmapEncoder();
            else if (path.ToLower().Contains(".bmp"))
                enc = new BmpBitmapEncoder();
            else
                return false;

            enc.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream s = File.Create(path))
                enc.Save(s);

            return true;
        }

        public bool StartRecordVideo(string path)
        {
            FFmpegLoader.FFmpegPath = @".\ffmpeg";

            VideoEncoderSettings settings = new VideoEncoderSettings((int)this.ActualWidth, (int)this.ActualHeight, 30, VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 17;

            mediaFile = MediaBuilder.CreateContainer(path).WithVideo(settings).Create();

            return true;
        }

        public void UpdateRecordVideo()
        {
            if (mediaFile == null)
                return;

            RenderTargetBitmap tgtbmp = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            tgtbmp.Render(this);

            var membmp = new Bitmap(tgtbmp.PixelWidth, tgtbmp.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            
            // copy rendertarget to memory buffer
            var bitLock = membmp.LockBits(new Rectangle(System.Drawing.Point.Empty, membmp.Size), ImageLockMode.WriteOnly, membmp.PixelFormat);
            tgtbmp.CopyPixels(Int32Rect.Empty, bitLock.Scan0, bitLock.Stride * bitLock.Height, bitLock.Stride);

            var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgra32, membmp.Size);
            mediaFile.Video.AddFrame(bitmapData);

            membmp.UnlockBits(bitLock);
        }

        public void StopRecordVideo()
        {
            mediaFile.Dispose();
            mediaFile = null;
        }
    }
}
