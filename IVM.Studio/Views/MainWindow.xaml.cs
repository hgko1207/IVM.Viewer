using DevExpress.Xpf.Core;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

            ShowImageSequeceGroup(15);
        }

        private void ShowImageSequeceGroup(int col)
        {
            this.ImageSequenceGroup.RowDefinitions.Clear();
            this.ImageSequenceGroup.ColumnDefinitions.Clear();

            for (int i = 0; i < 2; ++i)
            {
                this.ImageSequenceGroup.RowDefinitions.Add(new RowDefinition { });
            }

            for (int i = 0; i < col; ++i)
            {
                this.ImageSequenceGroup.ColumnDefinitions.Add(new ColumnDefinition { });
            }

            this.ImageSequenceGroup.Children.Clear();

            for (int i = 0; i < 2; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 65;
                    rect.Height = 65;
                    rect.Fill = new SolidColorBrush(Colors.Black);
                    rect.SetValue(Grid.RowProperty, i);
                    rect.SetValue(Grid.ColumnProperty, j);

                    this.ImageSequenceGroup.Children.Add(rect);
                }
            }
        }
    }
}
