using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using Prism.Ioc;
using System.Windows;
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
        public WindowInfo WindowInfo { get; set; }

        public int WindowId { get; set; }

        private ContentControl cropBox = null;
        private ContentControl cropCircle = null;
        private ContentControl cropTriangle = null;

        private Polygon polygon = null;

        private readonly IEventAggregator eventAggregator;
        private readonly DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="eventAggregator"></param>
        public ImageViewer(IContainerExtension container, IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.eventAggregator = eventAggregator;
            dataManager = container.Resolve<DataManager>();

            eventAggregator.GetEvent<EnableCropEvent>().Subscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableCropEvent>().Subscribe(DisableCrop);
            eventAggregator.GetEvent<EnableDrawEvent>().Subscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableDrawEvent>().Subscribe(ImageToTop);

            eventAggregator.GetEvent<DrawCropBoxEvent>().Subscribe(DrawCropBox);
            eventAggregator.GetEvent<DrawCropCircleEvent>().Subscribe(DrawCropCircle);
            eventAggregator.GetEvent<DrawCropTriangleEvent>().Subscribe(DrawCropTriangle);

            eventAggregator.GetEvent<GetPositionToCropEvent>().Subscribe(GetPosition);

            eventAggregator.GetEvent<DrawMeasurementEvent>().Subscribe(DrawMeasurement);

            Unloaded += ViewerUnload;
        }

        /// <summary>
        /// Viewer Unload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ViewerUnload(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<EnableCropEvent>().Unsubscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableCropEvent>().Unsubscribe(DisableCrop);
            eventAggregator.GetEvent<EnableDrawEvent>().Unsubscribe(CanvasToTop);
            eventAggregator.GetEvent<DisableDrawEvent>().Unsubscribe(ImageToTop);

            eventAggregator.GetEvent<DrawCropBoxEvent>().Unsubscribe(DrawCropBox);
            eventAggregator.GetEvent<DrawCropCircleEvent>().Unsubscribe(DrawCropCircle);
            eventAggregator.GetEvent<DrawCropTriangleEvent>().Unsubscribe(DrawCropTriangle);

            eventAggregator.GetEvent<GetPositionToCropEvent>().Unsubscribe(GetPosition);

            eventAggregator.GetEvent<DrawMeasurementEvent>().Unsubscribe(DrawMeasurement);
        }

        /// <summary>
        /// Canvas To Top
        /// </summary>
        private void CanvasToTop()
        {
            if (WindowId == dataManager.MainWindowId)
            {
                Panel.SetZIndex(ImageView, 0);
                Panel.SetZIndex(ImageOverlayCanvas, 1);
            }
        }

        /// <summary>
        /// Image To Top
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
            if (WindowId == dataManager.MainWindowId)
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

                if (cropTriangle != null)
                {
                    ImageOverlayCanvas.Children.Remove(cropTriangle);
                    cropTriangle = null;
                }
            }
        }

        /// <summary>
        /// DrawCropBox
        /// </summary>
        /// <param name="param"></param>
        private void DrawCropBox(DrawParam param)
        {
            if (WindowId == dataManager.MainWindowId)
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
        }

        /// <summary>
        /// DrawCropCircle
        /// </summary>
        private void DrawCropCircle(DrawParam param)
        {
            if (WindowId == dataManager.MainWindowId)
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
        }

        /// <summary>
        /// DrawCropTriangle
        /// </summary>
        /// <param name="param"></param>
        private void DrawCropTriangle(DrawParam param)
        {
            if (WindowId == dataManager.MainWindowId)
            {
                if (cropTriangle == null)
                {
                    cropTriangle = new ContentControl { Template = (ControlTemplate)FindResource("DesignerItemTemplate") };
                    polygon = new Polygon()
                    {
                        Stroke = Brushes.White,
                        StrokeThickness = 2,
                    };

                    cropTriangle.Content = polygon;

                    Panel.SetZIndex(cropTriangle, 1);
                    ImageOverlayCanvas.Children.Add(cropTriangle);
                }

                if (polygon != null)
                {
                    double x1 = 0;
                    double y1 = 0;
                    double x2 = x1 + param.Width;
                    double y2 = y1 + param.Height;

                    PointCollection points = new PointCollection
                    {
                        new Point(x1 + (x2 - x1) / 2, y1),
                        new Point(x2, y2),
                        new Point(x1, y2)
                    };
                    polygon.Points = points;
                }

                cropTriangle.Width = param.Width;
                cropTriangle.Height = param.Height;
                Canvas.SetTop(cropTriangle, param.Top);
                Canvas.SetLeft(cropTriangle, param.Left);
            }
        }

        /// <summary>
        /// Draw Measurement
        /// </summary>
        /// <param name="enabled"></param>
        private void DrawMeasurement(bool enabled)
        {
            if (enabled)
                CanvasToTop();
            else
                ImageToTop();
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
            else if (cropTriangle != null)
            {
                param.Left = Canvas.GetLeft(cropTriangle) - leftMargin;
                param.Top = Canvas.GetTop(cropTriangle) - topMargin;
                param.Width = cropTriangle.Width;
                param.Height = cropTriangle.Height;
            }

            param.Routed = true;
        }
    }
}
