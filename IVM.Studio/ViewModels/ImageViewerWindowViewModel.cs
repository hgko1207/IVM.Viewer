using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Utils;
using IVM.Studio.Views;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
namespace IVM.Studio.ViewModels
{
    public class ImageViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ImageViewerWindow>
    {
        private Drawing.ImageSource displayingImage;
        public Drawing.ImageSource DisplayingImage
        {
            get => displayingImage;
            set => SetProperty(ref displayingImage, value);
        }

        private ImageViewerWindow view;

        private Bitmap originalImage;
        private Bitmap flippedOriginalImage;
        private Bitmap annotationImage;
        private Bitmap displayingImageGDI;

        /// <summary>이미지 새로고침 이벤트를 비활성화하는 플래그입니다.</summary>
        private bool disableRefreshImageEvent;

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap { get; }

        private int[] currentTranslationByChannel;
        private bool[] currentVisibilityByChannel;
        private float[][] currentColorMatrix;

        private int fOVSizeX;
        private int fOVSizeY;

        private bool horizontalReflect;
        private bool verticalReflect;
        private int currentRotate;

        private bool scaleBarEnabled;
        private int scaleBarSize;

        public ImageViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Image Viewer";

            currentRotate = 0;

            EventAggregator.GetEvent<DisplayImageEvent>().Subscribe(DisplayImage, ThreadOption.UIThread);
            EventAggregator.GetEvent<RefreshImageEvent>().Subscribe(InternalDisplayImage);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(Close);

            colorChannelInfoMap = Container.Resolve<DataManager>().ColorChannelInfoMap;

            currentTranslationByChannel = colorChannelInfoMap.Values.Select(s => (int)s.Color).ToArray();
            currentVisibilityByChannel = colorChannelInfoMap.Values.Select(s => s.Visible).ToArray();
            currentColorMatrix = Container.Resolve<ImageService>().GenerateColorMatrix(
                    startLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelLowerValue).ToArray(),
                    endLevelByChannel: colorChannelInfoMap.Values.Select(s => s.ColorLevelUpperValue).ToArray(),
                    brightnessByChannel: colorChannelInfoMap.Values.Select(s => s.Brightness).ToArray(),
                    contrastByChannel: colorChannelInfoMap.Values.Select(s => s.Contrast).ToArray(),
                    translationByChannel: currentTranslationByChannel,
                    visibilityByChannel: currentVisibilityByChannel
                );
        }


        public void OnLoaded(ImageViewerWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(ImageViewerWindow view)
        {
            EventAggregator.GetEvent<DisplayImageEvent>().Unsubscribe(DisplayImage);
            EventAggregator.GetEvent<RefreshImageEvent>().Unsubscribe(InternalDisplayImage);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(Close);
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
            }

            if (param.Metadata != null)
            {
                fOVSizeX = param.Metadata.FovX;
                fOVSizeY = param.Metadata.FovY;
               
                // 메타데이터의 채널 정보 반영
                ChannelNameConverter converter = Container.Resolve<FileService>().GenerateChannelNameConverter(param.Metadata);
                foreach (ChannelType type in Enum.GetValues(typeof(ChannelType)))
                {
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

        private void DisplayImageWithoutMetadata(FileInfo file)
        {
            // 레지스트레이션 체크
            FileInfo registrationFile = new FileInfo(Path.Combine(file.DirectoryName, Path.GetFileNameWithoutExtension(file.Name) + "_Reg" + file.Extension));

            FileInfo fileToDisplay;
            if (registrationFile.Exists)
                fileToDisplay = registrationFile;
            else
                fileToDisplay = file;

            originalImage?.Dispose();
            originalImage = Container.Resolve<ImageService>().LoadImage(fileToDisplay.FullName);

            // 어노테이션 초기화
            annotationImage?.Dispose();
            annotationImage = null;

            // 로테이션 초기화
            currentRotate = 0;

            InternalDisplayImage();
        }

        /// <summary>
        /// 지정한 색상값에 따라 이미지의 색상을 투영한 후, 어노테이션 이미지를 붙여 화면에 표시하고, 히스토그램을 생성하는 내부 메서드
        /// </summary>
        private void InternalDisplayImage()
        {
            if (originalImage == null || disableRefreshImageEvent)
                return;

            // 이미지 표시는 히스토그램 생성 등으로 인해 오래 걸리므로 백그라운드에서 처리
            Task.Run(() =>
            {
                // 주 이미지 변경
                {
                    List<ColorMap?> colormaps = colorChannelInfoMap.Values.Select<ColorChannelModel, ColorMap?>(s => {
                        if (s.Visible && s.ColorMapEnabled) return s.ColorMap;
                        else return null;
                    }).ToList();

                    using (Bitmap img1 = Container.Resolve<ImageService>().TranslateColor(originalImage, currentColorMatrix))
                    using (Bitmap img2 = Container.Resolve<ImageService>().ApplyColorMaps(img1, colormaps))
                    {
                        InternalDisplayAnnotatedImage(img2);
                        InternalDisplayHistogram(img2);
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
            }

            // 한번 뒤집기 후에는 반드시 비트맵을 다시 생성해줘야 함.
            // 90도, 270도 플립시 이미지의 가로 길이보다 Y좌표가 큰 영역에 쓰지 못하는 문제가 있음. (원인불명)
            using (Bitmap workingImage = new Bitmap(flippedOriginalImage))
            {
                if (workingImage == null)
                    return;

                // 어노테이션
                if (annotationImage != null)
                {
                    using (Bitmap annotationImg = new Bitmap(annotationImage))
                    {
                        Container.Resolve<ImageService>().ReflectAndRotate(annotationImg, horizontalReflect, verticalReflect, currentRotate);
                        Container.Resolve<ImageService>().DrawImageOnImage(workingImage, annotationImg);
                    }
                }

                // 스케일 바
                if (scaleBarEnabled && fOVSizeX > 0 && fOVSizeY > 0 && scaleBarSize > 0 && scaleBarSize < fOVSizeX && scaleBarSize < fOVSizeY)
                    Container.Resolve<ImageService>().DrawScaleBar(workingImage, fOVSizeX, fOVSizeY, scaleBarSize, 2, 3, 9);

                displayingImageGDI?.Dispose();
                displayingImageGDI = new Bitmap(workingImage);
                DisplayingImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(workingImage);
            }
        }

        /// <summary>
        /// 히스토그램 생성
        /// </summary>
        /// <param name="img"></param>
        private void InternalDisplayHistogram(Bitmap img)
        {
            using (Bitmap hist = Container.Resolve<ImageService>().CreateHistogram(img, currentTranslationByChannel, currentVisibilityByChannel))
            {
                Container.Resolve<DataManager>().HistogramImage = Container.Resolve<ImageService>().ConvertGDIBitmapToWPF(hist);
            }
        }

        /// <summary>
        /// 종료 이벤트
        /// </summary>
        private void Close()
        {
            view.Close();
        }
    }
}
