using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

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
        public bool ZSSliderEnabled => ZSSliderMaximum > 1;
        public bool MSSliderEnabled => MSSliderMaximum > 1;
        public bool MPSliderEnabled => MPSliderMaximum > 1;
        public bool TLSliderEnabled => TLSliderMaximum > 1;

        //////////////////////////////////////////////////////////////////////////////////////////

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

        //////////////////////////////////////////////////////////////////////////////////////////

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

        //////////////////////////////////////////////////////////////////////////////////////////

        public string ZSSliderText => $"{ZSSliderValue}/{ZSSliderMaximum}";
        public string MSSliderText => $"{MSSliderValue}/{MSSliderMaximum}";
        public string MPSliderText => $"{MPSliderValue}/{MPSliderMaximum}";
        public string TLSliderText => $"{TLSliderValue}/{TLSliderMaximum}";

        public string ZSSliderRangeText => $"{ZSSliderStart}/{ZSSliderEnd}";
        public string MSSliderRangeText => $"{MSSliderStart}/{MSSliderEnd}";
        public string MPSliderRangeText => $"{MPSliderStart}/{MPSliderEnd}";
        public string TLSliderRangeText => $"{TLSliderStart}/{TLSliderEnd}";

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

        //////////////////////////////////////////////////////////////////////////////////////////

        private int _ZSSliderStart;
        public int ZSSliderStart
        {
            get => _ZSSliderStart;
            set 
            {
                if (SetProperty(ref _ZSSliderStart, value))
                    RaisePropertyChanged(nameof(ZSSliderRangeText));
            }
        }
        private int _MSSliderStart;
        public int MSSliderStart
        {
            get => _MSSliderStart;
            set
            {
                if (SetProperty(ref _MSSliderStart, value))
                    RaisePropertyChanged(nameof(MSSliderRangeText));
            }
        }
        private int _MPSliderStart;
        public int MPSliderStart
        {
            get => _MPSliderStart;
            set
            {
                if (SetProperty(ref _MPSliderStart, value))
                    RaisePropertyChanged(nameof(MPSliderRangeText));
            }
        }
        private int _TLSliderStart;
        public int TLSliderStart
        {
            get => _TLSliderStart;
            set
            {
                if (SetProperty(ref _TLSliderStart, value))
                    RaisePropertyChanged(nameof(TLSliderRangeText));
            }
        }

        private int _ZSSliderEnd;
        public int ZSSliderEnd
        {
            get => _ZSSliderEnd;
            set
            {
                if (SetProperty(ref _ZSSliderEnd, value))
                    RaisePropertyChanged(nameof(ZSSliderRangeText));
            }
        }
        private int _MSSliderEnd;
        public int MSSliderEnd
        {
            get => _MSSliderEnd;
            set
            {
                if (SetProperty(ref _MSSliderEnd, value))
                    RaisePropertyChanged(nameof(MSSliderRangeText));
            }
        }
        private int _MPSliderEnd;
        public int MPSliderEnd
        {
            get => _MPSliderEnd;
            set
            {
                if (SetProperty(ref _MPSliderEnd, value))
                    RaisePropertyChanged(nameof(MPSliderRangeText));
            }
        }
        private int _TLSliderEnd;
        public int TLSliderEnd
        {
            get => _TLSliderEnd;
            set
            {
                if (SetProperty(ref _TLSliderEnd, value))
                    RaisePropertyChanged(nameof(TLSliderRangeText));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////

        private bool disableSlidersEvent;
        public bool DisableSlidersEvent
        {
            get => disableSlidersEvent;
            set => SetProperty(ref disableSlidersEvent, value);
        }

        private double slideShowFps;
        public double SlideShowFps
        {
            get => slideShowFps;
            set => SetProperty(ref slideShowFps, value);
        }

        private int slideShowRepeat;
        public int SlideShowRepeat
        {
            get => slideShowRepeat;
            set => SetProperty(ref slideShowRepeat, value);
        }

        private int _ZStackProjLowerIndex;
        public int ZStackProjLowerIndex
        {
            get => _ZStackProjLowerIndex;
            set => SetProperty(ref _ZStackProjLowerIndex, value);
        }

        private int _ZStackProjUpperIndex;
        public int ZStackProjUpperIndex
        {
            get => _ZStackProjUpperIndex;
            set => SetProperty(ref _ZStackProjUpperIndex, value);
        }

        public bool ZStackProjEnabled => ZSSliderEnabled && dataManager.ViewerName == nameof(ImageViewer);
        public bool MosaicEnabled => MSSliderEnabled && dataManager.ViewerName == nameof(ImageViewer);

        public ICommand PlaySlideShowCommand { get; private set; }

        private IContainerExtension container;
        private IEventAggregator eventAggregator;

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="eventAggregator"></param>
        public SliderControlInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.container = container;
            this.eventAggregator = eventAggregator;

            PlaySlideShowCommand = new DelegateCommand<string>(PlaySlideShow);

            eventAggregator.GetEvent<PlaySlideShowEvent>().Subscribe(InternalPlaySlideShow);
            eventAggregator.GetEvent<StopSlideShowEvent>().Subscribe(StopSlideShow);
            eventAggregator.GetEvent<EnableImageSlidersEvent>().Subscribe(EnableImageSliders);
            //eventAggregator.GetEvent<MainViewerOpendEvent>().Subscribe(SliderStateChanged);
            eventAggregator.GetEvent<MainViewerClosedEvent>().Subscribe(StopSlideShow);
            eventAggregator.GetEvent<ViewerPageChangedEvent>().Subscribe(ViewerPageChanged);

            dataManager = container.Resolve<DataManager>();

            CurrentPlayingSlider = -1;
            SlideShowFps = 5;
            SlideShowRepeat = 2;
        }

        /// <summary>
        /// 현재 폴더에서의 슬라이드 목록을 새로고침 합니다.
        /// </summary>
        /// <param name="param"></param>
        private void EnableImageSliders(SlidersParam param)
        {
            string currentSlidesPath = param.CurrentSlidesPath;
            string slideName = param.SlideName;

            DisableSlidersEvent = true;

            if (dataManager.ApprovedExtensions.Any(s => slideName.EndsWith(s)))
            {
                ZSSliderValue = 0;
                ZSSliderMaximum = 0;
                ZSSliderStart = 0;
                ZSSliderEnd = 0;

                MSSliderValue = 0;
                MSSliderMaximum = 0;
                MSSliderStart = 0;
                MSSliderEnd = 0;

                MPSliderValue = 0;
                MPSliderMaximum = 0;
                MPSliderStart = 0;
                MPSliderEnd = 0;

                TLSliderValue = 0;
                TLSliderMaximum = 0;
                TLSliderStart = 0;
                TLSliderEnd = 0;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(currentSlidesPath, slideName));
                var (tlCount, mpCount, msCount, zsCount) = container.Resolve<FileService>().GetImagesModeStatus(di, dataManager.ApprovedExtensions);

                ZSSliderMinimum = zsCount == 0 ? 0 : 1;
                ZSSliderMaximum = zsCount;
                ZSSliderValue = zsCount == 0 ? 0 : 1;
                ZSSliderStart = zsCount == 0 ? 0 : 1;
                ZSSliderEnd = zsCount;

                MSSliderMinimum = msCount == 0 ? 0 : 1;
                MSSliderMaximum = msCount;
                MSSliderValue = msCount == 0 ? 0 : 1;
                MSSliderStart = msCount == 0 ? 0 : 1;
                MSSliderEnd = msCount;

                MPSliderMinimum = mpCount == 0 ? 0 : 1;
                MPSliderMaximum = mpCount;
                MPSliderValue = mpCount == 0 ? 0 : 1;
                MPSliderStart = mpCount == 0 ? 0 : 1;
                MPSliderEnd = mpCount;

                TLSliderMinimum = tlCount == 0 ? 0 : 1;
                TLSliderMaximum = tlCount;
                TLSliderValue = tlCount == 0 ? 0 : 1;
                TLSliderStart = tlCount == 0 ? 0 : 1;
                TLSliderEnd = tlCount;

                ZStackProjLowerIndex = 1;
                ZStackProjUpperIndex = zsCount;
            }

            DisableSlidersEvent = false;
        }

        /// <summary>
        /// SlideShow Play
        /// </summary>
        /// <param name="type"></param>
        private void PlaySlideShow(string type)
        {
            container.Resolve<SlideShowService>().StopSlideShow();

            switch (type)
            {
                case "ZStack":
                    if (CurrentPlayingSlider != 0 && ZSSliderEnabled && (ZSSliderEnd - ZSSliderStart) >= 2)
                    {
                        container.Resolve<SlideShowService>().StartSlideShow(SlideShowFps, ZSSliderEnd - ZSSliderStart, SlideShowRepeat);

                        // 현재 슬라이드 값이 1이 아닌 경우: 1로 이동시키면 재생 시작
                        // 현재 슬라이드 값이 1인 경우: 이동시킬 필요 없으므로 바로 재생 시작
                        if (ZSSliderValue == ZSSliderStart)
                        {
                            if (dataManager.ViewerName == nameof(ImageViewer))
                                container.Resolve<SlideShowService>().ContinueSlideShow();
                            else
                                eventAggregator.GetEvent<PlayVideoEvent>().Publish();
                        }
                        else
                            ZSSliderValue = ZSSliderStart;

                        CurrentPlayingSlider = 0;
                    }
                    else
                        CurrentPlayingSlider = -1;
                    break;
                case "Mosaic":
                    if (CurrentPlayingSlider != 1 && MSSliderEnabled && (MSSliderEnd - MSSliderStart) >= 2)
                    {
                        container.Resolve<SlideShowService>().StartSlideShow(SlideShowFps, MSSliderEnd - MSSliderStart, SlideShowRepeat);
                        if (MSSliderValue == MSSliderStart)
                        {
                            if (dataManager.ViewerName == nameof(ImageViewer))
                                container.Resolve<SlideShowService>().ContinueSlideShow();
                            else
                                eventAggregator.GetEvent<PlayVideoEvent>().Publish();
                        }
                        else
                            MSSliderValue = MSSliderStart;

                        CurrentPlayingSlider = 1;
                    }
                    else
                        CurrentPlayingSlider = -1;
                    break;
                case "MultiPosition":
                    if (CurrentPlayingSlider != 2 && MPSliderEnabled && (MPSliderEnd - MPSliderStart) >= 2)
                    {
                        container.Resolve<SlideShowService>().StartSlideShow(SlideShowFps, MPSliderEnd - MPSliderStart, SlideShowRepeat);
                        if (MPSliderValue == MPSliderStart)
                        {
                            if (dataManager.ViewerName == nameof(ImageViewer))
                                container.Resolve<SlideShowService>().ContinueSlideShow();
                            else
                                eventAggregator.GetEvent<PlayVideoEvent>().Publish();
                        }
                        else
                            MPSliderValue = MPSliderStart;

                        CurrentPlayingSlider = 2;
                    }
                    else
                        CurrentPlayingSlider = -1;
                    break;
                case "TimeLapse":
                    if (CurrentPlayingSlider != 3 && TLSliderEnabled && (TLSliderEnd - TLSliderStart) >= 2)
                    {
                        container.Resolve<SlideShowService>().StartSlideShow(SlideShowFps, TLSliderEnd - TLSliderStart, SlideShowRepeat);
                        if (TLSliderValue == TLSliderStart)
                        {
                            if (dataManager.ViewerName == nameof(ImageViewer))
                                container.Resolve<SlideShowService>().ContinueSlideShow();
                            else
                                eventAggregator.GetEvent<PlayVideoEvent>().Publish();
                        }
                        else
                            TLSliderValue = TLSliderStart;

                        CurrentPlayingSlider = 3;
                    }
                    else
                        CurrentPlayingSlider = -1;
                    break;
            }
        }

        /// <summary>
        /// InternalPlaySlideShow
        /// </summary>
        private void InternalPlaySlideShow()
        {
            switch (CurrentPlayingSlider)
            {
                case 0: // ZStack
                    if (ZSSliderValue == ZSSliderEnd)
                    {
                        ZSSliderValue = ZSSliderStart;
                    }
                    else
                    {
                        ZSSliderValue++;
                        if (!container.Resolve<SlideShowService>().NowPlaying && ZSSliderValue == ZSSliderEnd)
                            CurrentPlayingSlider = -1;
                    }
                    break;
                case 1: // Mosaic
                    if (MSSliderValue == MSSliderEnd)
                    {
                        MSSliderValue = MSSliderStart;
                    }
                    else
                    {
                        MSSliderValue++;
                        if (!container.Resolve<SlideShowService>().NowPlaying && MSSliderValue == MSSliderEnd)
                            CurrentPlayingSlider = -1;
                    }
                    break;
                case 2: // MultiPosition
                    if (MPSliderValue == MPSliderEnd)
                    {
                        MPSliderValue = MPSliderStart;
                    }
                    else
                    {
                        MPSliderValue++;
                        if (!container.Resolve<SlideShowService>().NowPlaying && MPSliderValue == MPSliderEnd)
                            CurrentPlayingSlider = -1;
                    }
                    break;
                case 3: // TimeLapse
                    if (TLSliderValue == TLSliderEnd)
                    {
                        TLSliderValue = TLSliderStart;
                    }
                    else
                    {
                        TLSliderValue++;
                        if (!container.Resolve<SlideShowService>().NowPlaying && TLSliderValue == TLSliderEnd)
                            CurrentPlayingSlider = -1;
                    }
                    break;
            }
        }

        /// <summary>
        /// StopSlideshow
        /// </summary>
        private void StopSlideShow()
        {
            container.Resolve<SlideShowService>().StopSlideShow();
            CurrentPlayingSlider = -1;
        }


        /// <summary>
        /// 메인 뷰어 변경 시
        /// </summary>
        private void ViewerPageChanged()
        {
            RaisePropertyChanged(nameof(ZStackProjEnabled));
            RaisePropertyChanged(nameof(MosaicEnabled));
        }
    }
}
