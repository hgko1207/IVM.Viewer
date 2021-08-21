using DevExpress.Xpf.Core;
using IVM.Studio.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace IVM.Studio.Views
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadingWindow : Window
    {
        DispatcherTimer timer;
        public bool loading = true;

        public LoadingWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += UpdateTick;
            timer.Start();
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            if (!loading)
            {
                timer.Stop();
                this.Close();
            }
        }
    }
}
