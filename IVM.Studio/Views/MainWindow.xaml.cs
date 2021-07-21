using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;

namespace IVM.Studio.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnTableViewPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        //private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (ZStackSlider.SelectionStart > e.NewValue || ZStackSlider.SelectionEnd < e.NewValue)
        //    {
        //        ZStackSlider.Value = e.OldValue;
        //    }
        //}
    }
}
