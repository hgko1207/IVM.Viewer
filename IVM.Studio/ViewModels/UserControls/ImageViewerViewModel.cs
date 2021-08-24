using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/**
 * @Class Name : ImageViewerWindowViewModel.cs
 * @Description : 이미지 뷰어 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.05     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.05
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class ImageViewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ImageViewer>
    {
        private ImageSource displayingImage;
        public ImageSource DisplayingImage
        {
            get => displayingImage;
            set => SetProperty(ref displayingImage, value);
        }

        /// <summary>현재 이미지의 표시 가로 길이입니다. <see cref="double.NaN"/>을 지정하면 현재 뷰포트에 맞춥니다.</summary>
        private double displayingImageWidth;
        public double DisplayingImageWidth
        {
            get => displayingImageWidth;
            set => SetProperty(ref displayingImageWidth, value);
        }

        public ICommand MouseWheelCommand { get; private set; }
        public ICommand SizeChangedCommand { get; private set; }
        public ICommand ImageMouseDownCommand { get; private set; }
        public ICommand ImageMouseMoveCommand { get; private set; }
        public ICommand ImageMouseUpCommand { get; private set; }

        public ICommand ViewPortMouseDownCommand { get; private set; }
        public ICommand ViewPortMouseMoveCommand { get; private set; }
        public ICommand ViewPortMouseUpCommand { get; private set; }

        public ICommand ScrollViewerSizeChangedCommand { get; private set; }
        public ICommand ScrollViewerScrollChangedCommand { get; private set; }

        private Bitmap originalImage;
        private Bitmap flippedOriginalImage;
        private Bitmap annotationImage;
        private Bitmap displayingImageGDI;

        private int displayWidth;
        private int originalImageWidth;
        private int originalImageHeight;

        /// <summary>뷰에서 표시 될 현재 이미지의 줌 배율입니다. 단위는 퍼센트입니다.</summary>
        /// <remarks>이미지의 표시 배율을 뷰모델에서 조정할 경우 <see cref="DisplayingImageWidth"/>를 사용합니다.</remarks>
        private int currentZoomRatio;

        private int[] CurrentTranslationByChannel => OrderByColor().Select(s => (int)s.Color).ToArray();
        private bool[] CurrentVisibilityByChannel => OrderByColor().Select(s => s.Visible).ToArray();
        private float[][] CurrentColorMatrix => imageService.GenerateColorMatrix(
                    startLevelByChannel: OrderByColor().Select(s => s.ColorLevelLowerValue).ToArray(),
                    endLevelByChannel: OrderByColor().Select(s => s.ColorLevelUpperValue).ToArray(),
                    brightnessByChannel: OrderByColor().Select(s => s.Brightness / 50).ToArray(),
                    contrastByChannel: OrderByColor().Select(s => s.Contrast / 50).ToArray(),
                    translationByChannel: CurrentTranslationByChannel,
                    visibilityByChannel: CurrentVisibilityByChannel
                );

        private ImageViewer view;

        private int sequense;
        private int fovSizeX;
        private int fovSizeY;

        private bool horizontalReflect;
        private bool verticalReflect;
        private int currentRotate;

        private FileInfo fileInfo;
        private FileInfo fileToDisplay;

        private readonly ImageService imageService;

        private readonly DataManager dataManager;
        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap { get; }
        private readonly AnnotationInfo annotationInfo;

        private System.Windows.Point? imagePreviousPoint;
        private System.Windows.Point? viewPortPreviousPoint;

        private System.Windows.Shapes.Rectangle drawRectangle;
        private System.Windows.Shapes.Ellipse drawEllipse;
        private System.Windows.Shapes.Polygon drawTriangle;
        private System.Windows.Shapes.Line drawLine;

        private System.Windows.Shapes.Line drawMeasurementLine;

        private List<Bitmap> annotationBitmapList { get; set; }
        private List<Bitmap> tempBitmapList { get; set; }

        private bool measurementEnabled;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageViewerViewModel(IContainerExtension container) : base(container)
        {
            MouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(MouseWheel);
            SizeChangedCommand = new DelegateCommand<SizeChangedEventArgs>(SizeChanged);

            ImageMouseDownCommand = new DelegateCommand<MouseButtonEventArgs>(ImageMouseDown);
            ImageMouseMoveCommand = new DelegateCommand<MouseEventArgs>(ImageMouseMove);
            ImageMouseUpCommand = new DelegateCommand<MouseButtonEventArgs>(ImageMouseUp);

            ViewPortMouseDownCommand = new DelegateCommand<MouseButtonEventArgs>(ViewPortMouseDown);
            ViewPortMouseMoveCommand = new DelegateCommand<MouseEventArgs>(ViewPortMouseMove);
            ViewPortMouseUpCommand = new DelegateCommand<MouseButtonEventArgs>(ViewPortMouseUp);

            ScrollViewerSizeChangedCommand = new DelegateCommand(SizeChanged);
            ScrollViewerScrollChangedCommand = new DelegateCommand(SizeChanged);

            EventAggregator.GetEvent<DisplayImageEvent>().Subscribe(DisplayImageWithMetadata, ThreadOption.UIThread);
            EventAggregator.GetEvent<MainViewerUnloadEvent>().Subscribe(MainViewerUnload);

            currentZoomRatio = 100;
            DisplayingImageWidth = double.NaN;
            currentRotate = 0;

            imageService = Container.Resolve<ImageService>();
            dataManager = Container.Resolve<DataManager>();

            colorChannelInfoMap = dataManager.ColorChannelInfoMap;
            annotationInfo = dataManager.AnnotationInfo;

            annotationBitmapList = new List<Bitmap>();
            tempBitmapList = new List<Bitmap>();
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ImageViewer view)
        {
            this.view = view;

            EventAggregator.GetEvent<RefreshImageEvent>().Subscribe(DisplayImage);
            EventAggregator.GetEvent<TextAnnotationEvent>().Subscribe(DrawAnnotationText, ThreadOption.UIThread);
            EventAggregator.GetEvent<DrawClearEvent>().Subscribe(DrawClear, ThreadOption.UIThread);
            EventAggregator.GetEvent<DrawUndoEvent>().Subscribe(DrawUndo, ThreadOption.UIThread);
            EventAggregator.GetEvent<DrawRedoEvent>().Subscribe(DrawRedo, ThreadOption.UIThread);

            EventAggregator.GetEvent<RotationEvent>().Subscribe(Rotation);
            EventAggregator.GetEvent<ReflectEvent>().Subscribe(Reflect);
            EventAggregator.GetEvent<RotationResetEvent>().Subscribe(RotationReset);

            EventAggregator.GetEvent<ZoomRatioControlEvent>().Subscribe(ZoomRatioControl);

            EventAggregator.GetEvent<ExportCropEvent>().Subscribe(ExportCrop);
            EventAggregator.GetEvent<ExportAllCropEvent>().Subscribe(ExportAllCrop);
            EventAggregator.GetEvent<ExportDrawEvent>().Subscribe(ExportDraw);
            EventAggregator.GetEvent<ExportDrawAllEvent>().Subscribe(ExportDrawAll);

            EventAggregator.GetEvent<DrawMeasurementEvent>().Subscribe(DrawMeasurement);

            // 디스플레이
            DisplayImage(dataManager.MainWindowId);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ImageViewer view)
        {
            EventAggregator.GetEvent<RefreshImageEvent>().Unsubscribe(DisplayImage);
            EventAggregator.GetEvent<TextAnnotationEvent>().Unsubscribe(DrawAnnotationText);
            EventAggregator.GetEvent<DrawClearEvent>().Unsubscribe(DrawClear);
            EventAggregator.GetEvent<DrawUndoEvent>().Subscribe(DrawUndo);
            EventAggregator.GetEvent<DrawRedoEvent>().Subscribe(DrawRedo);

            EventAggregator.GetEvent<RotationEvent>().Unsubscribe(Rotation);
            EventAggregator.GetEvent<ReflectEvent>().Unsubscribe(Reflect);
            EventAggregator.GetEvent<RotationResetEvent>().Unsubscribe(RotationReset);

            EventAggregator.GetEvent<ZoomRatioControlEvent>().Unsubscribe(ZoomRatioControl);
            EventAggregator.GetEvent<ExportCropEvent>().Unsubscribe(ExportCrop);

            EventAggregator.GetEvent<DrawMeasurementEvent>().Unsubscribe(DrawMeasurement);

            this.view = null;
        }

        /// <summary>
        /// MainViewer Unload
        /// </summary>
        /// <param name="windowId"></param>
        private void MainViewerUnload(int windowId)
        {
            if (windowId == dataManager.MainWindowId)
            {
                EventAggregator.GetEvent<DisplayImageEvent>().Unsubscribe(DisplayImageWithMetadata);
                EventAggregator.GetEvent<MainViewerUnloadEvent>().Unsubscribe(MainViewerUnload);
            }
        }

        /// <summary>
        /// Colormap 정렬
        /// </summary>
        /// <returns></returns>
        private List<ColorChannelModel> OrderByColor()
        {
            return colorChannelInfoMap.Values.OrderBy(c => c.InitColor).ToList();
        }

        /// <summary>
        /// DisplayImage
        /// </summary>
        /// <param name="param"></param>
        private void DisplayImageWithMetadata(DisplayParam param)
        {
            if (param.Metadata != null)
            {
                fovSizeX = param.Metadata.FovX;
                fovSizeY = param.Metadata.FovY;
                sequense = param.Metadata.Sequence.Sequence;
            }
            else
            {
                fovSizeX = 0;
                fovSizeY = 0;
            }

            if (fileInfo == null)
            {
                fileInfo = param.FileInfo;
            } 
            else
            {
                if (view.WindowId == dataManager.MainWindowId)
                    fileInfo = param.FileInfo;
            }

            // 레지스트레이션 체크
            FileInfo registrationFile = new FileInfo(Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "_Reg" + fileInfo.Extension));

            if (registrationFile.Exists)
                fileToDisplay = registrationFile;
            else
                fileToDisplay = fileInfo;

            // 어노테이션 초기화
            annotationImage?.Dispose();
            annotationImage = null;

            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                if (param.SlideChanged)
                    annotationBitmapList.Clear();

                // 디스플레이
                DisplayImage(dataManager.MainWindowId);

                // 슬라이드쇼
                Container.Resolve<SlideShowService>().ContinueSlideShow();
            }
        }

        /// <summary>
        /// 지정한 색상값에 따라 이미지의 색상을 투영한 후, 어노테이션 이미지를 붙여 화면에 표시하고, 히스토그램을 생성하는 내부 메서드
        /// </summary>
        private async void DisplayImage(int id)
        {
            if (id == 0 || fileToDisplay == null || view.WindowId != dataManager.MainWindowId)
                return;

            // 이미지 표시는 히스토그램 생성 등으로 인해 오래 걸리므로 백그라운드에서 처리
            await Task.Run(() =>
            {
                originalImage?.Dispose();
                originalImage = imageService.LoadImage(fileToDisplay.FullName);
                originalImageWidth = originalImage.Width;
                originalImageHeight = originalImage.Height;

                using (Bitmap bitmap = imageService.LoadImage(fileToDisplay.FullName))
                {
                    // 주 이미지 변경
                    {
                        List<ColorMap?> colormaps = colorChannelInfoMap.Values.Select<ColorChannelModel, ColorMap?>(c =>
                        {
                            if (c.Visible && c.ColorMapEnabled) return c.ColorMap;
                            else return null;
                        }).ToList();

                        using (Bitmap img1 = imageService.TranslateColor(bitmap, CurrentColorMatrix))
                        using (Bitmap img2 = imageService.ApplyColorMaps(img1, colormaps))
                        {
                            DisplayAnnotatedImage(img2);
                            DisplayHistogram(img2);
                        }
                    }
                }

                // 채널별 이미지 변경
                {
                    foreach (ChannelType type in Enum.GetValues(typeof(ChannelType)))
                    {
                        if (type == ChannelType.ALL)
                            continue;

                        ColorChannelModel colorChannelModel = colorChannelInfoMap[type];

                        if (colorChannelModel.Display)
                        {
                            bool[] visibilityByChannel = new bool[4] { false, false, false, false };
                            visibilityByChannel[(int)type] = true;

                            float[][] colorMatrix = imageService.GenerateColorMatrix(
                                startLevelByChannel: OrderByColor().Select(s => s.ColorLevelLowerValue).ToArray(),
                                endLevelByChannel: OrderByColor().Select(s => s.ColorLevelUpperValue).ToArray(),
                                brightnessByChannel: OrderByColor().Select(s => s.Brightness / 50).ToArray(),
                                contrastByChannel: OrderByColor().Select(s => s.Contrast / 50).ToArray(),
                                translationByChannel: CurrentTranslationByChannel,
                                visibilityByChannel: visibilityByChannel
                            );

                            using (Bitmap originalBitmap = imageService.LoadImage(fileToDisplay.FullName))
                            {
                                if (colorChannelModel.ColorMapEnabled)
                                {
                                    using (Bitmap img1 = imageService.TranslateColor(originalBitmap, colorMatrix))
                                    using (Bitmap img2 = imageService.ApplyColorMapGDI(img1, CurrentTranslationByChannel[(int)type], colorChannelModel.ColorMap))
                                    {
                                        BitmapSource img = imageService.ConvertGDIBitmapToWPF(img2);
                                        Container.Resolve<WindowByChannelService>().DisplayImage((int)type, img);
                                        using (Bitmap hist = imageService.CreateHistogram(img2, CurrentTranslationByChannel, new bool[4] { true, true, true, false }))
                                        {
                                            colorChannelModel.HistogramImage = imageService.ConvertGDIBitmapToWPF(hist);
                                            EventAggregator.GetEvent<RefreshChHistogramEvent>().Publish(type);
                                        }
                                    }
                                }
                                else
                                {
                                    using (Bitmap img = imageService.TranslateColor(originalBitmap, colorMatrix))
                                    {
                                        BitmapSource imgwpf = imageService.ConvertGDIBitmapToWPF(img);
                                        Container.Resolve<WindowByChannelService>().DisplayImage((int)type, imgwpf);
                                        using (Bitmap hist = imageService.CreateHistogram(img, CurrentTranslationByChannel, visibilityByChannel))
                                        {
                                            colorChannelModel.HistogramImage = imageService.ConvertGDIBitmapToWPF(hist);
                                            EventAggregator.GetEvent<RefreshChHistogramEvent>().Publish(type);
                                        }
                                    }
                                }
                            }

                            Thread.Sleep(500);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// AnnotatedImage 생성
        /// </summary>
        /// <param name="image"></param>
        private void DisplayAnnotatedImage(Bitmap image)
        {
            using (Bitmap workingImage = new Bitmap(image))
            {
                // 반전 및 회전
                imageService.ReflectAndRotate(workingImage, horizontalReflect, verticalReflect, currentRotate);
                
                flippedOriginalImage?.Dispose();
                flippedOriginalImage = new Bitmap(workingImage);

                // 한번 뒤집기 후에는 반드시 비트맵을 다시 생성해줘야 함.
                // 90도, 270도 플립시 이미지의 가로 길이보다 Y좌표가 큰 영역에 쓰지 못하는 문제가 있음. (원인불명)
                using (Bitmap bitmap = new Bitmap(workingImage))
                {
                    // 어노테이션
                    if (annotationImage != null)
                    {
                        using (Bitmap annotationImg = new Bitmap(annotationImage))
                        {
                            imageService.ReflectAndRotate(annotationImg, horizontalReflect, verticalReflect, currentRotate);
                            imageService.DrawImageOnImage(bitmap, annotationImg);
                        }
                    }

                    int scaleBarSize = annotationInfo.ScaleBarSize;

                    // 스케일 바
                    if (annotationInfo.ScaleBarEnabled && fovSizeX > 0 && fovSizeY > 0 && scaleBarSize > 0 && scaleBarSize < fovSizeX && scaleBarSize < fovSizeY)
                        imageService.DrawScaleBar(bitmap, fovSizeX, fovSizeY, scaleBarSize, annotationInfo.ScaleBarThickness, 30,
                            annotationInfo.XAxisEnabled, annotationInfo.YAxisEnabled, annotationInfo.ScaleBarPosition, annotationInfo.ScaleBarLabel, annotationInfo.TextFontSize, annotationInfo.TextColor);

                    // TimeStack
                    if (annotationInfo.TimeStampEnabled)
                        imageService.DrawTimeStampLabel(bitmap, annotationInfo.TimeStampText, annotationInfo.TimeStampPosition, annotationInfo.TextFontSize, annotationInfo.TextColor, 30);

                    // ZStackLabel
                    if (annotationInfo.ZStackLabelEnabled)
                        imageService.DrawZStackLabel(bitmap, annotationInfo.ZStackLabelText, annotationInfo.ZStackLabelPosition, annotationInfo.TextFontSize, annotationInfo.TextColor, 30);

                    displayingImageGDI?.Dispose();
                    displayingImageGDI = new Bitmap(bitmap);
                    DisplayingImage = imageService.ConvertGDIBitmapToWPF(bitmap);
                    DisplayingImageWidth = bitmap.Width * (currentZoomRatio / 100d);

                    displayWidth = bitmap.Width;

                    annotationBitmapList.Add(new Bitmap(bitmap));
                }
            }
        }

        /// <summary>
        /// 히스토그램 생성
        /// </summary>
        /// <param name="imgage"></param>
        private void DisplayHistogram(Bitmap imgage)
        {
            using (Bitmap hist = imageService.CreateHistogram(imgage, CurrentTranslationByChannel, CurrentVisibilityByChannel))
            {
                dataManager.HistogramImage = imageService.ConvertGDIBitmapToWPF(hist);
                EventAggregator.GetEvent<RefreshMainHistogramEvent>().Publish();
            }
        }

        /// <summary>
        /// Text 그리기
        /// </summary>
        /// <param name="param"></param>
        private void DrawAnnotationText(TextAnnotationParam param)
        {
            if (param.Text != null && annotationImage != null && view != null && view.WindowId == dataManager.MainWindowId)
            {
                imageService.DrawText(annotationImage, displayingImageGDI,
                    param.X, param.Y, annotationInfo.TextFontSize, annotationInfo.TextColor, param.Text,
                    horizontalReflect, verticalReflect, currentRotate
                );
                DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);
            }
        }

        /// <summary>
        /// 마우스 휠 동작 이벤트
        /// </summary>
        private void MouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta is int delta && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (delta > 0)
                    ZoomIn();
                else
                    ZoomOut();
            }
        }

        /// <summary>
        /// 이미지 사이즈 변경 시
        /// </summary>
        /// <param name="e"></param>
        private void SizeChanged(SizeChangedEventArgs e)
        {
            double? width = e.NewSize.Width;
            if (!width.HasValue)
                return;

            currentZoomRatio = (int)Math.Round(width.Value / displayWidth * 100);

            SizeChanged();
        }

        /// <summary>
        /// Image MouseDown
        /// </summary>
        /// <param name="e"></param>
        private void ImageMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                imagePreviousPoint = e.GetPosition(view.ImageView);

            if (e.ChangedButton == MouseButton.Middle)
                DisplayingImageWidth = double.NaN;
        }

        /// <summary>
        /// Image MouseMove
        /// </summary>
        /// <param name="e"></param>
        private void ImageMouseMove(MouseEventArgs e)
        {
            if ((annotationInfo.PenEnabled || annotationInfo.EraserEnabled) && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currntPoint = e.GetPosition(view.ImageView);

                if (annotationImage == null)
                    annotationImage = imageService.MakeEmptyImage(originalImageWidth, originalImageHeight);

                if (annotationInfo.PenEnabled)
                {
                    if (imagePreviousPoint != null)
                    {
                        imageService.DrawPen(
                            annotationImage: annotationImage, displayImage: displayingImageGDI,
                            x1: (int)Math.Round(imagePreviousPoint.Value.X / currentZoomRatio * 100), y1: (int)Math.Round(imagePreviousPoint.Value.Y / currentZoomRatio * 100),
                            x2: (int)Math.Round(currntPoint.X / currentZoomRatio * 100), y2: (int)Math.Round(currntPoint.Y / currentZoomRatio * 100),
                            thickness: annotationInfo.PenThickness, color: annotationInfo.PenColor,
                            horizontalReflect: horizontalReflect, verticalReflect: verticalReflect, rotate: currentRotate
                        );
                    }
                    imagePreviousPoint = new System.Windows.Point(currntPoint.X, currntPoint.Y);
                }
                else if (annotationInfo.EraserEnabled)
                {
                    imageService.DrawEraser(
                        annotationImage: annotationImage, displayImage: displayingImageGDI, originalImage: flippedOriginalImage,
                        colorMatrix: CurrentColorMatrix,
                        x: (int)Math.Round(currntPoint.X / currentZoomRatio * 100),
                        y: (int)Math.Round(currntPoint.Y / currentZoomRatio * 100),
                        thickness: annotationInfo.EraserThickness,
                        horizontalReflect: horizontalReflect, verticalReflect: verticalReflect, rotate: currentRotate
                    );
                }

                DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);
            }
        }

        /// <summary>
        /// Image MouseUp
        /// </summary>
        /// <param name="e"></param>
        private void ImageMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                System.Windows.Point currentPoint = e.GetPosition(view.ImageView);

                if (annotationInfo.PenEnabled || annotationInfo.EraserEnabled || annotationInfo.TextEnabled)
                {
                    if (annotationImage == null)
                        annotationImage = imageService.MakeEmptyImage(originalImageWidth, originalImageHeight);

                    if (annotationInfo.PenEnabled && imagePreviousPoint != null)
                    {
                        imageService.DrawPen(
                            annotationImage: annotationImage, displayImage: displayingImageGDI,
                            x1: (int)Math.Round(imagePreviousPoint.Value.X / currentZoomRatio * 100), y1: (int)Math.Round(imagePreviousPoint.Value.Y / currentZoomRatio * 100),
                            x2: (int)Math.Round(currentPoint.X / currentZoomRatio * 100), y2: (int)Math.Round(currentPoint.Y / currentZoomRatio * 100),
                            thickness: annotationInfo.PenThickness, color: annotationInfo.PenColor,
                            horizontalReflect: horizontalReflect, verticalReflect: verticalReflect, rotate: currentRotate
                        );
                    }
                    else if (annotationInfo.EraserEnabled)
                    {
                        imageService.DrawEraser(
                            annotationImage: annotationImage, displayImage: displayingImageGDI, originalImage: flippedOriginalImage,
                            colorMatrix: CurrentColorMatrix,
                            x: (int)Math.Round(currentPoint.X / currentZoomRatio * 100), y: (int)Math.Round(currentPoint.Y / currentZoomRatio * 100),
                            thickness: annotationInfo.EraserThickness,
                            horizontalReflect: horizontalReflect, verticalReflect: verticalReflect, rotate: currentRotate
                        );
                    }
                    else if (annotationInfo.TextEnabled)
                    {
                        if (!InputBoxWindow.IsShow)
                        {
                            InputBoxWindow inputBoxWindow = new InputBoxWindow(EventAggregator) { Topmost = true };
                            inputBoxWindow.Show();

                            TextAnnotationDialogParam param = new TextAnnotationDialogParam(
                                Title: "",
                                Content: "Please enter text.",
                                X: (int)Math.Round(currentPoint.X / currentZoomRatio * 100),
                                Y: (int)Math.Round(currentPoint.Y / currentZoomRatio * 100)
                            );

                            EventAggregator.GetEvent<TextAnnotationDialogEvent>().Publish(param);
                        }
                    }

                    DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);

                    if (view != null && view.WindowId == dataManager.MainWindowId)
                        annotationBitmapList.Add(new Bitmap(displayingImageGDI));
                }

                imagePreviousPoint = null;
            }
        }

        /// <summary>
        /// ViewPort MouseDown
        /// </summary>
        /// <param name="e"></param>
        private void ViewPortMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                viewPortPreviousPoint = e.GetPosition(view.ImageOverlayCanvas);
                imagePreviousPoint = e.GetPosition(view.ImageView);
            }
        }

        /// <summary>
        /// ViewPort MouseMove
        /// </summary>
        /// <param name="e"></param>
        private void ViewPortMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentPoint = e.GetPosition(view.ImageOverlayCanvas);

                if (viewPortPreviousPoint == null)
                {
                    viewPortPreviousPoint = new System.Windows.Point(currentPoint.X, currentPoint.Y);
                    return;
                }

                double x = Math.Min(currentPoint.X, viewPortPreviousPoint.Value.X);
                double y = Math.Min(currentPoint.Y, viewPortPreviousPoint.Value.Y);
                double width = Math.Abs(currentPoint.X - viewPortPreviousPoint.Value.X);
                double height;

                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    height = width;
                else
                    height = Math.Abs(currentPoint.Y - viewPortPreviousPoint.Value.Y);

                if (annotationInfo.CropCrosshairEnabled)
                {
                    if (annotationInfo.CropRectangleEnabled)
                        EventAggregator.GetEvent<DrawCropBoxEvent>().Publish(new DrawParam(x, y, width, height));
                    else if (annotationInfo.CropCircleEnabled)
                        EventAggregator.GetEvent<DrawCropCircleEvent>().Publish(new DrawParam(x, y, width, height));
                    else if (annotationInfo.CropTriangleEnabled)
                        EventAggregator.GetEvent<DrawCropTriangleEvent>().Publish(new DrawParam(x, y, width, height));
                }
                else if (annotationInfo.DrawRectangleEnabled)
                {
                    if (drawRectangle == null)
                    {
                        drawRectangle = new System.Windows.Shapes.Rectangle()
                        {
                            Stroke = new SolidColorBrush(annotationInfo.DrawColor),
                            StrokeThickness = annotationInfo.DrawThickness
                        };

                        Panel.SetZIndex(drawRectangle, 1);
                        view.ImageOverlayCanvas.Children.Add(drawRectangle);
                    }

                    drawRectangle.Width = width;
                    drawRectangle.Height = height;
                    Canvas.SetLeft(drawRectangle, x);
                    Canvas.SetTop(drawRectangle, y);
                }
                else if (annotationInfo.DrawCircleEnabled)
                {
                    if (drawEllipse == null)
                    {
                        drawEllipse = new System.Windows.Shapes.Ellipse()
                        {
                            Stroke = new SolidColorBrush(annotationInfo.DrawColor),
                            StrokeThickness = annotationInfo.DrawThickness
                        };

                        Panel.SetZIndex(drawEllipse, 1);
                        view.ImageOverlayCanvas.Children.Add(drawEllipse);
                    }

                    drawEllipse.Width = width;
                    drawEllipse.Height = height;
                    Canvas.SetLeft(drawEllipse, x);
                    Canvas.SetTop(drawEllipse, y);
                }
                else if (annotationInfo.DrawTriangleEnabled)
                {
                    if (drawTriangle == null)
                    {
                        drawTriangle = new System.Windows.Shapes.Polygon()
                        {
                            Stroke = new SolidColorBrush(annotationInfo.DrawColor),
                            StrokeThickness = annotationInfo.DrawThickness
                        };

                        Panel.SetZIndex(drawTriangle, 1);
                        view.ImageOverlayCanvas.Children.Add(drawTriangle);
                    }

                    PointCollection Points = new PointCollection
                    {
                        new System.Windows.Point(viewPortPreviousPoint.Value.X + (currentPoint.X - viewPortPreviousPoint.Value.X) / 2, viewPortPreviousPoint.Value.Y),
                        new System.Windows.Point(currentPoint.X, currentPoint.Y),
                        new System.Windows.Point(viewPortPreviousPoint.Value.X, currentPoint.Y)
                    };
                    drawTriangle.Points = Points;
                }
                else if (annotationInfo.DrawLineEnabled)
                {
                    if (drawLine == null)
                    {
                        drawLine = new System.Windows.Shapes.Line()
                        {
                            Stroke = new SolidColorBrush(annotationInfo.DrawColor),
                            StrokeThickness = annotationInfo.DrawThickness
                        };

                        Panel.SetZIndex(drawLine, 1);
                        view.ImageOverlayCanvas.Children.Add(drawLine);
                    }

                    drawLine.X1 = viewPortPreviousPoint.Value.X;
                    drawLine.Y1 = viewPortPreviousPoint.Value.Y;
                    drawLine.X2 = currentPoint.X;
                    drawLine.Y2 = currentPoint.Y;
                }
                else if (measurementEnabled)
                {
                    if (drawMeasurementLine == null)
                    {
                        drawMeasurementLine = new System.Windows.Shapes.Line()
                        {
                            Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0)),
                            StrokeThickness = 2
                        };

                        Panel.SetZIndex(drawMeasurementLine, 1);
                        view.ImageOverlayCanvas.Children.Add(drawMeasurementLine);
                    }

                    drawMeasurementLine.X1 = viewPortPreviousPoint.Value.X;
                    drawMeasurementLine.Y1 = viewPortPreviousPoint.Value.Y;
                    drawMeasurementLine.X2 = currentPoint.X;
                    drawMeasurementLine.Y2 = currentPoint.Y;
                }
            }
        }

        /// <summary>
        /// ViewPort MouseUp
        /// </summary>
        /// <param name="e"></param>
        private void ViewPortMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (annotationInfo.CropCrosshairEnabled)
                {
                    annotationInfo.CropCrosshairEnabled = false;
                }
                else if (annotationInfo.DrawRectangleEnabled || annotationInfo.DrawCircleEnabled 
                    || annotationInfo.DrawTriangleEnabled || annotationInfo.DrawLineEnabled)
                {
                    if (annotationImage == null)
                        annotationImage = imageService.MakeEmptyImage(originalImageWidth, originalImageHeight);

                    System.Windows.Point currentPoint = e.GetPosition(view.ImageView);

                    if (imagePreviousPoint != null)
                    {
                        int x1 = (int)Math.Round(imagePreviousPoint.Value.X / currentZoomRatio * 100);
                        int y1 = (int)Math.Round(imagePreviousPoint.Value.Y / currentZoomRatio * 100);
                        int x2 = (int)Math.Round(currentPoint.X / currentZoomRatio * 100);
                        int y2 = (int)Math.Round(currentPoint.Y / currentZoomRatio * 100);

                        int x = Math.Min(x1, x2);
                        int y = Math.Min(y1, y2);
                        int width = Math.Abs(x1 - x2);
                        int height = Math.Abs(y1 - y2);

                        if (annotationInfo.DrawRectangleEnabled)
                        {
                            view.ImageOverlayCanvas.Children.Remove(drawRectangle);
                            drawRectangle = null;

                            imageService.DrawRectangle(annotationImage, displayingImageGDI, x, y, width, height, annotationInfo.DrawThickness, annotationInfo.DrawColor,
                                    horizontalReflect, verticalReflect, currentRotate);
                        }
                        else if (annotationInfo.DrawCircleEnabled)
                        {
                            view.ImageOverlayCanvas.Children.Remove(drawEllipse);
                            drawEllipse = null;

                            imageService.DrawCircle(annotationImage, displayingImageGDI, x, y, width, height, annotationInfo.DrawThickness, annotationInfo.DrawColor,
                                    horizontalReflect, verticalReflect, currentRotate);
                        }
                        else if (annotationInfo.DrawTriangleEnabled)
                        {
                            view.ImageOverlayCanvas.Children.Remove(drawTriangle);
                            drawTriangle = null;

                            System.Drawing.Point[] points = new System.Drawing.Point[] {
                                new System.Drawing.Point(x1 + (x2 - x1) / 2, y1),
                                new System.Drawing.Point(x2, y2),
                                new System.Drawing.Point(x1, y2),
                            };

                            imageService.DrawTriangle(annotationImage, displayingImageGDI, points, annotationInfo.DrawThickness, annotationInfo.DrawColor,
                                    horizontalReflect, verticalReflect, currentRotate);
                        }
                        else if (annotationInfo.DrawLineEnabled)
                        {
                            view.ImageOverlayCanvas.Children.Remove(drawLine);
                            drawLine = null;

                            imageService.DrawLine(annotationImage, displayingImageGDI, x1, y1, x2, y2, annotationInfo.DrawThickness, annotationInfo.DrawColor,
                                    horizontalReflect, verticalReflect, currentRotate);
                        }

                        DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);

                        if (view != null && view.WindowId == dataManager.MainWindowId)
                            annotationBitmapList.Add(new Bitmap(displayingImageGDI));
                    }
                }
                else if (measurementEnabled && drawMeasurementLine != null)
                {
                    EventAggregator.GetEvent<AddMeasurementEvent>().Publish(new MeasurementData()
                    {
                        Seq = sequense,
                        StartX = (int)drawMeasurementLine.X1 * (fovSizeX == 0 ? 500 : fovSizeX),
                        StartY = (int)drawMeasurementLine.Y1 * (fovSizeY == 0 ? 500 : fovSizeY),
                        EndX = (int)drawMeasurementLine.X2 * (fovSizeX == 0 ? 500 : fovSizeX),
                        EndY = (int)drawMeasurementLine.Y2 * (fovSizeY == 0 ? 500 : fovSizeY),
                    });
                }

                viewPortPreviousPoint = null;
                imagePreviousPoint = null;
            }
        }

        /// <summary>
        /// Draw Clear
        /// </summary>
        private void DrawClear()
        {
            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                if (annotationBitmapList.Count > 0)
                {
                    annotationImage = null;
                    displayingImageGDI = new Bitmap(annotationBitmapList[0]);
                    DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);

                    annotationBitmapList.RemoveRange(1, annotationBitmapList.Count - 1);
                    tempBitmapList.Clear();
                }
            }
        }

        /// <summary>
        /// Draw Undo
        /// </summary>
        private void DrawUndo()
        {
            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                if (annotationBitmapList.Count > 1)
                {
                    tempBitmapList.Add(annotationBitmapList[annotationBitmapList.Count - 1]);
                    annotationBitmapList.RemoveAt(annotationBitmapList.Count - 1);

                    RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// Draw Redo
        /// </summary>
        private void DrawRedo()
        {
            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                if (tempBitmapList.Count > 0)
                {
                    annotationBitmapList.Add(tempBitmapList[tempBitmapList.Count - 1]);
                    tempBitmapList.RemoveAt(tempBitmapList.Count - 1);

                    RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// 이미지 다시 그리기
        /// </summary>
        private void RefreshDisplay()
        {
            annotationImage = new Bitmap(annotationBitmapList[annotationBitmapList.Count - 1]);
            displayingImageGDI = new Bitmap(annotationBitmapList[annotationBitmapList.Count - 1]);
            DisplayingImage = imageService.ConvertGDIBitmapToWPF(displayingImageGDI);
        }

        /// <summary>
        /// Get PositionParam
        /// </summary>
        /// <returns></returns>
        private GetPositionToCropParam GetPositionParam()
        {
            GetPositionToCropParam param = new GetPositionToCropParam();
            EventAggregator.GetEvent<GetPositionToCropEvent>().Publish(param);

            if (!param.Routed)
                return null;

            param.Left = Math.Max(param.Left / currentZoomRatio * 100, 0);
            param.Top = Math.Max(param.Top / currentZoomRatio * 100, 0);

            param.Width = param.Width / currentZoomRatio * 100;
            if (displayingImageGDI.Width < param.Width)
                param.Width = displayingImageGDI.Width;

            param.Height = param.Height / currentZoomRatio * 100;
            if (displayingImageGDI.Height < param.Height)
                param.Height = displayingImageGDI.Height;

            return param;
        }

        /// <summary>
        /// Crop 이미지 저장
        /// </summary>
        private void ExportCrop()
        {
            if (displayingImageGDI == null)
                return;

            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                GetPositionToCropParam param = GetPositionParam();
                if (param != null)
                {
                    VistaSaveFileDialog dialog = new VistaSaveFileDialog
                    {
                        DefaultExt = ".png",
                        Filter = "PNG image file(*.png)|*.png|IVM image file(*.ivm)|*.ivm|TIF image file(*.tif)|*.tif|JPG image file(*.jpg)|*.jpg",
                    };
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        CropImageSave(displayingImageGDI, dialog.FileName, param);
                    }
                }
            }
        }

        /// <summary>
        /// Crop 전체 ExPort
        /// </summary>
        private void ExportAllCrop()
        {
            DirectoryInfo directoryInfo = view.WindowInfo.DirectoryInfo;
            if (directoryInfo != null)
            {
                GetPositionToCropParam param = GetPositionParam();
                if (param != null)
                {
                    VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        foreach (FileInfo fileinfo in Container.Resolve<FileService>().GetImagesInFolder(directoryInfo, new[] { ".png" }, true))
                        {
                            Bitmap displayingImageGDI = CreateDisplayBitmap(fileinfo);
                            string filePath = dialog.SelectedPath + "\\" + fileinfo.Name;
                            CropImageSave(displayingImageGDI, filePath, param);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Crop 이미지 저장
        /// </summary>
        /// <param name="displayingImageGDI"></param>
        /// <param name="fileName"></param>
        /// <param name="param"></param>
        private void CropImageSave(Bitmap displayingImageGDI, string fileName, GetPositionToCropParam param)
        {
            ShapeType shapeType = annotationInfo.CropRectangleEnabled ? ShapeType.Rectangle : annotationInfo.CropCircleEnabled ? ShapeType.Ellipse : ShapeType.Triangle;

            if (annotationInfo.CropInvertEnabled)
            {
                using (Bitmap bitmap = imageService.CreateInvertCroppedImage(displayingImageGDI, param.Left, param.Top, param.Width, param.Height, shapeType))
                {
                    bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            else
            {
                using (Bitmap bitmap = imageService.CreateCroppedImage(displayingImageGDI, param.Left, param.Top, param.Width, param.Height, shapeType))
                {
                    bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// 현재 도시되고 있는 이미지 저장
        /// </summary>
        private void ExportDraw()
        {
            if (displayingImageGDI == null)
                return;

            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                VistaSaveFileDialog dlg = new VistaSaveFileDialog
                {
                    DefaultExt = ".png",
                    Filter = "PNG image file(*.png)|*.png|IVM image file(*.ivm)|*.ivm|TIF image file(*.tif)|*.tif|JPG image file(*.jpg)|*.jpg",
                };
                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    displayingImageGDI.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        /// <summary>
        /// 현재 도시되고 있는 파일 경로의 모든 이미지 저장
        /// </summary>
        private void ExportDrawAll()
        {
            if (view != null && view.WindowId == dataManager.MainWindowId)
            {
                DirectoryInfo directoryInfo = view.WindowInfo.DirectoryInfo;
                if (directoryInfo != null)
                {
                    VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        foreach (FileInfo fileinfo in Container.Resolve<FileService>().GetImagesInFolder(directoryInfo, new[] { ".png" }, true))
                        {
                            Bitmap displayingImageGDI = CreateDisplayBitmap(fileinfo);
                            string filePath = dialog.SelectedPath + "\\" + fileinfo.Name;
                            displayingImageGDI.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw Measurement
        /// </summary>
        /// <param name="enabled"></param>
        private void DrawMeasurement(bool enabled)
        {
            measurementEnabled = enabled;
            if (!enabled)
            {
                view.ImageOverlayCanvas.Children.Remove(drawMeasurementLine);
                drawMeasurementLine = null;
            }
        }

        /// <summary>
        /// ZoomIn
        /// </summary>
        private void ZoomIn()
        {
            if (originalImage != null)
            {
                int newScale = (int)Math.Floor(currentZoomRatio / 10d) * 10 + 10;
                double newWidth = originalImageWidth * (newScale / 100d);
                if (newScale <= 300 && newScale >= 10 && newWidth >= 20)
                {
                    DisplayingImageWidth = originalImageWidth * (newScale / 100d);
                }
            }
        }

        /// <summary>
        /// ZoomOut
        /// </summary>
        private void ZoomOut()
        {
            if (originalImage != null)
            {
                int newScale = (int)Math.Ceiling(currentZoomRatio / 10d) * 10 - 10;
                double newWidth = originalImageWidth * (newScale / 100d);
                if (newScale <= 300 && newScale >= 10 && newWidth >= 20)
                {
                    DisplayingImageWidth = originalImageWidth * (newScale / 100d);
                }
            }
        }

        /// <summary>
        /// 회전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Rotation(string type)
        {
            switch (type)
            {
                case "Left":
                    currentRotate++;
                    currentRotate %= 4;
                    break;
                case "Right":
                    currentRotate--;
                    currentRotate %= 4;

                    if (currentRotate < 0)
                        currentRotate += 4;
                    break;
            }

            EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
        }

        /// <summary>
        /// 좌우 또는 상하 반전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Reflect(string type)
        {
            switch (type)
            {
                case "HorizontalLeft":
                    horizontalReflect = false;
                    break;
                case "HorizontalRight":
                    horizontalReflect = true;
                    break;
                case "VerticalTop":
                    verticalReflect = false;
                    break;
                case "VerticalBottom":
                    verticalReflect = true;
                    break;
            }

            EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
        }

        /// <summary>
        /// 회전 및 반전 초기화
        /// </summary>
        private void RotationReset()
        {
            currentRotate = 0;
            horizontalReflect = false;
            verticalReflect = false;

            EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
        }

        /// <summary>
        /// ZoomRatio 변경 시
        /// </summary>
        /// <param name="zoomRatio"></param>
        private void ZoomRatioControl(int zoomRatio)
        {
            if (originalImage != null && dataManager.MainWindowId == view.WindowId)
                DisplayingImageWidth = originalImage.Width * (zoomRatio / 100d);
        }

        /// <summary>
        /// 스크롤 뷰 사이즈 변경 or 스크롤 변경 시
        /// </summary>
        private void SizeChanged()
        {
            if (displayingImageGDI != null)
            {
                EventAggregator.GetEvent<NavigatorChangeEvent>().Publish(new NavigatorParam
                {
                    ImageWidth = displayingImageGDI.Width,
                    ImageHeight = displayingImageGDI.Height,
                    ZoomRatio = currentZoomRatio
                });
            }
        }

        private Bitmap CreateDisplayBitmap(FileInfo fileInfo)
        {
            // 레지스트레이션 체크
            FileInfo registrationFile = new FileInfo(Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.Name) + "_Reg" + fileInfo.Extension));

            FileInfo fileToDisplay;
            if (registrationFile.Exists)
                fileToDisplay = registrationFile;
            else
                fileToDisplay = fileInfo;

            using (Bitmap originalBitmap = new Bitmap(fileToDisplay.FullName))
            {
                List<ColorMap?> colormaps = colorChannelInfoMap.Values.Select<ColorChannelModel, ColorMap?>(c =>
                {
                    if (c.Visible && c.ColorMapEnabled) return c.ColorMap;
                    else return null;
                }).ToList();

                using (Bitmap img1 = imageService.TranslateColor(originalBitmap, CurrentColorMatrix))
                using (Bitmap img2 = imageService.ApplyColorMaps(img1, colormaps))
                using (Bitmap workingImage = new Bitmap(img2))
                {
                    // 반전 및 회전
                    imageService.ReflectAndRotate(workingImage, horizontalReflect, verticalReflect, currentRotate);

                    // 한번 뒤집기 후에는 반드시 비트맵을 다시 생성해줘야 함.
                    // 90도, 270도 플립시 이미지의 가로 길이보다 Y좌표가 큰 영역에 쓰지 못하는 문제가 있음. (원인불명)
                    using (Bitmap bitmap = new Bitmap(workingImage))
                    {
                        // 어노테이션
                        if (annotationImage != null)
                        {
                            using (Bitmap annotationImg = new Bitmap(annotationImage))
                            {
                                imageService.ReflectAndRotate(annotationImg, horizontalReflect, verticalReflect, currentRotate);
                                imageService.DrawImageOnImage(bitmap, annotationImg);
                            }
                        }

                        int scaleBarSize = annotationInfo.ScaleBarSize;

                        // 스케일 바
                        if (annotationInfo.ScaleBarEnabled && fovSizeX > 0 && fovSizeY > 0 && scaleBarSize > 0 && scaleBarSize < fovSizeX && scaleBarSize < fovSizeY)
                            imageService.DrawScaleBar(bitmap, fovSizeX, fovSizeY, scaleBarSize, annotationInfo.ScaleBarThickness, 30,
                                annotationInfo.XAxisEnabled, annotationInfo.YAxisEnabled, annotationInfo.ScaleBarPosition, annotationInfo.ScaleBarLabel, annotationInfo.TextFontSize, annotationInfo.TextColor);

                        // TimeStack
                        if (annotationInfo.TimeStampEnabled)
                            imageService.DrawTimeStampLabel(bitmap, annotationInfo.TimeStampText, annotationInfo.TimeStampPosition, annotationInfo.TextFontSize, annotationInfo.TextColor, 30);

                        // ZStackLabel
                        if (annotationInfo.ZStackLabelEnabled)
                            imageService.DrawZStackLabel(bitmap, annotationInfo.ZStackLabelText, annotationInfo.ZStackLabelPosition, annotationInfo.TextFontSize, annotationInfo.TextColor, 30);

                        Bitmap displayingImageGDI = new Bitmap(bitmap);
                        return displayingImageGDI;
                    }
                }
            }
        }
    }
}
