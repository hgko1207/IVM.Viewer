using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Media;

/**
 * @Class Name : ColorChannelModel.cs
 * @Description : 컬러 채널 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.30     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.30
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public enum ChannelType 
    {
        DAPI, GFP, RFP, NIR, ALL
    }

    public class ColorChannelModel : BindableBase, IEquatable<ColorChannelModel>
    {
        public int Index { get; }

        public ChannelType ChannelType { get; set; }

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
                        ContainerExtension.Resolve<WindowByChannelService>().ShowDisplay(Index, AlwaysTopEnabled);
                    else
                        ContainerExtension.Resolve<WindowByChannelService>().CloseDisplay(Index);
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
                    ContainerExtension.Resolve<WindowByChannelService>().ChangeOwner(Index, value);
            }
        }

        private ImageSource histogramImage;
        public ImageSource HistogramImage
        {
            get => histogramImage;
            set => SetProperty(ref histogramImage, value);
        }

        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 낮은 쪽 값입니다.</summary>
        private int _ColorLevelLowerValue;
        public int ColorLevelLowerValue
        {
            get => _ColorLevelLowerValue;
            set
            {
                if (SetProperty(ref _ColorLevelLowerValue, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 높은 쪽 값입니다.</summary>
        private int _ColorLevelUpperValue;
        public int ColorLevelUpperValue
        {
            get => _ColorLevelUpperValue;
            set
            {
                if (SetProperty(ref _ColorLevelUpperValue, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        public IContainerExtension ContainerExtension;
        public IEventAggregator EventAggregator;

        public ColorChannelModel(ChannelType type, string channelName, bool visible, Colors color, bool display, float brightness, float contrast, 
                int lowerLevel, int upperLevel, IContainerExtension containerExtension, IEventAggregator eventAggregator)
        {
            this.ContainerExtension = containerExtension;
            this.EventAggregator = eventAggregator;

            this.Index = (int) type;
            this.ChannelType = type;
            this.ChannelName = channelName;
            this.visible = visible;
            this.color = color;
            this.display = display;
            this.brightness = brightness;
            this.contrast = contrast;
            this.colorMap = ColorMap.Hot;
            this._ColorLevelLowerValue = lowerLevel;
            this._ColorLevelUpperValue = upperLevel;
            this.alwaysTopEnabled = true;

            eventAggregator.GetEvent<ChWindowCloseEvent>().Subscribe(ClosedDisplay);
        }

        public ColorChannelModel(ChannelType type, string channelName)
        {
            this.ChannelType = type;
            this.ChannelName = channelName;
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

        public bool UpdateColorLevelUpperWithoutRefresh(int colorLevelUpperValue)
        {
            return SetProperty(ref _ColorLevelUpperValue, colorLevelUpperValue, nameof(colorLevelUpperValue));
        }

        public bool UpdateColorLevelLowerWithoutRefresh(int colorLevelLowerValue)
        {
            return SetProperty(ref _ColorLevelLowerValue, colorLevelLowerValue, nameof(colorLevelLowerValue));
        }

        /// <summary>
        /// string to color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(string color)
        {
            switch (color)
            {
                case "Red":
                    Color = Colors.Red;
                    break;
                case "Green":
                    Color = Colors.Green;
                    break;
                case "Blue":
                    Color = Colors.Blue;
                    break;
                case "Alpha":
                    Color = Colors.Alpha;
                    break;
                case "None":
                    Color = Colors.None;
                    break;
            }
        }
    }
}
