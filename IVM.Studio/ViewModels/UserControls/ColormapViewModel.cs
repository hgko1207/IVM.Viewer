using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.MvvM;
using IVM.Studio.Services;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ColormapViewModel : ViewModelBase
    {
        private ColorChannelModel currentSelectedChannel;
        public ColorChannelModel CurrentSelectedChannel
        {
            get => currentSelectedChannel;
            set => SetProperty(ref currentSelectedChannel, value);
        }

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

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ColormapViewModel(IContainerExtension container) : base(container)
        {
            CurrentSelectedChannel = Container.Resolve<DataManager>().CurrentSelectedChannel;
        }
    }
}
