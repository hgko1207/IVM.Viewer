using System.Windows.Controls;

namespace IVM.Studio.Views.UserControls
{
    /// <summary>
    /// MainViewerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        public int WindowId { get; set; }

        public ImageViewer()
        {
            InitializeComponent();
        }
    }
}
