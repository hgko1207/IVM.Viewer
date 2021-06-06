using IVM.Studio.Models;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Services
{
    public class DataManager
    {
        public SlideInfo SelectedSlideInfo { get; set; }

        public Dictionary<ChannelType, ColorChannelModel> ColorChannelInfoMap { get; set; }

        public ColorChannelModel CurrentSelectedChannel { get; set; }

        public WPFDrawing.ImageSource HistogramImage { get; set; }

        public void Init(IContainerExtension container, IEventAggregator eventAggregator)
        {
            ColorChannelInfoMap = new Dictionary<ChannelType, ColorChannelModel>();
            ColorChannelInfoMap.Add(ChannelType.DAPI, new ColorChannelModel(ChannelType.DAPI, "DAPI (425-465)", true, Colors.Red, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.GFP, new ColorChannelModel(ChannelType.GFP, "GFP (500-550)", true, Colors.Green, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.RFP, new ColorChannelModel(ChannelType.RFP, "RFP (582-618)", true, Colors.Blue, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.NIR, new ColorChannelModel(ChannelType.NIR, "NIR (663-733)", false, Colors.None, false, 0, 1, 0, 255, container, eventAggregator));

            CurrentSelectedChannel = ColorChannelInfoMap[ChannelType.DAPI];
        }
    }
}
