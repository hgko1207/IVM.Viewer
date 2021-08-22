using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;
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

        private bool axisVisible = true;
        public bool AxisVisible
        {
            get => axisVisible;
            set
            {
                if (SetProperty(ref axisVisible, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.Channel(channelId).OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        private int axisHeight = 100;
        public int AxisHeight
        {
            get => axisHeight;
            set
            {
                if (SetProperty(ref axisHeight, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.Channel(channelId).OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        private float axisThickness = 2;
        public float AxisThickness
        {
            get => axisThickness;
            set
            {
                if (SetProperty(ref axisThickness, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.Channel(channelId).OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        private int axisFontSize = 12;
        public int AxisFontSize
        {
            get => axisFontSize;
            set
            {
                if (SetProperty(ref axisFontSize, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.Channel(channelId).OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        public enum AxisPosType
        {
            [Display(Name = "RightTop", Order = 0)]
            RightTop = 0,
            [Display(Name = "LeftTop", Order = 1)]
            LeftTop = 1,
            [Display(Name = "RightBottom", Order = 2)]
            RightBottom = 2,
            [Display(Name = "LeftBottom", Order = 3)]
            LeftBottom = 3,
        }

        private AxisPosType axisPosMode = AxisPosType.RightTop;
        public AxisPosType AxisPosMode
        {
            get => axisPosMode;
            set
            {
                if (SetProperty(ref axisPosMode, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.Channel(channelId).OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        public float AxisSizeToHeight()
        {
            return (float)axisHeight * 2.0f / 100.0f * 0.1f;
        }

        public void AxisModeToPos(AxisPosType m, ref float px, ref float py)
        {
            if (m == AxisPosType.RightTop)
            {
                px = 0.75f;
                py = 0.75f;
            }
            else if (m == AxisPosType.LeftTop)
            {
                px = -0.75f;
                py = 0.75f;
            }
            else if (m == AxisPosType.RightBottom)
            {
                px = 0.75f;
                py = -0.75f;
            }
            else if (m == AxisPosType.LeftBottom)
            {
                px = -0.75f;
                py = -0.75f;
            }
        }

        public I3DBackgroundInfo(IContainerExtension container, IEventAggregator eventAggregator, int channelId)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            this.channelId = channelId;
        }
    }
}
