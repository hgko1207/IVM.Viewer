using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System.Windows.Input;

/**
 * @Class Name : RotateCropPanelViewModel.cs
 * @Description : 회전 및 자리그 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.30     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.30
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class RotateCropPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<RotateCropPanel>
    {
        private bool reflectHorizontalEnabled;
        public bool ReflectHorizontalEnabled
        {
            get => reflectHorizontalEnabled;
            set
            {
                if (SetProperty(ref reflectHorizontalEnabled, value))
                    EventAggregator.GetEvent<ReflectEvent>().Publish(value ? "HorizontalRight" : "HorizontalLeft");
            }
        }

        private bool reflectVerticalEnabled;
        public bool ReflectVerticalEnabled
        {
            get => reflectVerticalEnabled;
            set
            {
                if (SetProperty(ref reflectVerticalEnabled, value))
                    EventAggregator.GetEvent<ReflectEvent>().Publish(value ? "VerticalBottom" : "VerticalTop");
            }
        }

        private bool isLockRotate;
        public bool IsLockRotate
        {
            get => isLockRotate;
            set => SetProperty(ref isLockRotate, value);
        }

        public ICommand RotationCommand { get; private set; }
        public ICommand RotationResetCommand { get; private set; }
        

        public AnnotationInfo AnnotationInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public RotateCropPanelViewModel(IContainerExtension container) : base(container)
        {
            RotationCommand = new DelegateCommand<string>(Rotation);
            RotationResetCommand = new DelegateCommand(RotationReset);

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
        /// 회전, 반전 초기화
        /// </summary>
        private void RotationReset()
        {
            SetProperty(ref reflectHorizontalEnabled, false, nameof(ReflectHorizontalEnabled));
            SetProperty(ref reflectVerticalEnabled, false, nameof(ReflectVerticalEnabled));
            EventAggregator.GetEvent<RotationResetEvent>().Publish();
        }
    }
}
