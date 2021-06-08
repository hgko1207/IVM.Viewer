using IVM.Studio.Mvvm;
using Prism.Ioc;

/**
 * @Class Name : DisplayControlViewModel.cs
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
    public class DisplayControlViewModel : ViewModelBase
    {
        private int currentPlayingSlider;
        public int CurrentPlayingSlider
        {
            get => currentPlayingSlider;
            set
            {
                if (SetProperty(ref currentPlayingSlider, value))
                    RaisePropertyChanged(nameof(ZSSliderPlaying));
            }
        }

        public bool ZSSliderPlaying => CurrentPlayingSlider == 0;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public DisplayControlViewModel(IContainerExtension container) : base(container)
        {
            CurrentPlayingSlider = -1;
        }
    }
}
