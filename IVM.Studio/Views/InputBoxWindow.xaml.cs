using DevExpress.Xpf.Core;
using IVM.Studio.Models.Events;
using Prism.Events;
using System.Windows;

namespace IVM.Studio.Views
{
    /// <summary>
    /// InputBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputBoxWindow : ThemedWindow
    {
        private TextAnnotationDialogParam param;

        private IEventAggregator eventAggregator;

        public InputBoxWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;

            eventAggregator.GetEvent<TextAnnotationDialogEvent>().Subscribe(InputDialog);
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            if (InputText.Text == "")
                return;

            eventAggregator.GetEvent<TextAnnotationEvent>().Publish(new TextAnnotationParam(param.X, param.Y, InputText.Text));

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InputDialog(TextAnnotationDialogParam param)
        {
            this.param = param;
        }
    }
}
