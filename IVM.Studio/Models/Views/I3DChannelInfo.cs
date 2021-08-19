using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System.Windows.Input;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Models
{
    public class I3DChannelInfo : BindableBase
    {
        I3DWcfServer wcfserver;

        int channelId = -1;

        private string _DAPIColor = "Red";
        public string DAPIColor
        {
            get => _DAPIColor;
            set => SetProperty(ref _DAPIColor, value);
        }

        private string _GFPColor = "Green";
        public string GFPColor
        {
            get => _GFPColor;
            set => SetProperty(ref _GFPColor, value);
        }

        private string _RFPColor = "Blue";
        public string RFPColor
        {
            get => _RFPColor;
            set => SetProperty(ref _RFPColor, value);
        }

        private string _NIRColor = "None";
        public string NIRColor
        {
            get => _NIRColor;
            set => SetProperty(ref _NIRColor, value);
        }

        private bool _DAPIVisible = true;
        public bool DAPIVisible
        {
            get => _DAPIVisible;
            set
            {
                if (SetProperty(ref _DAPIVisible, value))
                {
                    wcfserver.Channel(channelId).OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _GFPVisible = true;
        public bool GFPVisible
        {
            get => _GFPVisible;
            set
            {
                if (SetProperty(ref _GFPVisible, value))
                {
                    wcfserver.Channel(channelId).OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _RFPVisible = true;
        public bool RFPVisible
        {
            get => _RFPVisible;
            set
            {
                if (SetProperty(ref _RFPVisible, value))
                {
                    wcfserver.Channel(channelId).OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _NIRVisible = false;
        public bool NIRVisible
        {
            get => _NIRVisible;
            set
            {
                if (SetProperty(ref _NIRVisible, value))
                {
                    wcfserver.Channel(channelId).OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        public ICommand DAPIColorChangedCommand { get; private set; }
        public ICommand GFPColorChangedCommand { get; private set; }
        public ICommand RFPColorChangedCommand { get; private set; }
        public ICommand NIRColorChangedCommand { get; private set; }

        int StrToBandIdx(string col)
        {
            switch (col)
            {
                case "Red":
                    return 0;
                case "Green":
                    return 1;
                case "Blue":
                    return 2;
            }
            return 3;
        }

        public I3DChannelInfo(IContainerExtension container, IEventAggregator eventAggregator, int channelId)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            this.channelId = channelId;

            DAPIColorChangedCommand = new DelegateCommand<string>(DAPIColorChanged);
            GFPColorChangedCommand = new DelegateCommand<string>(GFPColorChanged);
            RFPColorChangedCommand = new DelegateCommand<string>(RFPColorChanged);
            NIRColorChangedCommand = new DelegateCommand<string>(NIRColorChanged);
        }

        private void DAPIColorChanged(string col)
        {
            DAPIColor = col;

            wcfserver.Channel(channelId).OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }

        private void GFPColorChanged(string col)
        {
            GFPColor = col;

            wcfserver.Channel(channelId).OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }

        private void RFPColorChanged(string col)
        {
            RFPColor = col;

            wcfserver.Channel(channelId).OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }

        private void NIRColorChanged(string col)
        {
            NIRColor = col;

            wcfserver.Channel(channelId).OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }
    }
}
