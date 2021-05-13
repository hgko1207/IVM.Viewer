using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.MvvM;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ColormapViewModel : ViewModelBase
    {
        public IEnumerable<string> ColorMapString => Enum.GetValues(typeof(Colors)).Cast<Colors>().Select(s => s.ToString());

        public IEnumerable<ColorMap> ColorMaps
        {
            get
            {
                return new[] { ColorMap.Hot, ColorMap.Cool, ColorMap.Inferno, ColorMap.Bone, ColorMap.Jet,
                    ColorMap.Rainbow, ColorMap.Autumn, ColorMap.Ocean, ColorMap.TwilightShifted,
                    ColorMap.Winter, ColorMap.Summer, ColorMap.Spring, ColorMap.Hsv, ColorMap.Pink,
                    ColorMap.Parula, ColorMap.Magma, ColorMap.Plasma, ColorMap.Viridis, ColorMap.Cividis, ColorMap.Twilight
                };
            }
        }

        private ColorChannelModel currentSelectedChannel;
        public ColorChannelModel CurrentSelectedChannel
        {
            get => currentSelectedChannel;
            set
            {
                if (SetProperty(ref currentSelectedChannel, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        private ObservableCollection<ColorChannelModel> colorChannelInfoCollection { get; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ColormapViewModel(IContainerExtension container, IEventAggregator eventAggregator) : base(container, eventAggregator)
        {
            colorChannelInfoCollection = new ObservableCollection<ColorChannelModel> {
                new ColorChannelModel(0, true, Colors.Red, false, 0, 1, 0, 255, container, EventAggregator),
                new ColorChannelModel(1, true, Colors.Green, false, 0, 1, 0, 255, container, EventAggregator),
                new ColorChannelModel(2, true, Colors.Blue, false, 0, 1, 0, 255, container, EventAggregator),
                new ColorChannelModel(3, false, Colors.None, false, 0, 1, 0, 255, container, EventAggregator)
            };
            CurrentSelectedChannel = colorChannelInfoCollection[0];
        }
    }
}
