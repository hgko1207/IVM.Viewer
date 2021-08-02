using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Windows.Input;
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// Crop 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool cropRectangleEnabled;
        public bool CropRectangleEnabled
        {
            get => cropRectangleEnabled;
            set
            {
                if (SetProperty(ref cropRectangleEnabled, value))
                {
                    CropCircleEnabled = value ? false : true;
                    if (CropEnabled)
                    {
                        CropEnabled = false;
                        CropEnabled = true;
                    }
                }
            }
        }

        private bool cropCircleEnabled;
        public bool CropCircleEnabled
        {
            get => cropCircleEnabled;
            set
            {
                if (SetProperty(ref cropCircleEnabled, value))
                {
                    CropRectangleEnabled = value ? false : true;
                    if (CropEnabled)
                    {
                        CropEnabled = false;
                        CropEnabled = true;
                    }
                }
            }
        }

        private bool cropEnabled;
        public bool CropEnabled
        {
            get => cropEnabled;
            set
            {
                if (SetProperty(ref cropEnabled, value))
                {
                    if (value)
                    {
                        PenEnabled = false;
                        EraserEnabled = false;
                        TextEnabled = false;
                        DrawRectangleEnabled = false;
                        DrawCircleEnabled = false;
                        DrawTriangleEnabled = false;
                        CropCrosshairEnabled = true;
                        eventAggregator.GetEvent<EnableCropEvent>().Publish();
                    }
                    else
                    {
                        CropCrosshairEnabled = false;
                        eventAggregator.GetEvent<DisableCropEvent>().Publish();
                    }
                }
            }
        }

        private bool cropCrosshairEnabled;
        public bool CropCrosshairEnabled
        {
            get => cropCrosshairEnabled;
            set => SetProperty(ref cropCrosshairEnabled, value);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// Pen 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    CropEnabled = false;
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// Text 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    CropEnabled = false;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// Eraser 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    CropEnabled = false;
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// Draw 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                if (SetProperty(ref drawRectangleEnabled, value))
                {
                    if (value)
                    {
                        PenEnabled = false;
                        TextEnabled = false;
                        EraserEnabled = false;
                        DrawCircleEnabled = false;
                        DrawTriangleEnabled = false;
                        CropEnabled = false;
                        eventAggregator.GetEvent<EnableDrawEvent>().Publish();
                    }
                    else
                    {
                        eventAggregator.GetEvent<DisableDrawEvent>().Publish();
                    }
                }
            }
        }

        private bool drawCircleEnabled;
        public bool DrawCircleEnabled
        {
            get => drawCircleEnabled;
            set
            {
                if (SetProperty(ref drawCircleEnabled, value))
                {
                    if (value)
                    {
                        PenEnabled = false;
                        TextEnabled = false;
                        EraserEnabled = false;
                        DrawRectangleEnabled = false;
                        DrawTriangleEnabled = false;
                        CropEnabled = false;
                        eventAggregator.GetEvent<EnableDrawEvent>().Publish();
                    }
                    else
                    {
                        eventAggregator.GetEvent<DisableDrawEvent>().Publish();
                    }
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
                    CropEnabled = false;
                }
            }
        }

        private int drawThickness;
        public int DrawThickness
        {
            get => drawThickness;
            set => SetProperty(ref drawThickness, value);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// ScaleBar 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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

        private bool xAxisEnabled;
        public bool XAxisEnabled
        {
            get => xAxisEnabled;
            set => SetProperty(ref xAxisEnabled, value);
        }

        private bool yAxisEnabled;
        public bool YAxisEnabled
        {
            get => yAxisEnabled;
            set => SetProperty(ref yAxisEnabled, value);
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

        private PositionType scaleBarPosition;
        public PositionType ScaleBarPosition
        {
            get => scaleBarPosition;
            set => SetProperty(ref scaleBarPosition, value);
        }

        private ScaleBarLabelType scaleBarLabel;
        public ScaleBarLabelType ScaleBarLabel
        {
            get => scaleBarLabel;
            set => SetProperty(ref scaleBarLabel, value);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// TimeStamp 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PositionType timeStampPosition;
        public PositionType TimeStampPosition
        {
            get => timeStampPosition;
            set => SetProperty(ref timeStampPosition, value);
        }

        private string timeStampText;
        public string TimeStampText
        {
            get => timeStampText;
            set => SetProperty(ref timeStampText, value);
        }

        private bool timeStampEnabled;
        public bool TimeStampEnabled
        {
            get => timeStampEnabled;
            set
            {
                if (SetProperty(ref timeStampEnabled, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// ZStackLabel 정보
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        private PositionType zStackLabelPosition;
        public PositionType ZStackLabelPosition
        {
            get => zStackLabelPosition;
            set => SetProperty(ref zStackLabelPosition, value);
        }

        private string zStackLabelText;
        public string ZStackLabelText
        {
            get => zStackLabelText;
            set => SetProperty(ref zStackLabelText, value);
        }

        private bool zStackLabelEnabled;
        public bool ZStackLabelEnabled
        {
            get => zStackLabelEnabled;
            set
            {
                if (SetProperty(ref zStackLabelEnabled, value))
                    eventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        public ICommand AllCropCommand { get; private set; }
        public ICommand ExportCropCommand { get; private set; }

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

            AllCropCommand = new DelegateCommand(AllCrop);
            ExportCropCommand = new DelegateCommand(ExportCrop);

            CropRectangleEnabled = true;

            PenThickness = 2;
            PenColor = WPFDrawing.Colors.Blue;

            TextFontSize = 12;
            TextColor = WPFDrawing.Colors.Blue;

            EraserThickness = 20;

            DrawColor = WPFDrawing.Colors.Blue;
            DrawThickness = 2;

            ScaleBarSize = 100;
            ScaleBarThickness = 2;
            XAxisEnabled = true;
            YAxisEnabled = true;
            ScaleBarPosition = PositionType.RIGHT;
            ScaleBarLabel = ScaleBarLabelType.None;

            TimeStampPosition = PositionType.LEFT;

            ZStackLabelPosition = PositionType.RIGHT;
        }

        /// <summary>
        /// All Crop
        /// </summary>
        private void AllCrop()
        {

        }

        /// <summary>
        /// Export Crop
        /// </summary>
        private void ExportCrop()
        {
            if (CropEnabled)
                eventAggregator.GetEvent<ExportCropEvent>().Publish();
        }
    }
}
