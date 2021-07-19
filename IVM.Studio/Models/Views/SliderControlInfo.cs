using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

/**
* @Class Name : SliderControlInfo.cs
* @Description : Slider 제어 모델
* @
* @ 수정일         수정자              수정내용
* @ ----------   ---------   -------------------------------
* @ 2021.07.05     고형균              최초생성
*
* @author 고형균
* @since 2021.07.05
* @version 1.0
*/
namespace IVM.Studio.Models.Views
{
    public class SliderControlInfo : BindableBase
    {
        public bool ZSSliderEnabled => ZSSliderMaximum > 1 && dataManager.MainViewerOpend;
        public bool MSSliderEnabled => MSSliderMaximum > 1 && dataManager.MainViewerOpend;
        public bool MPSliderEnabled => MPSliderMaximum > 1 && dataManager.MainViewerOpend;
        public bool TLSliderEnabled => TLSliderMaximum > 1 && dataManager.MainViewerOpend;

        private int _ZSSliderMinimum;
        public int ZSSliderMinimum
        {
            get => _ZSSliderMinimum;
            set => SetProperty(ref _ZSSliderMinimum, value);
        }
        private int _MSSliderMinimum;
        public int MSSliderMinimum
        {
            get => _MSSliderMinimum;
            set => SetProperty(ref _MSSliderMinimum, value);
        }
        private int _MPSliderMinimum;
        public int MPSliderMinimum
        {
            get => _MPSliderMinimum;
            set => SetProperty(ref _MPSliderMinimum, value);
        }
        private int _TLSliderMinimum;
        public int TLSliderMinimum
        {
            get => _TLSliderMinimum;
            set => SetProperty(ref _TLSliderMinimum, value);
        }

        private int _ZSSliderMaximum;
        public int ZSSliderMaximum
        {
            get => _ZSSliderMaximum;
            set
            {
                if (SetProperty(ref _ZSSliderMaximum, value))
                {
                    RaisePropertyChanged(nameof(ZSSliderText));
                    RaisePropertyChanged(nameof(ZSSliderEnabled));
                }
            }
        }
        private int _MSSliderMaximum;
        public int MSSliderMaximum
        {
            get => _MSSliderMaximum;
            set
            {
                if (SetProperty(ref _MSSliderMaximum, value))
                {
                    RaisePropertyChanged(nameof(MSSliderText));
                    RaisePropertyChanged(nameof(MSSliderEnabled));
                }
            }
        }
        private int _MPSliderMaximum;
        public int MPSliderMaximum
        {
            get => _MPSliderMaximum;
            set
            {
                if (SetProperty(ref _MPSliderMaximum, value))
                {
                    RaisePropertyChanged(nameof(MPSliderText));
                    RaisePropertyChanged(nameof(MPSliderEnabled));
                }
            }
        }
        private int _TLSliderMaximum;
        public int TLSliderMaximum
        {
            get => _TLSliderMaximum;
            set
            {
                if (SetProperty(ref _TLSliderMaximum, value))
                {
                    RaisePropertyChanged(nameof(TLSliderText));
                    RaisePropertyChanged(nameof(TLSliderEnabled));
                }
            }
        }

        private int _ZSSliderValue;
        public int ZSSliderValue
        {
            get => _ZSSliderValue;
            set
            {
                if (SetProperty(ref _ZSSliderValue, value))
                {
                    RaisePropertyChanged(nameof(ZSSliderText));
                    if (!DisableSlidersEvent)
                        eventAggregator.GetEvent<DisplaySlideEvent>().Publish(false);
                }
            }
        }
        private int _MSSliderValue;
        public int MSSliderValue
        {
            get => _MSSliderValue;
            set
            {
                if (SetProperty(ref _MSSliderValue, value))
                {
                    RaisePropertyChanged(nameof(MSSliderText));
                    if (!DisableSlidersEvent)
                        eventAggregator.GetEvent<DisplaySlideEvent>().Publish(false);
                }
            }
        }
        private int _MPSliderValue;
        public int MPSliderValue
        {
            get => _MPSliderValue;
            set
            {
                if (SetProperty(ref _MPSliderValue, value))
                {
                    RaisePropertyChanged(nameof(MPSliderText));
                    if (!DisableSlidersEvent)
                        eventAggregator.GetEvent<DisplaySlideEvent>().Publish(false);
                }
            }
        }
        private int _TLSliderValue;
        public int TLSliderValue
        {
            get => _TLSliderValue;
            set
            {
                if (SetProperty(ref _TLSliderValue, value))
                {
                    RaisePropertyChanged(nameof(TLSliderText));
                    if (!DisableSlidersEvent)
                        eventAggregator.GetEvent<DisplaySlideEvent>().Publish(false);
                }
            }
        }

        public string ZSSliderText => $"{ZSSliderValue}/{ZSSliderMaximum}";
        public string MSSliderText => $"{MSSliderValue}/{MSSliderMaximum}";
        public string MPSliderText => $"{MPSliderValue}/{MPSliderMaximum}";
        public string TLSliderText => $"{TLSliderValue}/{TLSliderMaximum}";

        private int currentPlayingSlider;
        public int CurrentPlayingSlider
        {
            get => currentPlayingSlider;
            set
            {
                SetProperty(ref currentPlayingSlider, value);
                RaisePropertyChanged(nameof(ZSSliderPlaying));
                RaisePropertyChanged(nameof(MSSliderPlaying));
                RaisePropertyChanged(nameof(MPSliderPlaying));
                RaisePropertyChanged(nameof(TLSliderPlaying));
            }
        }

        public bool ZSSliderPlaying => CurrentPlayingSlider == 0;
        public bool MSSliderPlaying => CurrentPlayingSlider == 1;
        public bool MPSliderPlaying => CurrentPlayingSlider == 2;
        public bool TLSliderPlaying => CurrentPlayingSlider == 3;

        private bool disableSlidersEvent;
        public bool DisableSlidersEvent
        {
            get => disableSlidersEvent;
            set => SetProperty(ref disableSlidersEvent, value);
        }

        private IContainerExtension container;
        private IEventAggregator eventAggregator;

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="eventAggregator"></param>
        public SliderControlInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            dataManager = container.Resolve<DataManager>();

            CurrentPlayingSlider = -1;
        }
    }
}
