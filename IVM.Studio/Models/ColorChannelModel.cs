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
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
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
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private Colors initColor;
        public Colors InitColor
        {
            get => initColor;
            set => SetProperty(ref initColor, value);
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
                        containerExtension.Resolve<WindowByChannelService>().ShowDisplay((int)InitColor, ChannelType, AlwaysTopEnabled);
                    else
                        containerExtension.Resolve<WindowByChannelService>().CloseDisplay((int)InitColor);
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
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
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private float contrast;
        public float Contrast
        {
            get => contrast;
            set
            {
                if (SetProperty(ref contrast, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private ColorMap colorMap;
        public ColorMap ColorMap
        {
            get => colorMap;
            set
            {
                if (SetProperty(ref colorMap, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private bool colorMapEnabled;
        public bool ColorMapEnabled
        {
            get => colorMapEnabled;
            set
            {
                if (SetProperty(ref colorMapEnabled, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private bool alwaysTopEnabled;
        public bool AlwaysTopEnabled
        {
            get => alwaysTopEnabled;
            set
            {
                if (SetProperty(ref alwaysTopEnabled, value))
                    containerExtension.Resolve<WindowByChannelService>().ChangeOwner(Index, value);
            }
        }

        private bool histogram;
        public bool Histogram
        {
            get => histogram;
            set
            {
                if (SetProperty(ref histogram, value))
                {
                    if (value)
                        containerExtension.Resolve<WindowByHistogramService>().ShowDisplay(ChannelType, AlwaysTopEnabled);
                    else
                        containerExtension.Resolve<WindowByHistogramService>().CloseDisplay(Index);
                }
            }
        }

        private ImageSource histogramImage;
        public ImageSource HistogramImage
        {
            get => histogramImage;
            set => SetProperty(ref histogramImage, value);
        }

        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 낮은 쪽 값입니다.</summary>
        private int colorLevelLowerValue;
        public int ColorLevelLowerValue
        {
            get => colorLevelLowerValue;
            set
            {
                if (SetProperty(ref colorLevelLowerValue, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        /// <summary>이미지 컬러 레벨값을 지정하는 범위의 높은 쪽 값입니다.</summary>
        private int colorLevelUpperValue;
        public int ColorLevelUpperValue
        {
            get => colorLevelUpperValue;
            set
            {
                if (SetProperty(ref colorLevelUpperValue, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private readonly IContainerExtension containerExtension;
        private readonly IEventAggregator eventAggregator;

        private readonly DataManager dataManager;

        public ColorChannelModel(ChannelType type, string channelName, bool visible, Colors color, bool display, float brightness, float contrast, 
                int lowerLevel, int upperLevel, IContainerExtension containerExtension, IEventAggregator eventAggregator)
        {
            this.containerExtension = containerExtension;
            this.eventAggregator = eventAggregator;
            this.dataManager = containerExtension.Resolve<DataManager>();

            this.Index = (int) type;
            this.ChannelType = type;
            this.channelName = channelName;
            this.visible = visible;
            this.color = color;
            this.display = display;
            this.brightness = brightness;
            this.contrast = contrast;
            this.colorMap = ColorMap.Hot;
            this.colorLevelLowerValue = lowerLevel;
            this.colorLevelUpperValue = upperLevel;
            this.alwaysTopEnabled = true;

            eventAggregator.GetEvent<ChViewerWindowCloseEvent>().Subscribe(ClosedDisplay);
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

        public bool UpdateBrightnessWithoutRefresh(float value)
        {
            return base.SetProperty(ref this.brightness, value, nameof(Brightness));
        }

        public bool UpdateContrastWithoutRefresh(float value)
        {
            return SetProperty(ref contrast, value, nameof(Contrast));
        }

        public bool UpdateColorLevelUpperWithoutRefresh(int value)
        {
            return SetProperty(ref colorLevelUpperValue, value, nameof(ColorLevelUpperValue));
        }

        public bool UpdateColorLevelLowerWithoutRefresh(int value)
        {
            return SetProperty(ref colorLevelLowerValue, value, nameof(ColorLevelLowerValue));
        }

        /// <summary>
        /// Visible SetProperty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetPropertyVisible(bool value)
        {
            return SetProperty(ref visible, value, nameof(Visible));
        }

        /// <summary>
        /// Color SetProperty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="init"></param>
        public void SetPropertyColor(Colors value, bool init)
        {
            if (init)
            {
                SetProperty(ref color, value, nameof(Color));
                InitColor = value;
            }
            else
                Color = value;
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
