using IVM.Studio.Models.Events;
using Prism.Events;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IVM.Studio.Views.UserControls
{
    /// <summary>
    /// MainViewerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewer : UserControl
    {
        public int WindowId { get; set; }

        private ContentControl cropBox = null;
        private ContentControl cropCircle = null;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="eventAggregator"></param>
        public ImageViewer(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<EnableCropEvent>().Subscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableCropEvent>().Subscribe(DisableCrop);
            eventAggregator.GetEvent<EnableDrawEvent>().Subscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableDrawEvent>().Subscribe(ImageToTop);
            eventAggregator.GetEvent<DrawCropBoxEvent>().Subscribe(DrawCropBox);
            eventAggregator.GetEvent<DrawCropCircleEvent>().Subscribe(DrawCropCircle);
            eventAggregator.GetEvent<GetPositionToCropEvent>().Subscribe(GetPosition);
        }

        /// <summary>
        /// CanvasToTop
        /// </summary>
        private void CanvasToTop()
        {
            Panel.SetZIndex(ImageView, 0);
            Panel.SetZIndex(ImageOverlayCanvas, 1);
        }

        /// <summary>
        /// ImageToTop
        /// </summary>
        private void ImageToTop()
        {
            Panel.SetZIndex(ImageView, 1);
            Panel.SetZIndex(ImageOverlayCanvas, 0);
        }

        /// <summary>
        /// DisableCrop
        /// </summary>
        private void DisableCrop()
        {
            ImageToTop();

            if (cropBox != null)
            {
                ImageOverlayCanvas.Children.Remove(cropBox);
                cropBox = null;
            }

            if (cropCircle != null)
            {
                ImageOverlayCanvas.Children.Remove(cropCircle);
                cropCircle = null;
            }
        }

        /// <summary>
        /// DrawCropBox
        /// </summary>
        /// <param name="param"></param>
        private void DrawCropBox(DrawParam param)
        {
            if (cropBox == null)
            {
                cropBox = new ContentControl { Template = (ControlTemplate)FindResource("DesignerItemTemplate") };

                Panel.SetZIndex(cropBox, 1);
                ImageOverlayCanvas.Children.Add(cropBox);
            }

            cropBox.Width = param.Width;
            cropBox.Height = param.Height;
            Canvas.SetTop(cropBox, param.Top);
            Canvas.SetLeft(cropBox, param.Left);
        }

        /// <summary>
        /// DrawCropCircle
        /// </summary>
        private void DrawCropCircle(DrawParam param)
        {
            if (cropCircle == null)
            {
                cropCircle = new ContentControl { Template = (ControlTemplate)FindResource("DesignerItemTemplate") };
                Ellipse ellipse = new Ellipse()
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };

                cropCircle.Content = ellipse;

                Panel.SetZIndex(cropCircle, 1);
                ImageOverlayCanvas.Children.Add(cropCircle);
            }

            cropCircle.Width = param.Width;
            cropCircle.Height = param.Height;
            Canvas.SetTop(cropCircle, param.Top);
            Canvas.SetLeft(cropCircle, param.Left);
        }

        /// <summary>
        /// GetPosition
        /// </summary>
        /// <param name="param"></param>
        private void GetPosition(GetPositionToCropParam param)
        {
            double leftMargin = (ImageOverlayCanvas.ActualWidth - ImageView.ActualWidth) / 2;
            double topMargin = (ImageOverlayCanvas.ActualHeight - ImageView.ActualHeight) / 2;

            param.HorizontalOffset = DisplayingScrollViewer.HorizontalOffset - leftMargin;
            param.VerticalOffset = DisplayingScrollViewer.VerticalOffset - topMargin;
            param.ViewportWidth = DisplayingScrollViewer.ViewportWidth;
            param.ViewportHeight = DisplayingScrollViewer.ViewportHeight;

            if (param.Routed)
                return;

            if (cropBox != null)
            {
                param.Left = Canvas.GetLeft(cropBox) - leftMargin;
                param.Top = Canvas.GetTop(cropBox) - topMargin;
                param.Width = cropBox.Width;
                param.Height = cropBox.Height;
            }
            else if (cropCircle != null)
            {
                param.Left = Canvas.GetLeft(cropCircle) - leftMargin;
                param.Top = Canvas.GetTop(cropCircle) - topMargin;
                param.Width = cropCircle.Width;
                param.Height = cropCircle.Height;
            }

            param.Routed = true;
        }
    }
}
