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
                    I3DChangedChannelVisibleParam p = new I3DChangedChannelVisibleParam();
                    p.DAPI = _DAPIVisible; p.GFP = _GFPVisible; p.RFP = _RFPVisible; p.NIR = _NIRVisible;
                    eventAggregator.GetEvent<I3DChangedChannelVisibleEvent>().Publish(p);
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
                    I3DChangedChannelVisibleParam p = new I3DChangedChannelVisibleParam();
                    p.DAPI = _DAPIVisible; p.GFP = _GFPVisible; p.RFP = _RFPVisible; p.NIR = _NIRVisible;
                    eventAggregator.GetEvent<I3DChangedChannelVisibleEvent>().Publish(p);
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
                    I3DChangedChannelVisibleParam p = new I3DChangedChannelVisibleParam();
                    p.DAPI = _DAPIVisible; p.GFP = _GFPVisible; p.RFP = _RFPVisible; p.NIR = _NIRVisible;
                    eventAggregator.GetEvent<I3DChangedChannelVisibleEvent>().Publish(p);
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
                    I3DChangedChannelVisibleParam p = new I3DChangedChannelVisibleParam();
                    p.DAPI = _DAPIVisible; p.GFP = _GFPVisible; p.RFP = _RFPVisible; p.NIR = _NIRVisible;
                    eventAggregator.GetEvent<I3DChangedChannelVisibleEvent>().Publish(p);
                }
            }
        }

        private IEventAggregator eventAggregator;

        public I3DChannelInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }
    }
}
