using IVM.Studio.Models.Events;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Models
{
    public class ColorChannelModel : BindableBase, IEquatable<ColorChannelModel>
    {
        public int Index { get; }

        private bool visible;
        public bool Visible
        {
            get => visible;
            set
            {
                if (SetProperty(ref visible, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private string channelName;
        public string ChannelName
        {
            get => channelName;
            set => SetProperty(ref channelName, value);
        }

        private Colors color;
        public Colors Color
        {
            get => color;
            set
            {
                if (SetProperty(ref color, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private bool display;
        public bool Display
        {
            get => display;
            set
            {
                if (SetProperty(ref display, value))
                {
                    if (value)
                        ContainerExtension.Resolve<Services.WindowByChannelService>().ShowDisplay(Index, AlwaysTopEnabled);
                    else
                        ContainerExtension.Resolve<Services.WindowByChannelService>().CloseDisplay(Index);
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
                }
            }
        }
        private float brightness;
        public float Brightness
        {
            get => brightness;
            set
            {
                if (SetProperty(ref brightness, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private float contrast;
        public float Contrast
        {
            get => contrast;
            set
            {
                if (SetProperty(ref contrast, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        private ColorMap colorMap;
        public ColorMap ColorMap
        {
            get => colorMap;
            set
            {
                if (SetProperty(ref colorMap, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private bool colorMapEnabled;
        public bool ColorMapEnabled
        {
            get => colorMapEnabled;
            set
            {
                if (SetProperty(ref colorMapEnabled, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private bool alwaysTopEnabled;
        public bool AlwaysTopEnabled
        {
            get => alwaysTopEnabled;
            set
            {
                if (SetProperty(ref alwaysTopEnabled, value))
                    ContainerExtension.Resolve<Services.WindowByChannelService>().ChangeOwner(Index, value);
            }
        }

        private WPFDrawing.ImageSource histogramImage;
        public WPFDrawing.ImageSource HistogramImage
        {
            get => histogramImage;
            set => SetProperty(ref histogramImage, value);
        }

        private int colorLevelLowerValue;
        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 낮은 쪽 값입니다.</summary>
        public int ColorLevelLowerValue
        {
            get => colorLevelLowerValue;
            set
            {
                if (SetProperty(ref colorLevelLowerValue, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }
        private int colorLevelUpperValue;
        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 높은 쪽 값입니다.</summary>
        public int ColorLevelUpperValue
        {
            get => colorLevelUpperValue;
            set
            {
                if (SetProperty(ref colorLevelUpperValue, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        protected IContainerExtension ContainerExtension;
        protected IEventAggregator EventAggregator;

        public ColorChannelModel(int index, bool visible, Colors color, bool display, float brightness, float contrast, 
                int lowerLevel, int upperLevel, IContainerExtension containerExtension, IEventAggregator eventAggregator)
        {
            this.ContainerExtension = containerExtension;
            this.EventAggregator = eventAggregator;

            this.Index = index;
            this.visible = visible;
            this.color = color;
            this.display = display;
            this.brightness = brightness;
            this.contrast = contrast;
            this.colorMap = ColorMap.Hot;
            this.colorLevelLowerValue = lowerLevel;
            this.colorLevelUpperValue = upperLevel;
            this.alwaysTopEnabled = true;

            EventAggregator.GetEvent<ChWindowClosedEvent>().Subscribe(ClosedDisplay);
        }

        public override bool Equals(object obj) => Equals(obj as ColorChannelModel);

        public bool Equals(ColorChannelModel other) => !(other is null) && Index == other.Index;

        public override int GetHashCode() => Index;

        public static bool operator ==(ColorChannelModel left, ColorChannelModel right) => EqualityComparer<ColorChannelModel>.Default.Equals(left, right);

        public static bool operator !=(ColorChannelModel left, ColorChannelModel right) => !EqualityComparer<ColorChannelModel>.Default.Equals(left, right);

        private void ClosedDisplay(int index)
        {
            if (index == this.Index)
                SetProperty(ref display, false, nameof(Display));
        }

        public bool UpdateBrightnessWithoutRefresh(float Brightness)
        {
            return SetProperty(ref brightness, Brightness, nameof(Brightness));
        }

        public bool UpdateContrastWithoutRefresh(float Contrast)
        {
            return SetProperty(ref contrast, Contrast, nameof(Contrast));
        }

        public bool UpdateColorLevelUpperWithoutRefresh(int ColorLevelUpperValue)
        {
            return SetProperty(ref colorLevelUpperValue, ColorLevelUpperValue, nameof(ColorLevelUpperValue));
        }

        public bool UpdateColorLevelLowerWithoutRefresh(int ColorLevelLowerValue)
        {
            return SetProperty(ref colorLevelLowerValue, ColorLevelLowerValue, nameof(ColorLevelLowerValue));
        }
    }
}
