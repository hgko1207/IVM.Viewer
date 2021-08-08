using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IVM.Studio.Views.UserControls
{
    /// <summary>
    /// ThumbnailPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ThumbnailPanel : UserControl
    {
        public ThumbnailPanel()
        {
            InitializeComponent();

            ShowImageSequeceGroup(20);
        }

        private void ShowImageSequeceGroup(int cols)
        {
            this.ImageSequenceGroup.RowDefinitions.Clear();
            this.ImageSequenceGroup.ColumnDefinitions.Clear();

            int rows = 1;

            for (int i = 0; i < rows; ++i)
            {
                this.ImageSequenceGroup.RowDefinitions.Add(new RowDefinition { });
            }

            for (int i = 0; i < cols; ++i)
            {
                this.ImageSequenceGroup.ColumnDefinitions.Add(new ColumnDefinition { });
            }

            this.ImageSequenceGroup.Children.Clear();

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.VerticalAlignment = VerticalAlignment.Center;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanel.SetValue(Grid.RowProperty, i);
                    stackPanel.SetValue(Grid.ColumnProperty, j);

                    Rectangle rect = new Rectangle();
                    rect.Width = 80;
                    rect.Height = 80;
                    rect.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#262626");
                    rect.Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#515151");
                    rect.Margin = new Thickness(5, 0, 0, 0);

                    stackPanel.Children.Add(rect);

                    TextBlock text = new TextBlock();
                    text.Text = "Filename";
                    text.Margin = new Thickness(5, 2, 0, 0);
                    text.HorizontalAlignment = HorizontalAlignment.Center;

                    stackPanel.Children.Add(text);

                    this.ImageSequenceGroup.Children.Add(stackPanel);
                }
            }
        }
    }
}
