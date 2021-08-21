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

        private void I3DMainView_SelectedItemChanged(object sender, DevExpress.Xpf.Docking.Base.SelectedItemChangedEventArgs e)
        {
            if (EventAggregator == null)
                return;

            if (e.Item != null)
                EventAggregator.GetEvent<I3DViewSelectEvent>().Publish(e.Item.ActualCaption);

            if (e.OldItem != null)
                EventAggregator.GetEvent<I3DViewDeselectEvent>().Publish(e.OldItem.ActualCaption);
        }

        private void I3DSliceView_SelectedItemChanged(object sender, DevExpress.Xpf.Docking.Base.SelectedItemChangedEventArgs e)
        {
            if (EventAggregator == null)
                return;

            if (e.Item != null)
                EventAggregator.GetEvent<I3DViewSelectEvent>().Publish(e.Item.ActualCaption);

            if (e.OldItem != null)
                EventAggregator.GetEvent<I3DViewDeselectEvent>().Publish(e.OldItem.ActualCaption);
        }
        
        private void I3DMainView_LayoutChanged(object sender, System.EventArgs e)
        {
            int a = 1;
        }
    }
}
