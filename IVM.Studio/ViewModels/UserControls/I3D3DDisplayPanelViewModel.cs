using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Ioc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3D3DDisplayPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        private readonly DataManager datamanager;

        public I3DBackgroundInfo I3DBackgroundInfo { get; set; }

        private bool timelapseVisible = true;
        public bool TimelapseVisible
        {
            get => timelapseVisible;
            set
            {
                if (SetProperty(ref timelapseVisible, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                }
            }
        }

        private Color timelapseLabelColor = Color.FromScRgb(1, 1, 1, 1);
        public Color TimelapseLabelColor
        {
            get => timelapseLabelColor;
            set
            {
                if (SetProperty(ref timelapseLabelColor, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                }
            }
        }

        private int timelapseFontSize = 12;
        public int TimelapseFontSize
        {
            get => timelapseFontSize;
            set
            {
                if (SetProperty(ref timelapseFontSize, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                }
            }
        }

        public enum TimelapsePosType
        {
            [Display(Name = "RightTop", Order = 0)]
            RightTop = 0,
            [Display(Name = "LeftTop", Order = 1)]
            LeftTop = 1,
            [Display(Name = "Top", Order = 2)]
            Top = 2,
            [Display(Name = "RightBottom", Order = 3)]
            RightBottom = 3,
            [Display(Name = "LeftBottom", Order = 4)]
            LeftBottom = 4,
            [Display(Name = "Bottom", Order = 5)]
            Bottom = 5,
        }

        private TimelapsePosType timelapsePosMode = TimelapsePosType.LeftTop;
        public TimelapsePosType TimelapsePosMode
        {
            get => timelapsePosMode;
            set
            {
                if (SetProperty(ref timelapsePosMode, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                }
            }
        }

        void TimelapseToPos(TimelapsePosType m, ref float px, ref float py)
        {
            if (m == TimelapsePosType.RightTop)
            {
                px = 0.70f;
                py = 0.95f;
            }
            else if (m == TimelapsePosType.LeftTop)
            {
                px = 0.05f;
                py = 0.95f;
            }
            else if (m == TimelapsePosType.Top)
            {
                px = 0.40f;
                py = 0.95f;
            }
            else if (m == TimelapsePosType.RightBottom)
            {
                px = 0.70f;
                py = 0.05f;
            }
            else if (m == TimelapsePosType.LeftBottom)
            {
                px = 0.05f;
                py = 0.05f;
            }
            else if (m == TimelapsePosType.Bottom)
            {
                px = 0.40f;
                py = 0.05f;
            }
        }

        private string timelapseFormat = "yy-MM-dd HH:mm:ss";
        public string TimelapseFormat
        {
            get => timelapseFormat;
            set
            {
                if (SetProperty(ref timelapseFormat, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                }
            }
        }

        private int timelapseInterval = 100;
        public int TimelapseInterval
        {
            get => timelapseInterval;
            set
            {
                if (SetProperty(ref timelapseInterval, value))
                {
                    float px = 0, py = 0; TimelapseToPos(timelapsePosMode, ref px, ref py);
                    wcfserver.channel1.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
                    wcfserver.channel2.OnChangeTimelapseLabelParam(timelapseVisible, timelapseLabelColor.ScR, timelapseLabelColor.ScG, timelapseLabelColor.ScB, timelapseLabelColor.ScA, timelapseFontSize, timelapseFormat, px, py, timelapseInterval);
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
            datamanager = container.Resolve<DataManager>();

            I3DBackgroundInfo = datamanager.I3DBackgroundInfo1;

            EventAggregator.GetEvent<I3DFirstRenderEvent>().Subscribe(OnFirstRender);
        }

        private void OnFirstRender(int viewtype)
        {
            // 로딩후 업데이트 해주어야 폰트가 보임
            I3DBackgroundInfo b = I3DBackgroundInfo;
            float px = 0, py = 0; b.AxisModeToPos(b.AxisPosMode, ref px, ref py);

            if (viewtype == (int)I3DViewType.MAIN_VIEW)
            {
                Task.Run(() =>
                {
                    wcfserver.channel1.OnChangeBoxParam(b.BoxColor.ScR, b.BoxColor.ScG, b.BoxColor.ScB, b.BoxColor.ScA, b.BoxThickness);
                    wcfserver.channel1.OnChangeGridLabelParam(b.GridLabelColor.ScR, b.GridLabelColor.ScG, b.GridLabelColor.ScB, b.GridLabelColor.ScA, b.GridFontSize);
                    wcfserver.channel1.OnChangeBackgroundParam(b.BackgroundColor.ScR, b.BackgroundColor.ScG, b.BackgroundColor.ScB, b.BackgroundColor.ScA);
                    wcfserver.channel1.OnChangeAxisParam(b.AxisVisible, b.AxisFontSize, b.AxisSizeToHeight(), b.AxisThickness, px, py); // 마지막에 업데이트 해야 함
                });
            }
            else
            {
                Task.Run(() =>
                {
                    wcfserver.channel2.OnChangeBoxParam(b.BoxColor.ScR, b.BoxColor.ScG, b.BoxColor.ScB, b.BoxColor.ScA, b.BoxThickness);
                    wcfserver.channel2.OnChangeGridLabelParam(b.GridLabelColor.ScR, b.GridLabelColor.ScG, b.GridLabelColor.ScB, b.GridLabelColor.ScA, b.GridFontSize);
                    wcfserver.channel2.OnChangeBackgroundParam(b.BackgroundColor.ScR, b.BackgroundColor.ScG, b.BackgroundColor.ScB, b.BackgroundColor.ScA);
                    wcfserver.channel2.OnChangeAxisParam(b.AxisVisible, b.AxisFontSize, b.AxisSizeToHeight(), b.AxisThickness, px, py); // 마지막에 업데이트 해야 함
                });
            }
        }
    }
}
