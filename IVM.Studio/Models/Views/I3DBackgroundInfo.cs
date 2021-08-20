using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System.Windows.Input;
using System.Windows.Media;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Models
{
    public class I3DBackgroundInfo : BindableBase
    {
        I3DWcfServer wcfserver;

        int channelId = -1;

        private Color boxColor = Color.FromScRgb(1, 1, 1, 1);
        public Color BoxColor
        {
            get => boxColor;
            set
            {
                if (SetProperty(ref boxColor, value))
                {
                    wcfserver.Channel(channelId).OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
                }
            }
        }

        private float boxThickness = 2;
        public float BoxThickness
        {
            get => boxThickness;
            set
            {
                if (SetProperty(ref boxThickness, value))
                {
                    wcfserver.Channel(channelId).OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
                }
            }
        }

        private float gridMajor = 40;
        public float GridMajor
        {
            get => gridMajor;
            set
            {
                if (SetProperty(ref gridMajor, value))
                {
                    wcfserver.Channel(channelId).OnChangeGridSizeParam(gridMajor, gridMinor);
                }
            }
        }

        private float gridMinor = 10;
        public float GridMinor
        {
            get => gridMinor;
            set
            {
                if (SetProperty(ref gridMinor, value))
                {
                    wcfserver.Channel(channelId).OnChangeGridSizeParam(gridMajor, gridMinor);
                }
            }
        }

        private Color gridLabelColor = Color.FromScRgb(1, 1, 1, 1);
        public Color GridLabelColor
        {
            get => gridLabelColor;
            set
            {
                if (SetProperty(ref gridLabelColor, value))
                {
                    wcfserver.Channel(channelId).OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
                }
            }
        }

        private int gridFontSize = 12;
        public int GridFontSize
        {
            get => gridFontSize;
            set
            {
                if (SetProperty(ref gridFontSize, value))
                {
                    wcfserver.Channel(channelId).OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
                }
            }
        }

        private Color backgroundColor = Color.FromScRgb(1, 0, 0, 0);
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                if (SetProperty(ref backgroundColor, value))
                {
                    wcfserver.Channel(channelId).OnChangeBackgroundParam(backgroundColor.ScR, backgroundColor.ScG, backgroundColor.ScB, backgroundColor.ScA);
                }
            }
        }

        public I3DBackgroundInfo(IContainerExtension container, IEventAggregator eventAggregator, int channelId)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            this.channelId = channelId;
        }
    }
}
