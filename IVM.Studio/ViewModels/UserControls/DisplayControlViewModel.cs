using IVM.Studio.MvvM;
using Prism.Ioc;

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
                {
                    RaisePropertyChanged(nameof(ZSSliderPlaying));
                }
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
