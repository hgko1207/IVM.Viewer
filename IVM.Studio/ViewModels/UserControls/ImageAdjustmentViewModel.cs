using DevExpress.Xpf.Editors;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

/**
 * @Class Name : ImageAdjustmentViewModel.cs
 * @Description : 이미지 조정 뷰 모델
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
    public class ImageAdjustmentViewModel : ViewModelBase
    {
        private List<ColorChannelModel> channelNames;
        public List<ColorChannelModel> ChannelNames
        {
            get => channelNames;
            set => SetProperty(ref channelNames, value);
        }

        private ColorChannelModel selectedChannel;
        public ColorChannelModel SelectedChannel
        {
            get => selectedChannel;
            set => SetProperty(ref selectedChannel, value);
        }

        private bool _DAPIVisible;
        public bool DAPIVisible
        {
            get => _DAPIVisible;
            set
            {
                if (SetProperty(ref _DAPIVisible, value))
                    colorChannelInfoMap[ChannelType.DAPI].Visible = value;
            }
        }

        private bool _GFPVisible;
        public bool GFPVisible
        {
            get => _GFPVisible;
            set
            {
                if (SetProperty(ref _GFPVisible, value))
                    colorChannelInfoMap[ChannelType.GFP].Visible = value;
            }
        }

        private bool _RFPVisible;
        public bool RFPVisible
        {
            get => _RFPVisible;
            set
            {
                if (SetProperty(ref _RFPVisible, value))
                    colorChannelInfoMap[ChannelType.RFP].Visible = value;
            }
        }

        private bool _NIRVisible;
        public bool NIRVisible
        {
            get => _NIRVisible;
            set
            {
                if (SetProperty(ref _NIRVisible, value))
                    colorChannelInfoMap[ChannelType.NIR].Visible = value;
            }
        }

        private int allLevelLower;
        public int AllLevelLower
        {
            get => allLevelLower;
            set
            {
                SetProperty(ref allLevelLower, value);
                bool refresh = false;
                foreach (ColorChannelModel c in colorChannelInfoMap.Values)
                    refresh |= c.UpdateColorLevelLowerWithoutRefresh(value);
                if (refresh)
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        private int allLevelUpper;
        public int AllLevelUpper
        {
            get => allLevelUpper;
            set
            {
                SetProperty(ref allLevelUpper, value);
                bool refresh = false;
                foreach (ColorChannelModel c in colorChannelInfoMap.Values)
                    refresh |= c.UpdateColorLevelUpperWithoutRefresh(value);
                if (refresh)
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        private bool allWindowOpend;
        public bool AllWindowOpend
        {
            get => allWindowOpend;
            set
            {
                if (SetProperty(ref allWindowOpend, value))
                {
                    if (value)
                    {
                        new ImageViewerWindow() { Topmost = true }.Show();
                        FileInfo currentFile = Container.Resolve<DataManager>().CurrentFile;
                        if (currentFile != null)
                        {
                            Metadata metadata = Container.Resolve<DataManager>().Metadata;
                            EventAggregator.GetEvent<DisplayImageEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                        }
                    }
                    else
                        EventAggregator.GetEvent<ImageViewerCloseEvent>().Publish();
                }
            }
        }

        private bool allHistogramOpend;
        public bool AllHistogramOpend
        {
            get => allHistogramOpend;
            set
            {
                if (SetProperty(ref allHistogramOpend, value))
                {
                    if (value)
                        new HistogramWindow() { Topmost = true }.Show();
                    else
                        EventAggregator.GetEvent<HistogramCloseEvent>().Publish();
                }
            }
        }

        public ICommand BrightnessChangedCommand { get; set; }
        public ICommand ContrastChangedCommand { get; set; }

        public ICommand ColorResetCommand { get; set; }
        public ICommand AllVisibleCommand { get; set; }

        public ICommand LevelLockCommand { get; private set; }
        public ICommand LevelResetCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentViewModel(IContainerExtension container) : base(container)
        {
            BrightnessChangedCommand = new DelegateCommand<EditValueChangedEventArgs>(BrightnessChanged);
            ContrastChangedCommand = new DelegateCommand<EditValueChangedEventArgs>(ContrastChanged);
            ColorResetCommand = new DelegateCommand(ColorReset);
            AllVisibleCommand = new DelegateCommand(AllVisible);
            LevelLockCommand = new DelegateCommand(LevelLock);
            LevelResetCommand = new DelegateCommand(LevelReset);

            EventAggregator.GetEvent<SlideChangedEvent>().Subscribe(InitVisible);
            EventAggregator.GetEvent<ImageViewerClosedEvent>().Subscribe(() => AllWindowOpend = false);
            EventAggregator.GetEvent<HistogramClosedEvent>().Subscribe(() => AllHistogramOpend = false);

            colorChannelInfoMap = container.Resolve<DataManager>().ColorChannelInfoMap;

            ChannelNames = colorChannelInfoMap.Values.ToList();
            SelectedChannel = ChannelNames[0];

            InitVisible();
            LevelReset();
        }

        /// <summary>
        /// 밝기 변경
        /// </summary>
        /// <param name="e"></param>
        private void BrightnessChanged(EditValueChangedEventArgs e)
        {
            decimal v = (decimal)e.NewValue;
            float value = (float)(v - 50) * 0.05f;

            if (SelectedChannel.ChannelType == ChannelType.ALL)
            {
                bool refresh = false;
                foreach (ColorChannelModel i in colorChannelInfoMap.Values)
                    refresh |= i.UpdateBrightnessWithoutRefresh((float)value);
                if (refresh)
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
            else
                colorChannelInfoMap[SelectedChannel.ChannelType].Brightness = (float)value;
        }

        /// <summary>
        /// 대비 변경
        /// </summary>
        /// <param name="e"></param>
        private void ContrastChanged(EditValueChangedEventArgs e)
        {
            if (e.NewValue is decimal value)
                colorChannelInfoMap[SelectedChannel.ChannelType].Contrast = (float)value;
        }

        /// <summary>
        /// metadata에 저장된 pseudocolor값 그대로 읽어옴
        /// </summary>
        private void ColorReset()
        {
        }

        /// <summary>
        /// 모든 Ch.의 visible 버튼 ON
        /// </summary>
        private void AllVisible()
        {
            DAPIVisible = true;
            GFPVisible = true;
            RFPVisible = true;
            NIRVisible = true;
        }

        /// <summary>
        /// Histgram Level 잠금
        /// </summary>
        private void LevelLock()
        {
        }

        /// <summary>
        /// Histgram Level 초기화
        /// </summary>
        private void LevelReset()
        {
            AllLevelLower = 0;
            AllLevelUpper = 255;
        }

        /// <summary>
        /// 채널 Visible 초기화
        /// </summary>
        private void InitVisible()
        {
            DAPIVisible = colorChannelInfoMap[ChannelType.DAPI].Visible;
            GFPVisible = colorChannelInfoMap[ChannelType.GFP].Visible;
            RFPVisible = colorChannelInfoMap[ChannelType.RFP].Visible;
            NIRVisible = colorChannelInfoMap[ChannelType.NIR].Visible;
        }
    }
}
