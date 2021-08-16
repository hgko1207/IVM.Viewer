using IVM.Studio.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IVM.Studio.I3D
{
    /// <summary>
    /// ImageStackViewApp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public I3DWcfClient wcfclient;
        public int viewtype = -1;

        public String AppTitle;

        public MainWindow()
        {
            InitializeComponent();

            wcfclient = new I3DWcfClient();
            I3DClientService.w = this;

            Loaded += Control_loaded;
			MouseMove += Control_MouseMove;
        }

        private void Control_loaded(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            string title = "";

            if (args.Length >= 2)
            {
                title = args[1];
                if (title == "I3D_MAIN_VIEW")
                    viewtype = (int)I3DViewType.MAIN_VIEW;
                else if(title == "I3D_SLICE_VIEW")
                    viewtype = (int)I3DViewType.SLICE_VIEW;
            }

            // set window title
            this.Title = title;

            // wcf connect & send loaded message to ivm-studio
            if (viewtype != -1)
            {
                wcfclient.Listen(viewtype);
                wcfclient.Connect();
                Task.Run(() => wcfclient.channel.OnWindowLoaded(viewtype));
            }

            // init viewer
            InitViewer(viewtype);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            vw.InvalidateVisual();
        }

        private void InitViewer(int viewtype)
        {
            vw.param.RENDER_MODE = I3DRenderMode.ADDED;
            if (viewtype == (int)I3DViewType.SLICE_VIEW)
                vw.param.RENDER_MODE = I3DRenderMode.OBLIQUE;

            vw.camera.updateFunc = UpdateCamera;
        }

        private void UpdateCamera()
        {
            if (viewtype == -1)
                return;

            wcfclient.channel.OnUpdateCamera(
                viewtype,
                vw.param.CAMERA_POS.x, vw.param.CAMERA_POS.y, vw.param.CAMERA_POS.z,
                vw.param.CAMERA_ANGLE.x, vw.param.CAMERA_ANGLE.y, vw.param.CAMERA_ANGLE.z,
                vw.param.CAMERA_SCALE_FACTOR);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // disable "alt + f4"
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }
    }
}
