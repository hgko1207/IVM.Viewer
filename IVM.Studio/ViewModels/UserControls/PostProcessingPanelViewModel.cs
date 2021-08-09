using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Models.Views;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : PostProcessingPanelViewModel.cs
 * @Description : Post Processing 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.08     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.08
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class PostProcessingPanelViewModel : ViewModelBase
    {
        private ColorChannelItem selectedTimeLapseItem;
        public ColorChannelItem SelectedTimeLapseItem
        {
            get => selectedTimeLapseItem;
            set => SetProperty(ref selectedTimeLapseItem, value);
        }

        private ColorChannelItem selectedZStackItem;
        public ColorChannelItem SelectedZStackItem
        {
            get => selectedZStackItem;
            set => SetProperty(ref selectedZStackItem, value);
        }

        private ColorChannelItem selectedRotationItem;
        public ColorChannelItem SelectedRotationItem
        {
            get => selectedRotationItem;
            set => SetProperty(ref selectedRotationItem, value);
        }

        private ZStackProjectionType selectedProjection;
        public ZStackProjectionType SelectedProjection
        {
            get => selectedProjection;
            set => SetProperty(ref selectedProjection, value);
        }

        private EdgeCropType selectedEdgeCrop;
        public EdgeCropType SelectedEdgeCrop
        {
            get => selectedEdgeCrop;
            set => SetProperty(ref selectedEdgeCrop, value);
        }

        private double mosaicOverlap;
        public double MosaicOverlap
        {
            get => mosaicOverlap;
            set => SetProperty(ref mosaicOverlap, value);
        }

        public bool ZStackProjEnabled => SliderControlInfo.ZSSliderEnabled && dataManager.ViewerName == nameof(ImageViewer);
        public bool MosaicEnabled => SliderControlInfo.MSSliderEnabled && dataManager.ViewerName == nameof(ImageViewer);

        public ICommand ZStackProjCommand { get; private set; }
        public ICommand MosaicCommand { get; private set; }

        private DataManager dataManager;
        public SliderControlInfo SliderControlInfo { get; set; }
        private List<ColorChannelItem> colorChannelItems { get; set; }

        private readonly IEnumerable<string> approvedImageExtensions;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public PostProcessingPanelViewModel(IContainerExtension container) : base(container)
        {
            ZStackProjCommand = new DelegateCommand(ZStackProj);
            MosaicCommand = new DelegateCommand(ApplyMosaic);

            EventAggregator.GetEvent<ViewerPageChangedEvent>().Subscribe(ViewerPageChanged);

            dataManager = Container.Resolve<DataManager>();
            SliderControlInfo = dataManager.SliderControlInfo;
            colorChannelItems = dataManager.ColorChannelItems;

            SelectedTimeLapseItem = colorChannelItems[0];
            SelectedZStackItem = colorChannelItems[0];
            SelectedRotationItem = colorChannelItems[0];

            SelectedProjection = ZStackProjectionType.Maximum;
            SelectedEdgeCrop = EdgeCropType.Minimum;

            approvedImageExtensions = new[] { ".ivm" };
        }

        /// <summary>
        /// 메인 뷰어 변경 시
        /// </summary>
        private void ViewerPageChanged()
        {
            RaisePropertyChanged(nameof(ZStackProjEnabled));
            RaisePropertyChanged(nameof(MosaicEnabled));
        }

        /// <summary>
        /// Apply ZStackProj
        /// </summary>
        private async void ZStackProj()
        {
            SlideInfo selectedSlideInfo = dataManager.SelectedSlideInfo;
            string currentSlidesPath = dataManager.CurrentSlidesPath;
            if (dataManager.ViewerName != nameof(ImageViewer) && selectedSlideInfo == null)
                return;

            int idx = 0;

            DirectoryInfo targetFolder = new DirectoryInfo(Path.Combine(currentSlidesPath, $"{selectedSlideInfo.Name}_ZSProj"));
            while (targetFolder.Exists)
            {
                targetFolder = new DirectoryInfo(Path.Combine(currentSlidesPath, $"{selectedSlideInfo.Name}_ZSProj_{idx}"));
                idx++;
            }

            try
            {
                await Container.Resolve<BatchImageService>().ZStackProjection(
                    slidesRootDir: currentSlidesPath,
                    srcSlideRootDir: new DirectoryInfo(Path.Combine(currentSlidesPath, selectedSlideInfo.Name)),
                    dstSlideRootDir: targetFolder,
                    approvedExtensions: new[] { approvedImageExtensions.First() },
                    startZIndex: SliderControlInfo.ZStackProjLowerIndex,
                    endZIndex: SliderControlInfo.ZStackProjUpperIndex
                );

                WinUIMessageBox.Show("Z 스택 프로젝션이 완료되었습니다.", "Z 스택 프로젝션", MessageBoxButton.OK, MessageBoxImage.Information);
                EventAggregator.GetEvent<RefreshFolderEvent>().Publish(targetFolder);
            }
            catch (ArgumentException)
            {
                WinUIMessageBox.Show("선택한 슬라이드가 Z 스택 프로젝션을 실행하기 위한 충분한 이미지를 가지고 있지 않습니다.", "Z 스택 프로젝션", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (FileNotFoundException)
            {
                WinUIMessageBox.Show("프로젝션 도중 대상 이미지 파일을 찾지 못했습니다.", "Z 스택 프로젝션", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                WinUIMessageBox.Show($"원인 불명의 오류입니다. {e.Message}", "Z 스택 프로젝션", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Apply Mosaic
        /// </summary>
        private void ApplyMosaic()
        {
            SlideInfo selectedSlideInfo = dataManager.SelectedSlideInfo;
            if (dataManager.ViewerName != nameof(ImageViewer) && selectedSlideInfo == null)
                return;

            if (SliderControlInfo.ZSSliderEnabled)
            {
                MessageBoxResult boxResult = ThemedMessageBox.Show(title: "모자이크 수행", text: "슬라이드 내에 Z 스택 이미지 모드가 존재합니다. Z 스택 프로젝션을 수행 후 모자이크를 수행하시겠습니까?",
                         messageBoxButtons: MessageBoxButton.YesNo, icon: MessageBoxImage.Question);

                RunMosaic(boxResult == MessageBoxResult.Yes);
            }
            else
            {
                RunMosaic(false);
            }
        }

        /// <summary>
        /// 모자이크 수행
        /// </summary>
        /// <param name="zStackReg"></param>
        private async void RunMosaic(bool zStackReg)
        {
            if (zStackReg)
            {
                ZStackProj();
            }

            {
                SlideInfo selectedSlideInfo = dataManager.SelectedSlideInfo;
                string currentSlidesPath = dataManager.CurrentSlidesPath;

                int idx = 0;

                // 모자이크
                DirectoryInfo targetFolder = new DirectoryInfo(Path.Combine(currentSlidesPath, $"{selectedSlideInfo.Name}_Mosaic"));
                while (targetFolder.Exists)
                {
                    targetFolder = new DirectoryInfo(Path.Combine(currentSlidesPath, $"{selectedSlideInfo.Name}_Mosaic_{idx}"));
                    idx++;
                }

                try
                {
                    await Container.Resolve<BatchImageService>().Mosaic(
                        slidesRootDir: currentSlidesPath,
                        srcSlideRootDir: new DirectoryInfo(Path.Combine(currentSlidesPath, selectedSlideInfo.Name)),
                        dstSlideRootDir: targetFolder, 
                        approvedExtensions: new[] { approvedImageExtensions.First() }, 
                        cropRate: MosaicOverlap
                    );
                }
                catch (ArgumentException)
                {
                    WinUIMessageBox.Show("선택한 슬라이드가 모자이크를 실행하기 위한 충분한 이미지를 가지고 있지 않습니다.", "모자이크 수행", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (FileNotFoundException)
                {
                    WinUIMessageBox.Show("모자이크 도중 대상 이미지 파일을 찾지 못했습니다.", "모자이크 수행", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception e)
                {
                    WinUIMessageBox.Show($"원인 불명의 오류입니다. {e.Message}", "모자이크 수행", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
