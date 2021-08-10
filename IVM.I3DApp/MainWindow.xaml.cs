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
        I3DWcfClient wcfclient;

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
            //vw.Open(@"..\..\..\..\data\t");

            string[] args = Environment.GetCommandLineArgs();

            string title = "";
            string viewtypestr = "";
            int viewtype = 0;

            if (args.Length >= 3)
            {
                title = args[1];
                viewtypestr = args[2];
                if (viewtypestr == "I3D_MAIN_VIEW")
                    viewtype = (int)I3DViewType.MAIN_VIEW;
                else
                    viewtype = (int)I3DViewType.SLICE_VIEW;
            }

            // set window title
            this.Title = title;

            // wcf connect & send loaded message to ivm-studio
            wcfclient.Listen(viewtypestr);
            wcfclient.Connect();
            Task.Run(() => wcfclient.channel.OnWindowLoaded(viewtype));
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            vw.InvalidateVisual();
        }
    }
}
