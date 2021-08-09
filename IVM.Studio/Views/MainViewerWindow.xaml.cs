using DevExpress.Xpf.Core;
using IVM.Studio.Models;

namespace IVM.Studio.Views
{
    /// <summary>
    /// MainViewerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainViewerWindow : ThemedWindow
    {
        public WindowInfo WindowInfo { get; set; }

        public int WindowId { get; set; }

        public MainViewerWindow(WindowInfo windowInfo)
        {
            InitializeComponent();

            WindowInfo = windowInfo;
            WindowId = windowInfo.Seq;

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
        }
    }
}
