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

        DataManager datamanager;

        public I3DBackgroundInfo I3DBackgroundInfo { get; set; }

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

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3D3DDisplayPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
            datamanager = container.Resolve<DataManager>();

            I3DBackgroundInfo = datamanager.I3DBackgroundInfo1;

            EventAggregator.GetEvent<I3DFirstRenderEvent>().Subscribe(OnFirstRender);
        }

        private void OnFirstRender(int viewtype)
        {
            // 로딩후 업데이트 해주어야 폰트가 보임
            float px = 0, py = 0; AxisModeToPos(axisPosMode, ref px, ref py);
            I3DBackgroundInfo b = I3DBackgroundInfo;

            if (viewtype == (int)I3DViewType.MAIN_VIEW)
            {
                Task.Run(() =>
                {
                    wcfserver.channel1.OnChangeBoxParam(b.BoxColor.ScR, b.BoxColor.ScG, b.BoxColor.ScB, b.BoxColor.ScA, b.BoxThickness);
                    wcfserver.channel1.OnChangeGridLabelParam(b.GridLabelColor.ScR, b.GridLabelColor.ScG, b.GridLabelColor.ScB, b.GridLabelColor.ScA, b.GridFontSize);
                    wcfserver.channel1.OnChangeBackgroundParam(b.BackgroundColor.ScR, b.BackgroundColor.ScG, b.BackgroundColor.ScB, b.BackgroundColor.ScA);
                    wcfserver.channel1.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py); // 마지막에 업데이트 해야 함
                });
            }
            else
            {
                Task.Run(() =>
                {
                    wcfserver.channel2.OnChangeBoxParam(b.BoxColor.ScR, b.BoxColor.ScG, b.BoxColor.ScB, b.BoxColor.ScA, b.BoxThickness);
                    wcfserver.channel2.OnChangeGridLabelParam(b.GridLabelColor.ScR, b.GridLabelColor.ScG, b.GridLabelColor.ScB, b.GridLabelColor.ScA, b.GridFontSize);
                    wcfserver.channel2.OnChangeBackgroundParam(b.BackgroundColor.ScR, b.BackgroundColor.ScG, b.BackgroundColor.ScB, b.BackgroundColor.ScA);
                    wcfserver.channel2.OnChangeAxisParam(axisVisible, axisFontSize, AxisSizeToHeight(), axisThickness, px, py); // 마지막에 업데이트 해야 함
                });
            }
        }
    }
}
