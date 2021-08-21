using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Models.Views;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

/**
 * @Class Name : DisplayControlPanelViewModel.cs
 * @Description : 이미지 컨트롤 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.10     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.10
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class DisplayControlPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<DisplayControlPanel>
    {
        private bool navigatorEnabled;
        public bool NavigatorEnabled
        {
            get => navigatorEnabled;
            set
            {
                if (SetProperty(ref navigatorEnabled, value))
                    ShowNavigator(value);
            }
        }

        private int zoomRatioValue;
        public int ZoomRatioValue
        {
            get => zoomRatioValue;
            set
            {
                if (SetProperty(ref zoomRatioValue, value))
                    EventAggregator.GetEvent<ZoomRatioControlEvent>().Publish(value);
            }
        }

        private VidioModeType selectedVidioMode;
        public VidioModeType SelectedVidioMode
        {
            get => selectedVidioMode;
            set => SetProperty(ref selectedVidioMode, value);
        }

        public ICommand ResetZoomRatioCommand { get; private set; }

        private DisplayControlPanel view;

        public SliderControlInfo SliderControlInfo { get; set; }

        private NavigatorParam navigatorParam;
        private Rectangle drawRectangle;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public DisplayControlPanelViewModel(IContainerExtension container) : base(container)
        {
            ResetZoomRatioCommand = new DelegateCommand(ResetZoomRatio);

            EventAggregator.GetEvent<NavigatorChangeEvent>().Subscribe(NavigatorChange, ThreadOption.UIThread);

            SliderControlInfo = container.Resolve<DataManager>().SliderControlInfo;

            ZoomRatioValue = 100;

            SelectedVidioMode = VidioModeType.ZS;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(DisplayControlPanel view)
        {
            this.view = view;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(DisplayControlPanel view)
        {
        }

        /// <summary>
        /// Show Navigator
        /// </summary>
        /// <param name="enabled"></param>
        private void ShowNavigator(bool enabled)
        {
            if (enabled)
            {
                if (drawRectangle == null)
                {
                    drawRectangle = new Rectangle()
                    {
                        Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFC000"),
                        StrokeThickness = 2,
                        Width = 0,
                        Height = 0
                    };

                    view.NavigatorCanvas.Children.Add(drawRectangle);

                    if (navigatorParam != null)
                        DrawRectangle();
                }
            }
            else
            {
                view.NavigatorCanvas.Children.Remove(drawRectangle);
                drawRectangle = null;
            }
        }

        /// <summary>
        /// Changed Navigator
        /// </summary>
        /// <param name="navigatorParam"></param>
        private void NavigatorChange(NavigatorParam navigatorParam)
        {
            this.navigatorParam = navigatorParam;

            ZoomRatioValue = navigatorParam.ZoomRatio;

            DrawRectangle();
        }

        /// <summary>
        /// DrawRectangle()
        /// </summary>
        private void DrawRectangle()
        {
            if (drawRectangle != null)
            {
                double widthRatio = view.NavigatorCanvas.ActualWidth / navigatorParam.ImageWidth;
                double heightRatio = view.NavigatorCanvas.ActualHeight / navigatorParam.ImageHeight;

                GetPositionToCropParam positionParam = new GetPositionToCropParam();
                EventAggregator.GetEvent<GetPositionToCropEvent>().Publish(positionParam);

                positionParam.HorizontalOffset = Math.Max(positionParam.HorizontalOffset / ZoomRatioValue * 100, 0);
                positionParam.VerticalOffset = Math.Max(positionParam.VerticalOffset / ZoomRatioValue * 100, 0);
                positionParam.ViewportWidth = positionParam.ViewportWidth / ZoomRatioValue * 100;
                if (navigatorParam.ImageWidth < positionParam.ViewportWidth)
                    positionParam.ViewportWidth = navigatorParam.ImageWidth;

                positionParam.ViewportHeight = positionParam.ViewportHeight / ZoomRatioValue * 100;
                if (navigatorParam.ImageHeight < positionParam.ViewportHeight)
                    positionParam.ViewportHeight = navigatorParam.ImageHeight;

                drawRectangle.Width = positionParam.ViewportWidth * widthRatio;
                drawRectangle.Height = positionParam.ViewportHeight * widthRatio;
                Canvas.SetLeft(drawRectangle, positionParam.HorizontalOffset * widthRatio);
                Canvas.SetTop(drawRectangle, positionParam.VerticalOffset * heightRatio);
            }
        }

        /// <summary>
        /// Reset ZoomRatio
        /// </summary>
        private void ResetZoomRatio()
        {
            ZoomRatioValue = 100;

            EventAggregator.GetEvent<ZoomRatioControlEvent>().Publish(ZoomRatioValue);
        }
    }
}