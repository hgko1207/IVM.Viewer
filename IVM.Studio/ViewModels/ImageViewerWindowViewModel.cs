using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.MvvM;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace IVM.Studio.ViewModels
{
    public class ImageViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ImageViewerWindow>
    {
        private ImageViewerWindow view;

        private Bitmap originalImage;
        private Bitmap flippedOriginalImage;
        private Bitmap annotationImage;

        private int currentRotate;

        /// <summary>이미지 새로고침 이벤트를 비활성화하는 플래그입니다.</summary>
        private bool disableRefreshImageEvent;

        public ImageViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Image Viewer";

            currentRotate = 0;

            EventAggregator.GetEvent<DisplayImageEvent>().Subscribe(DisplayImage, ThreadOption.UIThread);
            EventAggregator.GetEvent<RefreshImageEvent>().Subscribe(InternalDisplayImage);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(Close);
        }

        public void OnLoaded(ImageViewerWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(ImageViewerWindow view)
        {
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(Close);
        }

        private void DisplayImage(DisplayParam param)
        {
            disableRefreshImageEvent = true;

            if (param.SlideChanged)
            {
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

                }
            });
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
