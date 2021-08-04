using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
    public class ImageAdjustmentPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ImageAdjustmentPanel>
{
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
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
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
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
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
                        MainViewerWindow mainViewerWindow = new MainViewerWindow() { Owner = Application.Current.MainWindow, WindowId = ++dataManager.MainWindowSeq };
                        mainViewerWindow.Show();

                        FileInfo currentFile = dataManager.CurrentFile;
                        if (currentFile != null)
                        {
                            Metadata metadata = dataManager.Metadata;
                            if (dataManager.ViewerName == nameof(ImageViewer))
                                EventAggregator.GetEvent<DisplayImageEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                            else
                                EventAggregator.GetEvent<DisplayVideoEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                        }
                    }
                    else
                    {
                        dataManager.MainViewerOpend = false;
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

        private bool isLockHistogram;
        public bool IsLockHistogram
        {
            get => isLockHistogram;
            set => SetProperty(ref isLockHistogram, value);
        }

        public ColorChannelModel DAPIColorChannel { get; set; }
        public ColorChannelModel GFPColorChannel { get; set; }
        public ColorChannelModel RFPColorChannel { get; set; }
        public ColorChannelModel NIRColorChannel { get; set; }

        public ICommand DAPIColorChangedCommand { get; private set; }
        public ICommand GFPColorChangedCommand { get; private set; }
        public ICommand RFPColorChangedCommand { get; private set; }
        public ICommand NIRColorChangedCommand { get; private set; }

        public ICommand ColorResetCommand { get; private set; }
        public ICommand AllVisibleCommand { get; private set; }

        public ICommand ResetHistogramCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentPanelViewModel(IContainerExtension container) : base(container)
        {
            DAPIColorChangedCommand = new DelegateCommand<string>(DAPIColorChanged);
            GFPColorChangedCommand = new DelegateCommand<string>(GFPColorChanged);
            RFPColorChangedCommand = new DelegateCommand<string>(RFPColorChanged);
            NIRColorChangedCommand = new DelegateCommand<string>(NIRColorChanged);
            ColorResetCommand = new DelegateCommand(ColorReset);
            AllVisibleCommand = new DelegateCommand(AllVisible);
            ResetHistogramCommand = new DelegateCommand(ResetHistogram);

            EventAggregator.GetEvent<RefreshMetadataEvent>().Subscribe(RefreshMetadata, ThreadOption.UIThread);
            EventAggregator.GetEvent<MainViewerClosedEvent>().Subscribe(() => AllWindowOpend = false);
            EventAggregator.GetEvent<HistogramClosedEvent>().Subscribe(() => AllHistogramOpend = false);
            EventAggregator.GetEvent<ChViewerWindowClosedEvent>().Subscribe(ChWindowClosed);
            EventAggregator.GetEvent<ChHistogramWindowClosedEvent>().Subscribe(ChHistogramClosed);

            dataManager = container.Resolve<DataManager>();
            colorChannelInfoMap = dataManager.ColorChannelInfoMap;

            DAPIColorChannel = colorChannelInfoMap[ChannelType.DAPI];
            GFPColorChannel = colorChannelInfoMap[ChannelType.GFP];
            RFPColorChannel = colorChannelInfoMap[ChannelType.RFP];
            NIRColorChannel = colorChannelInfoMap[ChannelType.NIR];

            RefreshColorStyle();
            RefreshVisible();
            ResetHistogram();

            IsLockHistogram = true;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ImageAdjustmentPanel view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ImageAdjustmentPanel view)
        {
            EventAggregator.GetEvent<RefreshMetadataEvent>().Unsubscribe(RefreshMetadata);
            EventAggregator.GetEvent<MainViewerClosedEvent>().Unsubscribe(() => AllWindowOpend = false);
            EventAggregator.GetEvent<HistogramClosedEvent>().Unsubscribe(() => AllHistogramOpend = false);
            EventAggregator.GetEvent<ChViewerWindowClosedEvent>().Unsubscribe(ChWindowClosed);
            EventAggregator.GetEvent<ChHistogramWindowClosedEvent>().Unsubscribe(ChHistogramClosed);
        }

        /// <summary>
        /// Color 초기화
        /// </summary>
        private void RefreshColorStyle()
        {
            DAPIColor = ConvertColorToString(DAPIColorChannel.Color);
            GFPColor = ConvertColorToString(GFPColorChannel.Color);
            RFPColor = ConvertColorToString(RFPColorChannel.Color);
            NIRColor = ConvertColorToString(NIRColorChannel.Color);
        }

        /// <summary>
        /// DAPI Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void DAPIColorChanged(string type)
        {
            ChangedNoneColor(type);

            DAPIColor = type;
            DAPIColorChannel.SetColor(type);
        }

        /// <summary>
        /// GFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void GFPColorChanged(string type)
        {
            ChangedNoneColor(type);

            GFPColor = type;
            GFPColorChannel.SetColor(type);
        }

        /// <summary>
        /// RFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void RFPColorChanged(string type)
        {
            ChangedNoneColor(type);

            RFPColor = type;
            RFPColorChannel.SetColor(type);
        }

        /// <summary>
        /// RFP Color 변경 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void NIRColorChanged(string type)
        {
            ChangedNoneColor(type);

            NIRColor = type;
            NIRColorChannel.SetColor(type);
        }

        /// <summary>
        /// 컬러 선택 시 다른 채널에 컬러가 있을 시 다른 채널의 컬러를 None으로 변환
        /// </summary>
        /// <param name="color"></param>
        private void ChangedNoneColor(string color)
        {
            ColorChannelModel channelModel = colorChannelInfoMap.Values.SingleOrDefault(item => item.Color == ConvertMetadataToColor(color));
            if (channelModel != null)
            {
                channelModel.Color = Colors.None;
                ConvertChannelColor(channelModel.ChannelType);
            }
        }

        /// <summary>
        /// metadata에 저장된 pseudocolor값 그대로 읽어옴
        /// </summary>
        private void ColorReset()
        {
            RefreshMetadata(new DisplayParam(null, dataManager.Metadata, true));
        }

        /// <summary>
        /// 메타데이터 변경 시
        /// </summary>
        /// <param name="param"></param>
        private void RefreshMetadata(DisplayParam param)
        {
            Metadata metadata = param.Metadata;
            if (metadata != null)
            {
                DAPIColorChannel.Color = metadata.ChA == "0" ? Colors.Red : ConvertMetadataToColor(metadata.ChA);
                GFPColorChannel.Color = metadata.ChB == "0" ? Colors.Green : ConvertMetadataToColor(metadata.ChB);
                RFPColorChannel.Color = metadata.ChC == "0" ? Colors.Blue : ConvertMetadataToColor(metadata.ChC);
                NIRColorChannel.Color = metadata.ChD == "0" ? Colors.Alpha : ConvertMetadataToColor(metadata.ChD);
            }

            RefreshColorStyle();

            if (param.SlideChanged)
                RefreshVisible();

            if (!IsLockHistogram)
                ResetHistogram();
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
        /// Histgram Level 초기화
        /// </summary>
        private void ResetHistogram()
        {
            AllLevelLower = 0;
            AllLevelUpper = 255;
        }

        /// <summary>
        /// VisibleFromColor
        /// </summary>
        private void VisibleFromColor()
        {
            DAPIColorChannel.Visible = DAPIColor == "Alpha" ? false : true;
            GFPColorChannel.Visible = GFPColor == "Alpha" ? false : true;
            RFPColorChannel.Visible = RFPColor == "Alpha" ? false : true;
            NIRColorChannel.Visible = NIRColor == "Alpha" ? false : true;
        }

        /// <summary>
        /// 채널 Visible 초기화
        /// </summary>
        private void RefreshVisible()
        {
            VisibleFromColor();

            DAPIVisible = DAPIColorChannel.Visible;
            GFPVisible = GFPColorChannel.Visible;
            RFPVisible = RFPColorChannel.Visible;
            NIRVisible = NIRColorChannel.Visible;
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

        /// <summary>
        /// Metadata Color를 색상으로 변환
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Colors ConvertMetadataToColor(string color)
        {
            switch (color)
            {
                case "Red":
                case "R":
                    return Colors.Red;
                case "Green":
                case "G":
                    return Colors.Green;
                case "Blue":
                case "B":
                    return Colors.Blue;
                case "Alpha":
                case "A":
                    return Colors.Alpha;
                default:
                    return Colors.None;
            }
        }

        /// <summary>
        /// Color를 string으로 변환
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private string ConvertColorToString(Colors color)
        {
            switch (color)
            {
                case Colors.Red:
                    return "Red";
                case Colors.Green:
                    return "Green";
                case Colors.Blue:
                    return "Blue";
                case Colors.Alpha:
                    return "Alpha";
                default:
                    return "None";
            }
        }

        /// <summary>
        /// 채널 컬러를 None으로 변환
        /// </summary>
        /// <param name="type"></param>
        /// <param name="color"></param>
        private void ConvertChannelColor(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.DAPI:
                    DAPIColor = "None";
                    break;
                case ChannelType.GFP:
                    GFPColor = "None";
                    break;
                case ChannelType.RFP:
                    RFPColor = "None";
                    break;
                case ChannelType.NIR:
                    NIRColor = "None";
                    break;
            }
        }
    }
}
