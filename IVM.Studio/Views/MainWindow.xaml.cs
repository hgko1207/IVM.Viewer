using DevExpress.Xpf.Core;
using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using System.Windows.Input;

namespace IVM.Studio.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public IEventAggregator EventAggregator;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnTableViewPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void i3dmv_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (EventAggregator == null)
                return;

            EventAggregator.GetEvent<I3DMainViewVisibleChangedEvent>().Publish((bool)e.NewValue);
        }

        private void i3dsv_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (EventAggregator == null)
                return;

            EventAggregator.GetEvent<I3DSliceViewVisibleChangedEvent>().Publish((bool)e.NewValue);
        }
    }
}
