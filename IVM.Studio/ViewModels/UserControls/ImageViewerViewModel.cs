using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Utils;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Drawing = System.Windows.Media;

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
        private Drawing.ImageSource displayingImage;
        public Drawing.ImageSource DisplayingImage
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

        public ICommand MouseWheelCommand { get; set; }
        public ICommand SizeChangedCommand { get; set; }

        private Bitmap originalImage;
        private Bitmap flippedOriginalImage;
        private Bitmap annotationImage;
        private Bitmap displayingImageGDI;

        /// <summary>뷰에서 표시 될 현재 이미지의 줌 배율입니다. 단위는 퍼센트입니다.</summary>
        /// <remarks>이미지의 표시 배율을 뷰모델에서 조정할 경우 <see cref="DisplayingImageWidth"/>를 사용합니다.</remarks>
        private int currentZoomRatio;

        /// <summary>이미지 새로고침 이벤트를 비활성화하는 플래그입니다.</summary>
        private bool disableRefreshImageEvent;

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap { get; }

        private int[] currentTranslationByChannel => colorChannelInfoMap.Values.Select(s => (int)s.Color).ToArray();
        private bool[] currentVisibilityByChannel => colorChannelInfoMap.Values.Select(s => s.Visible).ToArray();
        private float[][] currentColorMatrix => Container.Resolve<ImageService>().GenerateColorMatrix(
                    startLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelLowerValue).ToArray(),
                    endLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelUpperValue).ToArray(),
                    brightnessByChannel: colorChannelInfoMap.Values.Select(s => s.Brightness).ToArray(),
                    contrastByChannel: colorChannelInfoMap.Values.Select(s => s.Contrast).ToArray(),
                    translationByChannel: currentTranslationByChannel,
                    visibilityByChannel: currentVisibilityByChannel
                );

        private int fOVSizeX;
        private int fOVSizeY;

        private bool horizontalReflect;
        private bool verticalReflect;
        private int currentRotate;

        private bool scaleBarEnabled;
        private int scaleBarSize;

        FileInfo fileToDisplay;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageViewerViewModel(IContainerExtension container) : base(container)
        {
            MouseWheelCommand = new DelegateCommand<MouseWheelEventArgs>(MouseWheel);
            SizeChangedCommand = new DelegateCommand<SizeChangedEventArgs>(SizeChanged);

            EventAggregator.GetEvent<DisplayImageEvent>().Subscribe(DisplayImage, ThreadOption.UIThread);

            currentZoomRatio = 100;
            DisplayingImageWidth = double.NaN;
            currentRotate = 0;

            colorChannelInfoMap = Container.Resolve<DataManager>().ColorChannelInfoMap.Values.Where(c => c.ChannelType != ChannelType.ALL).ToDictionary(data => data.ChannelType);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ImageViewer view)
        {
            EventAggregator.GetEvent<RefreshImageEvent>().Subscribe(InternalDisplayImage, ThreadOption.UIThread);

            EventAggregator.GetEvent<RotationEvent>().Subscribe(Rotation, ThreadOption.UIThread);
            EventAggregator.GetEvent<ReflectEvent>().Subscribe(Reflect, ThreadOption.UIThread);
            EventAggregator.GetEvent<RotationResetEvent>().Subscribe(RotationReset, ThreadOption.UIThread);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ImageViewer view)
        {
            EventAggregator.GetEvent<RefreshImageEvent>().Unsubscribe(InternalDisplayImage);
            EventAggregator.GetEvent<RotationEvent>().Unsubscribe(Rotation);
            EventAggregator.GetEvent<ReflectEvent>().Unsubscribe(Reflect);
            EventAggregator.GetEvent<RotationResetEvent>().Unsubscribe(RotationReset);
        }

        /// <summary>
        /// DisplayImage
        /// </summary>
        /// <param name="param"></param>
        private void DisplayImage(DisplayParam param)
        {
            disableRefreshImageEvent = true;

            if (param.SlideChanged)
            {
                colorChannelInfoMap[ChannelType.DAPI].Color = Colors.Red;
                colorChannelInfoMap[ChannelType.DAPI].Visible = true;
                colorChannelInfoMap[ChannelType.GFP].Color = Colors.Green;
                colorChannelInfoMap[ChannelType.GFP].Visible = true;
                colorChannelInfoMap[ChannelType.RFP].Color = Colors.Blue;
                colorChannelInfoMap[ChannelType.RFP].Visible = true;
                colorChannelInfoMap[ChannelType.NIR].Color = Colors.Alpha;
                colorChannelInfoMap[ChannelType.NIR].Visible = false;

                EventAggregator.GetEvent<SlideChangedEvent>().Publish();
            }

            if (param.Metadata != null)
            {
                fOVSizeX = param.Metadata.FovX;
                fOVSizeY = param.Metadata.FovY;

                // 메타데이터의 채널 정보 반영
                ChannelNameConverter converter = Container.Resolve<FileService>().GenerateChannelNameConverter(param.Metadata);
                foreach (ChannelType type in Enum.GetValues(typeof(ChannelType)))
                {
                    if (type == ChannelType.ALL)
                        continue;

                    colorChannelInfoMap[type].ChannelName = converter.ConvertNumberToName((int)type);
                }
            }
            else
            {
                fOVSizeX = 0;
                fOVSizeY = 0;
            }

            disableRefreshImageEvent = false;

            DisplayImageWithoutMetadata(param.FileInfo);
        }

        /// <summary>
        /// DisplayImageWithoutMetadata
        /// </summary>
        /// <param name="file"></param>
        private void DisplayImageWithoutMetadata(FileInfo file)
        {
            // 레지스트레이션 체크
            FileInfo registrationFile = new FileInfo(Path.Combine(file.DirectoryName, Path.GetFileNameWithoutExtension(file.Name) + "_Reg" + file.Extension));
           
            if (registrationFile.Exists)
                fileToDisplay = registrationFile;
            else
                fileToDisplay = file;

            // 어노테이션 초기화
            annotationImage?.Dispose();
            annotationImage = null;

            // 로테이션 초기화
            currentRotate = 0;

            // 디스플레이
            InternalDisplayImage();

            // 슬라이드쇼
            //Container.Resolve<SlideShowService>().ContinueSlideShow();
        }

        /// <summary>
        /// 지정한 색상값에 따라 이미지의 색상을 투영한 후, 어노테이션 이미지를 붙여 화면에 표시하고, 히스토그램을 생성하는 내부 메서드
        /// </summary>
        private async void InternalDisplayImage()
        {
            if (fileToDisplay == null && disableRefreshImageEvent)
                return;

            // 이미지 표시는 히스토그램 생성 등으로 인해 오래 걸리므로 백그라운드에서 처리
            await Task.Run(() =>
            {
                originalImage?.Dispose();
                originalImage = Container.Resolve<ImageService>().LoadImage(fileToDisplay.FullName);

                // 주 이미지 변경
                {
                    List<ColorMap?> colormaps = colorChannelInfoMap.Values.Select<ColorChannelModel, ColorMap?>(c =>
                    {
                        if (c.Visible && c.ColorMapEnabled) return c.ColorMap;
                        else return null;
                    }).ToList();

                    using (Bitmap img1 = Container.Resolve<ImageService>().TranslateColor(originalImage, currentColorMatrix))
                    using (Bitmap img2 = Container.Resolve<ImageService>().ApplyColorMaps(img1, colormaps))
                    {
                        InternalDisplayAnnotatedImage(img2);
                        InternalDisplayHistogram(img2);
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

                            float[][] colorMatrix = Container.Resolve<ImageService>().GenerateColorMatrix(
                                startLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelLowerValue).ToArray(),
                                endLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelUpperValue).ToArray(),
                                brightnessByChannel: colorChannelInfoMap.Values.Select(s => s.Brightness).ToArray(),
                                contrastByChannel: colorChannelInfoMap.Values.Select(s => s.Contrast).ToArray(),
                                translationByChannel: currentTranslationByChannel,
                                visibilityByChannel: visibilityByChannel
                            );

                            if (colorChannelModel.ColorMapEnabled)
                            {
                                using (Bitmap img1 = Container.Resolve<ImageService>().TranslateColor(originalImage, colorMatrix))
                                using (Bitmap img2 = Container.Resolve<ImageService>().ApplyColorMapGDI(img1, currentTranslationByChannel[(int)type], colorChannelModel.ColorMap))
                                {
                                    BitmapSource img = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(img2);
                                    Container.Resolve<WindowByChannelService>().DisplayImage((int)type, img);
                                    using (Bitmap hist = Container.Resolve<ImageService>().CreateHistogram(img2, currentTranslationByChannel, new bool[4] { true, true, true, false }))
                                    {
                                        Drawing.ImageSource histogramImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(hist);
                                        colorChannelModel.HistogramImage = histogramImage;
                                        //Container.Resolve<WindowByHistogramService>().DisplayHistogram((int)type, histogramImage);
                                    }
                                }
                            }
                            else
                            {
                                using (Bitmap img = Container.Resolve<ImageService>().TranslateColor(originalImage, colorMatrix))
                                {
                                    BitmapSource imgwpf = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(img);
                                    Container.Resolve<WindowByChannelService>().DisplayImage((int)type, imgwpf);
                                    using (Bitmap hist = Container.Resolve<ImageService>().CreateHistogram(img, currentTranslationByChannel, visibilityByChannel))
                                    {
                                        Drawing.ImageSource histogramImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(hist);
                                        colorChannelModel.HistogramImage = histogramImage;
                                        //Container.Resolve<WindowByHistogramService>().DisplayHistogram((int)type, histogramImage);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// AnnotatedImage 생성
        /// </summary>
        /// <param name="image"></param>
        private void InternalDisplayAnnotatedImage(Bitmap image)
        {
            using (Bitmap workingImage = new Bitmap(image))
            {
                // 반전 및 회전
                Container.Resolve<ImageService>().ReflectAndRotate(workingImage, horizontalReflect, verticalReflect, currentRotate);

                flippedOriginalImage?.Dispose();
                flippedOriginalImage = new Bitmap(workingImage);

                // 한번 뒤집기 후에는 반드시 비트맵을 다시 생성해줘야 함.
                // 90도, 270도 플립시 이미지의 가로 길이보다 Y좌표가 큰 영역에 쓰지 못하는 문제가 있음. (원인불명)
                using (Bitmap bitmap = new Bitmap(flippedOriginalImage))
                {
                    // 어노테이션
                    if (annotationImage != null)
                    {
                        using (Bitmap annotationImg = new Bitmap(annotationImage))
                        {
                            Container.Resolve<ImageService>().ReflectAndRotate(annotationImg, horizontalReflect, verticalReflect, currentRotate);
                            Container.Resolve<ImageService>().DrawImageOnImage(bitmap, annotationImg);
                        }
                    }

                    // 스케일 바
                    if (scaleBarEnabled && fOVSizeX > 0 && fOVSizeY > 0 && scaleBarSize > 0 && scaleBarSize < fOVSizeX && scaleBarSize < fOVSizeY)
                        Container.Resolve<ImageService>().DrawScaleBar(bitmap, fOVSizeX, fOVSizeY, scaleBarSize, 2, 3, 9);

                    displayingImageGDI?.Dispose();
                    displayingImageGDI = new Bitmap(bitmap);
                    DisplayingImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(bitmap);
                }
            }
        }

        /// <summary>
        /// 히스토그램 생성
        /// </summary>
        /// <param name="imgage"></param>
        private void InternalDisplayHistogram(Bitmap imgage)
        {
            using (Bitmap hist = Container.Resolve<ImageService>().CreateHistogram(imgage, currentTranslationByChannel, currentVisibilityByChannel))
            {
                Container.Resolve<DataManager>().HistogramImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(hist);
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

            EventAggregator.GetEvent<RefreshImageEvent>().Publish();
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

            EventAggregator.GetEvent<RefreshImageEvent>().Publish();
        }

        /// <summary>
        /// 회전 및 반전 초기화
        /// </summary>
        private void RotationReset()
        {
            currentRotate = 0;
            horizontalReflect = false;
            verticalReflect = false;

            EventAggregator.GetEvent<RefreshImageEvent>().Publish();
        }

        /// <summary>
        /// 마우스 휠 동작 이벤트
        /// </summary>
        private void MouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta is int delta)
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
            if (e.NewSize.Width is double value)
                currentZoomRatio = (int)Math.Round(value / displayingImageGDI.Width * 100);
        }

        /// <summary>
        /// ZoomIn
        /// </summary>
        private void ZoomIn()
        {
            if (originalImage != null)
            {
                int newScale = (int)Math.Floor(currentZoomRatio / 10d) * 10 + 10;
                double newWidth = originalImage.Width * (newScale / 100d);
                if (newScale <= 400 && newScale >= 10 && newWidth >= 20)
                {
                    DisplayingImageWidth = originalImage.Width * (newScale / 100d);
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
                double newWidth = originalImage.Width * (newScale / 100d);
                if (newScale <= 400 && newScale >= 10 && newWidth >= 20)
                {
                    DisplayingImageWidth = originalImage.Width * (newScale / 100d);
                }
            }
        }
    }
}
