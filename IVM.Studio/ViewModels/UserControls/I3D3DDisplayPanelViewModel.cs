using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3D3DDisplayPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        private bool axisVisible = true;
        public bool AxisVisible
        {
            get => axisVisible;
            set
            {
                if (SetProperty(ref axisVisible, value))
                {
                    float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
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
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
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
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
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
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
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
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                }
            }
        }

        float AxisSizeToHeight()
        {
            return (float)axisHeight * 2.0f / 100.0f * 0.1f;
        }

        void AxisModeToPos(AxisPosType m, ref float px, ref float py)
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

        private Color boxColor = Color.FromScRgb(1, 1, 1, 1);
        public Color BoxColor
        {
            get => boxColor;
            set
            {
                if (SetProperty(ref boxColor, value))
                {
                    wcfserver.channel1.OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
                    wcfserver.channel2.OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
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
                    wcfserver.channel1.OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
                    wcfserver.channel2.OnChangeBoxParam(boxColor.ScR, boxColor.ScG, boxColor.ScB, boxColor.ScA, boxThickness);
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
                    wcfserver.channel1.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
                    wcfserver.channel2.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
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
                    wcfserver.channel1.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
                    wcfserver.channel2.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
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
                    wcfserver.channel1.OnChangeBackgroundParam(backgroundColor.ScR, backgroundColor.ScG, backgroundColor.ScB, backgroundColor.ScA);
                    wcfserver.channel2.OnChangeBackgroundParam(backgroundColor.ScR, backgroundColor.ScG, backgroundColor.ScB, backgroundColor.ScA);
                }
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3D3DDisplayPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            EventAggregator.GetEvent<I3DMetaLoadedEvent>().Subscribe(OnMetaLoaded);
        }

        private void OnMetaLoaded(I3DMetaLoadedParam obj)
        {
            // 로딩후 업데이트 해주어야 폰트가 보임
            float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
            Task.Run(() => {
                wcfserver.channel1.OnChangeBoxParam(boxColor.R, boxColor.G, boxColor.B, boxColor.A, boxThickness);
                wcfserver.channel2.OnChangeBoxParam(boxColor.R, boxColor.G, boxColor.B, boxColor.A, boxThickness);

                wcfserver.channel1.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);
                wcfserver.channel2.OnChangeGridLabelParam(gridLabelColor.ScR, gridLabelColor.ScG, gridLabelColor.ScB, gridLabelColor.ScA, gridFontSize);

                wcfserver.channel1.OnChangeBackgroundParam(backgroundColor.ScR, backgroundColor.ScG, backgroundColor.ScB, backgroundColor.ScA);
                wcfserver.channel2.OnChangeBackgroundParam(backgroundColor.ScR, backgroundColor.ScG, backgroundColor.ScB, backgroundColor.ScA);

                // 마지막에 업데이트 해야 함
                wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
                wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py);
            });
        }
    }
}
