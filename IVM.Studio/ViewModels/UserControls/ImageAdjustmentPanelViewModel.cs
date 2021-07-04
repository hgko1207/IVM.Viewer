using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : ImageAdjustmentPanelViewModel.cs
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
    public class ImageAdjustmentPanelViewModel : ViewModelBase
    {
        private List<ColorChannelItem> colorChannelItems;
        public List<ColorChannelItem> ColorChannelItems
        {
            get => colorChannelItems;
            set => SetProperty(ref colorChannelItems, value);
        }

        private ColorChannelItem selectedChannel;
        public ColorChannelItem SelectedChannel
        {
            get => selectedChannel;
            set => SetProperty(ref selectedChannel, value);
        }

        private string _DAPIColor;
        public string DAPIColor
        {
            get => _DAPIColor;
            set => SetProperty(ref _DAPIColor, value);
        }

        private string _GFPColor;
        public string GFPColor
        {
            get => _GFPColor;
            set => SetProperty(ref _GFPColor, value);
        }

        private string _RFPColor;
        public string RFPColor
        {
            get => _RFPColor;
            set => SetProperty(ref _RFPColor, value);
        }

        private string _NIRColor;
        public string NIRColor
        {
            get => _NIRColor;
            set => SetProperty(ref _NIRColor, value);
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
                        new MainViewerWindow() { Topmost = true }.Show();

                        FileInfo currentFile = Container.Resolve<DataManager>().CurrentFile;
                        if (currentFile != null)
                        {
                            Metadata metadata = Container.Resolve<DataManager>().Metadata;
                            if (Container.Resolve<DataManager>().ViewerPage == new ImageViewer())
                                EventAggregator.GetEvent<DisplayImageEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                            else
                                EventAggregator.GetEvent<DisplayVideoEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                        }
                    }
                    else
                    {
                        Container.Resolve<DataManager>().MainViewerOpend = false;
                        EventAggregator.GetEvent<MainViewerCloseEvent>().Publish();
                    }
                }
            }
        }

        private bool _DAPIWindowOpend;
        public bool DAPIWindowOpend
        {
            get => _DAPIWindowOpend;
            set
            {
                if (SetProperty(ref _DAPIWindowOpend, value))
                    colorChannelInfoMap[ChannelType.DAPI].Display = value;
            }
        }

        private bool _GFPWindowOpend;
        public bool GFPWindowOpend
        {
            get => _GFPWindowOpend;
            set
            {
                if (SetProperty(ref _GFPWindowOpend, value))
                    colorChannelInfoMap[ChannelType.GFP].Display = value;
            }
        }

        private bool _RFPWindowOpend;
        public bool RFPWindowOpend
        {
            get => _RFPWindowOpend;
            set
            {
                if (SetProperty(ref _RFPWindowOpend, value))
                    colorChannelInfoMap[ChannelType.RFP].Display = value;
            }
        }

        private bool _NIRWindowOpend;
        public bool NIRWindowOpend
        {
            get => _NIRWindowOpend;
            set
            {
                if (SetProperty(ref _NIRWindowOpend, value))
                    colorChannelInfoMap[ChannelType.NIR].Display = value;
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
                        new MainHistogramWindow() { Topmost = true }.Show();
                    else
                        EventAggregator.GetEvent<HistogramCloseEvent>().Publish();
                }
            }
        }

        private bool _DAPIHistogramOpend;
        public bool DAPIHistogramOpend
        {
            get => _DAPIHistogramOpend;
            set
            {
                if (SetProperty(ref _DAPIHistogramOpend, value))
                    colorChannelInfoMap[ChannelType.DAPI].Histogram = value;
            }
        }

        private bool _GFPHistogramOpend;
        public bool GFPHistogramOpend
        {
            get => _GFPHistogramOpend;
            set
            {
                if (SetProperty(ref _GFPHistogramOpend, value))
                    colorChannelInfoMap[ChannelType.GFP].Histogram = value;
            }
        }

        private bool _RFPHistogramOpend;
        public bool RFPHistogramOpend
        {
            get => _RFPHistogramOpend;
            set
            {
                if (SetProperty(ref _RFPHistogramOpend, value))
                    colorChannelInfoMap[ChannelType.RFP].Histogram = value;
            }
        }

        private bool _NIRHistogramOpend;
        public bool NIRHistogramOpend
        {
            get => _NIRHistogramOpend;
            set
            {
                if (SetProperty(ref _NIRHistogramOpend, value))
                    colorChannelInfoMap[ChannelType.NIR].Histogram = value;
            }
        }

        public ColorChannelModel DAPIChannel { get; set; }
        public ColorChannelModel GFPChannel { get; set; }
        public ColorChannelModel RFPChannel { get; set; }
        public ColorChannelModel NIRChannel { get; set; }

        public ICommand BrightnessChangedCommand { get; private set; }
        public ICommand ContrastChangedCommand { get; private set; }

        public ICommand DAPIColorChangedCommand { get; private set; }
        public ICommand GFPColorChangedCommand { get; private set; }
        public ICommand RFPColorChangedCommand { get; private set; }
        public ICommand NIRColorChangedCommand { get; private set; }

        public ICommand ColorResetCommand { get; private set; }
        public ICommand AllVisibleCommand { get; private set; }

        public ICommand LevelLockCommand { get; private set; }
        public ICommand LevelResetCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentPanelViewModel(IContainerExtension container) : base(container)
        {
            BrightnessChangedCommand = new DelegateCommand<EditValueChangedEventArgs>(BrightnessChanged);
            ContrastChangedCommand = new DelegateCommand<EditValueChangedEventArgs>(ContrastChanged);
            DAPIColorChangedCommand = new DelegateCommand<string>(DAPIColorChanged);
            GFPColorChangedCommand = new DelegateCommand<string>(GFPColorChanged);
            RFPColorChangedCommand = new DelegateCommand<string>(RFPColorChanged);
            NIRColorChangedCommand = new DelegateCommand<string>(NIRColorChanged);
            ColorResetCommand = new DelegateCommand(ColorReset);
            AllVisibleCommand = new DelegateCommand(AllVisible);
            LevelLockCommand = new DelegateCommand(LevelLock);
            LevelResetCommand = new DelegateCommand(LevelReset);

            EventAggregator.GetEvent<SlideChangedEvent>().Subscribe(InitVisible);
            EventAggregator.GetEvent<MainViewerClosedEvent>().Subscribe(() => AllWindowOpend = false);
            EventAggregator.GetEvent<HistogramClosedEvent>().Subscribe(() => AllHistogramOpend = false);
            EventAggregator.GetEvent<ChViewerWindowClosedEvent>().Subscribe(ChWindowClosed);
            EventAggregator.GetEvent<ChHistogramWindowClosedEvent>().Subscribe(ChHistogramClosed);

            colorChannelInfoMap = container.Resolve<DataManager>().ColorChannelInfoMap;

            ColorChannelItems = Container.Resolve<DataManager>().ColorChannelItems;
            SelectedChannel = ColorChannelItems[0];

            DAPIChannel = colorChannelInfoMap[ChannelType.DAPI];
            GFPChannel = colorChannelInfoMap[ChannelType.GFP];
            RFPChannel = colorChannelInfoMap[ChannelType.RFP];
            NIRChannel = colorChannelInfoMap[ChannelType.NIR];

            InitColorStyle();
            InitVisible();
            LevelReset();
        }

        /// <summary>
        /// Color 초기화
        /// </summary>
        private void InitColorStyle()
        {
            DAPIColor = "Red";
            GFPColor = "Green";
            RFPColor = "Blue";
            NIRColor = "Alpha";
        }

        /// <summary>
        /// 밝기 변경
        /// </summary>
        /// <param name="e"></param>
        private void BrightnessChanged(EditValueChangedEventArgs e)
        {
            Thread.Sleep(500);

            if (e.NewValue is decimal value)
            {
                if (SelectedChannel.Type == ChannelType.ALL)
                {
                    bool refresh = false;
                    colorChannelInfoMap.Values.Where(item => item.ChannelType != ChannelType.ALL).ForEach(date =>
                    {
                        refresh |= date.UpdateBrightnessWithoutRefresh((float)value);
                    });
                    if (refresh)
                        EventAggregator.GetEvent<RefreshImageEvent>().Publish();
                }
                else
                    colorChannelInfoMap[SelectedChannel.Type].Brightness = (float)value;
            }
        }

        /// <summary>
        /// 대비 변경
        /// </summary>
        /// <param name="e"></param>
        private void ContrastChanged(EditValueChangedEventArgs e)
        {
            Thread.Sleep(500);
             
            if (e.NewValue is decimal value)
            {
                if (SelectedChannel.Type == ChannelType.ALL)
                {
                    bool refresh = false;
                    colorChannelInfoMap.Values.Where(item => item.ChannelType != ChannelType.ALL).ForEach(date =>
                    {
                        refresh |= date.UpdateContrastWithoutRefresh((float)value);
                    });
                    if (refresh)
                        EventAggregator.GetEvent<RefreshImageEvent>().Publish();
                }
                else
                    colorChannelInfoMap[SelectedChannel.Type].Contrast = (float)value;
            }
        }

        /// <summary>
        /// DAPI Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void DAPIColorChanged(string type)
        {
            DAPIColor = type;
            colorChannelInfoMap[ChannelType.DAPI].SetColor(type);
        }

        /// <summary>
        /// GFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void GFPColorChanged(string type)
        {
            GFPColor = type;
            colorChannelInfoMap[ChannelType.GFP].SetColor(type);
        }

        /// <summary>
        /// RFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void RFPColorChanged(string type)
        {
            RFPColor = type;
            colorChannelInfoMap[ChannelType.RFP].SetColor(type);
        }

        /// <summary>
        /// RFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void NIRColorChanged(string type)
        {
            NIRColor = type;
            colorChannelInfoMap[ChannelType.NIR].SetColor(type);
        }

        /// <summary>
        /// metadata에 저장된 pseudocolor값 그대로 읽어옴
        /// </summary>
        private void ColorReset()
        {
            InitColorStyle();

            colorChannelInfoMap[ChannelType.DAPI].SetColor("Red");
            colorChannelInfoMap[ChannelType.GFP].SetColor("Green");
            colorChannelInfoMap[ChannelType.RFP].SetColor("Blue");
            colorChannelInfoMap[ChannelType.NIR].SetColor("Alpha");
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

        /// <summary>
        /// 채널 뷰어 윈도우 종료될 때
        /// </summary>
        /// <param name="channel"></param>
        private void ChWindowClosed(int channel)
        {
            switch (channel)
            {
                case (int)ChannelType.DAPI:
                    DAPIWindowOpend = false;
                    break;
                case (int)ChannelType.GFP:
                    GFPWindowOpend = false;
                    break;
                case (int)ChannelType.RFP:
                    RFPWindowOpend = false;
                    break;
                case (int)ChannelType.NIR:
                    NIRWindowOpend = false;
                    break;
            }
        }

        /// <summary>
        /// 채널 히스토그램 윈도우 종료될 때
        /// </summary>
        /// <param name="channelType"></param>
        private void ChHistogramClosed(ChannelType channelType)
        {
            switch (channelType)
            {
                case ChannelType.DAPI:
                    DAPIHistogramOpend = false;
                    break;
                case ChannelType.GFP:
                    GFPHistogramOpend = false;
                    break;
                case ChannelType.RFP:
                    RFPHistogramOpend = false;
                    break;
                case ChannelType.NIR:
                    NIRHistogramOpend = false;
                    break;
            }
        }
    }
}
