using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System.Windows.Input;

namespace IVM.Studio.ViewModels.UserControls
{
    public class RotateCropPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<RotateCropPanel>
    {
        private bool isLockRotate;
        public bool IsLockRotate
        {
            get => isLockRotate;
            set => SetProperty(ref isLockRotate, value);
        }

        private bool invertEnabled;
        public bool InvertEnabled
        {
            get => invertEnabled;
            set => SetProperty(ref invertEnabled, value);
        }

        public ICommand RotationCommand { get; private set; }
        public ICommand ReflectCommand { get; private set; }
        public ICommand RotationResetCommand { get; private set; }

        public ICommand AllCropCommand { get; private set; }
        public ICommand ExportCropCommand { get; private set; }

        public AnnotationInfo AnnotationInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public RotateCropPanelViewModel(IContainerExtension container) : base(container)
        {
            RotationCommand = new DelegateCommand<string>(Rotation);
            ReflectCommand = new DelegateCommand<string>(Reflect);
            RotationResetCommand = new DelegateCommand(RotationReset);
            AllCropCommand = new DelegateCommand(AllCrop);
            ExportCropCommand = new DelegateCommand(ExportCrop);

            EventAggregator.GetEvent<RefreshMetadataEvent>().Subscribe(DisplayImageWithMetadata, ThreadOption.UIThread);

            AnnotationInfo = Container.Resolve<DataManager>().AnnotationInfo;

            IsLockRotate = true;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(RotateCropPanel view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(RotateCropPanel view)
        {
            EventAggregator.GetEvent<RefreshMetadataEvent>().Unsubscribe(DisplayImageWithMetadata);
        }

        /// <summary>
        /// 메타데이터 표출
        /// </summary>
        /// <param name="param"></param>
        private void DisplayImageWithMetadata(DisplayParam param)
        {
            if (!IsLockRotate)
                RotationReset();
        }

        /// <summary>
        /// 회전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Rotation(string type)
        {
            EventAggregator.GetEvent<RotationEvent>().Publish(type);
        }

        /// <summary>
        /// 좌우 또는 상하 반전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Reflect(string type)
        {
            EventAggregator.GetEvent<ReflectEvent>().Publish(type);
        }

        /// <summary>
        /// 회전, 반전 초기화
        /// </summary>
        private void RotationReset()
        {
            EventAggregator.GetEvent<RotationResetEvent>().Publish();
        }

        /// <summary>
        /// All Crop
        /// </summary>
        private void AllCrop()
        {

        }

        /// <summary>
        /// Export Crop
        /// </summary>
        private void ExportCrop()
        {

        }
    }
}
