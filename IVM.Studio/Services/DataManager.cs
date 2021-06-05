using IVM.Studio.Models;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IVM.Studio.Services
{
    public class DataManager
    {
        public SlideInfo SelectedSlideInfo { get; set; }

        public Dictionary<int, ColorChannelModel> ColorChannelInfoMap { get; set; }

        public ColorChannelModel CurrentSelectedChannel { get; set; }

        public void Init(IContainerExtension container, IEventAggregator eventAggregator)
        {
            ColorChannelInfoMap = new Dictionary<int, ColorChannelModel>();
            ColorChannelInfoMap.Add(0, new ColorChannelModel(0, true, Colors.Red, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(1, new ColorChannelModel(1, true, Colors.Green, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(2, new ColorChannelModel(2, true, Colors.Blue, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(3, new ColorChannelModel(3, false, Colors.None, false, 0, 1, 0, 255, container, eventAggregator));

            CurrentSelectedChannel = ColorChannelInfoMap[0];
        }
    }
}
