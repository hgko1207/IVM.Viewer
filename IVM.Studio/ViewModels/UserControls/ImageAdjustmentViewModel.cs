using DevExpress.Xpf.Editors;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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
                    ColorChannelInfoMap[ChannelType.DAPI].Visible = value;
            }
        }

        private bool _GFPVisible;
        public bool GFPVisible
        {
            get => _GFPVisible;
            set
            {
                if (SetProperty(ref _GFPVisible, value))
                    ColorChannelInfoMap[ChannelType.GFP].Visible = value;
            }
        }

        private bool _RFPVisible;
        public bool RFPVisible
        {
            get => _RFPVisible;
            set
            {
                if (SetProperty(ref _RFPVisible, value))
                    ColorChannelInfoMap[ChannelType.RFP].Visible = value;
            }
        }

        private bool _NIRVisible;
        public bool NIRVisible
        {
            get => _NIRVisible;
            set
            {
                if (SetProperty(ref _NIRVisible, value))
                    ColorChannelInfoMap[ChannelType.NIR].Visible = value;
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
                        new ImageViewerWindow().Show();
                    else
                        EventAggregator.GetEvent<MainViewerCloseEvent>().Publish();
                }
            }
        }

        public ICommand BrightnessChangedCommand { get; set; }
        public ICommand ContrastChangedCommand { get; set; }

        public ICommand ColorResetCommand { get; set; }
        public ICommand AllVisibleCommand { get; set; }

        public ICommand LevelLockCommand { get; private set; }
        public ICommand LevelResetCommand { get; private set; }

        public Dictionary<ChannelType, ColorChannelModel> ColorChannelInfoMap { get; set; }

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

            ColorChannelInfoMap = container.Resolve<DataManager>().ColorChannelInfoMap;

            ChannelNames = ColorChannelInfoMap.Values.ToList();
            SelectedChannel = ChannelNames[0];

            DAPIVisible = ColorChannelInfoMap[ChannelType.DAPI].Visible;
            GFPVisible = ColorChannelInfoMap[ChannelType.GFP].Visible;
            RFPVisible = ColorChannelInfoMap[ChannelType.RFP].Visible;
            NIRVisible = ColorChannelInfoMap[ChannelType.NIR].Visible;
        }

        /// <summary>
        /// 밝기 변경
        /// </summary>
        /// <param name="e"></param>
        private void BrightnessChanged(EditValueChangedEventArgs e)
        {
            if (e.NewValue is decimal value)
                ColorChannelInfoMap[SelectedChannel.ChannelType].Brightness = (float)value;
        }

        /// <summary>
        /// 대비 변경
        /// </summary>
        /// <param name="e"></param>
        private void ContrastChanged(EditValueChangedEventArgs e)
        {
            if (e.NewValue is decimal value)
                ColorChannelInfoMap[SelectedChannel.ChannelType].Contrast = (float)value;
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
        /// 잠금
        /// </summary>
        private void LevelLock()
        {

        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void LevelReset()
        {

        }
    }
}
