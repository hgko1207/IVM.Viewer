using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using WPFDrawing = System.Windows.Media;

/**
 * @Class Name : Annotation.cs
 * @Description : Annotation 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.23     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.23
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public class AnnotationInfo : BindableBase
    {
        private bool penEnabled;
        public bool PenEnabled
        {
            get => penEnabled;
            set
            {
                if (SetProperty(ref penEnabled, value) && value)
                {
                    EraserEnabled = false;
                    TextEnabled = false;
                    DrawRectangleEnabled = false;
                    DrawCircleEnabled = false;
                    DrawTriangleEnabled = false;
                }
            }
        }

        private WPFDrawing.Color penColor;
        public WPFDrawing.Color PenColor
        {
            get => penColor;
            set => SetProperty(ref penColor, value);
        }

        private int penThickness;
        public int PenThickness
        {
            get => penThickness;
            set => SetProperty(ref penThickness, value);
        }

        private WPFDrawing.Color textColor;
        public WPFDrawing.Color TextColor
        {
            get => textColor;
            set => SetProperty(ref textColor, value);
        }

        private int textFontSize;
        public int TextFontSize
        {
            get => textFontSize;
            set => SetProperty(ref textFontSize, value);
        }

        private bool textEnabled;
        public bool TextEnabled
        {
            get => textEnabled;
            set
            {
                if (SetProperty(ref textEnabled, value) && value)
                {
                    PenEnabled = false;
                    EraserEnabled = false;
                    DrawRectangleEnabled = false;
                    DrawCircleEnabled = false;
                    DrawTriangleEnabled = false;
                }
            }
        }

        private int eraserThickness;
        public int EraserThickness
        {
            get => eraserThickness;
            set => SetProperty(ref eraserThickness, value);
        }

        private bool eraserEnabled;
        public bool EraserEnabled
        {
            get => eraserEnabled;
            set
            {
                if (SetProperty(ref eraserEnabled, value) && value)
                {
                    PenEnabled = false;
                    TextEnabled = false;
                    DrawRectangleEnabled = false;
                    DrawCircleEnabled = false;
                    DrawTriangleEnabled = false;
                }
            }
        }

        private WPFDrawing.Color drawColor;
        public WPFDrawing.Color DrawColor
        {
            get => drawColor;
            set => SetProperty(ref drawColor, value);
        }

        private bool drawRectangleEnabled;
        public bool DrawRectangleEnabled
        {
            get => drawRectangleEnabled;
            set 
            {
                if (SetProperty(ref drawRectangleEnabled, value) && value)
                {
                    PenEnabled = false;
                    TextEnabled = false;
                    EraserEnabled = false;
                    DrawCircleEnabled = false;
                    DrawTriangleEnabled = false;
                }
            }
        }

        private bool drawCircleEnabled;
        public bool DrawCircleEnabled
        {
            get => drawCircleEnabled;
            set
            {
                if (SetProperty(ref drawCircleEnabled, value) && value)
                {
                    PenEnabled = false;
                    TextEnabled = false;
                    EraserEnabled = false;
                    DrawRectangleEnabled = false;
                    DrawTriangleEnabled = false;
                }
            }
        }

        private bool drawTriangleEnabled;
        public bool DrawTriangleEnabled
        {
            get => drawTriangleEnabled;
            set
            {
                if (SetProperty(ref drawTriangleEnabled, value) && value)
                {
                    PenEnabled = false;
                    TextEnabled = false;
                    EraserEnabled = false;
                    DrawRectangleEnabled = false;
                    DrawCircleEnabled = false;
                }
            }
        }

        private int drawThickness;
        public int DrawThickness
        {
            get => drawThickness;
            set => SetProperty(ref drawThickness, value);
        }

        private int scaleBarSize;
        public int ScaleBarSize
        {
            get => scaleBarSize;
            set => SetProperty(ref scaleBarSize, value);
        }

        private int scaleBarThickness;
        public int ScaleBarThickness
        {
            get => scaleBarThickness;
            set => SetProperty(ref scaleBarThickness, value);
        }

        private bool scaleBarEnabled;
        public bool ScaleBarEnabled
        {
            get => scaleBarEnabled;
            set
            {
                if (SetProperty(ref scaleBarEnabled, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private bool labelEnabled;
        public bool LabelEnabled
        {
            get => labelEnabled;
            set
            {
                if (SetProperty(ref labelEnabled, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private IEventAggregator eventAggregator;

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="eventAggregator"></param>
        public AnnotationInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            dataManager = container.Resolve<DataManager>();

            PenThickness = 2;
            PenColor = WPFDrawing.Colors.Blue;

            TextFontSize = 12;
            TextColor = WPFDrawing.Colors.Blue;

            DrawColor = WPFDrawing.Colors.Blue;

            DrawThickness = 2;
            EraserThickness = 20;

            ScaleBarSize = 100;
            ScaleBarThickness = 2;
        }
    }
}
