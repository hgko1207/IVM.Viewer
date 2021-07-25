using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System.Windows.Input;

/**
 * @Class Name : DisplayControlPanelViewModel.cs
 * @Description : 이미지 컨트롤 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.10     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.10
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class DisplayControlPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<DisplayControlPanel>
    {
        private bool isNavigator;
        public bool IsNavigator
        {
            get => isNavigator;
            set => SetProperty(ref isNavigator, value);
        }

        private int zoomRatioValue;
        public int ZoomRatioValue
        {
            get => zoomRatioValue;
            set
            {
                if (SetProperty(ref zoomRatioValue, value))
                    EventAggregator.GetEvent<ZoomRatioControlEvent>().Publish(value);
            }
        }

        public ICommand ResetZoomRatioCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public DisplayControlPanelViewModel(IContainerExtension container) : base(container)
        {
            ResetZoomRatioCommand = new DelegateCommand(ResetZoomRatio);

            EventAggregator.GetEvent<ZoomRatioChangedEvent>().Subscribe(ZoomRatioChanged, ThreadOption.UIThread);

            ZoomRatioValue = 100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(DisplayControlPanel view)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(DisplayControlPanel view)
        {
            EventAggregator.GetEvent<ZoomRatioChangedEvent>().Unsubscribe(ZoomRatioChanged);
        }

        /// <summary>
        /// Changed ZoomRatio
        /// </summary>
        /// <param name="zoomRatio"></param>
        private void ZoomRatioChanged(int zoomRatio)
        {
            ZoomRatioValue = zoomRatio;  
        }

        /// <summary>
        /// Reset ZoomRatio
        /// </summary>
        private void ResetZoomRatio()
        {
            ZoomRatioValue = 100;

            EventAggregator.GetEvent<ZoomRatioControlEvent>().Publish(ZoomRatioValue);
        }
    }
}
