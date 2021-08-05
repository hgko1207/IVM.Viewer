using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Utils;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows.Input;

/**
 * @Class Name : AnnotationPanelViewModel.cs
 * @Description : Annotation 화면 뷰 모델
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
    public class AnnotationPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<AnnotationPanel>
    {
        private List<string> fontItemList;
        public List<string> FontItemList
        {
            get => fontItemList;
            set => SetProperty(ref fontItemList, value);
        }

        private string selectedFontItem;
        public string SelectedFontItem
        {
            get => selectedFontItem;
            set => SetProperty(ref selectedFontItem, value);
        }

        private TimeSpanType selectedDateTimeType;
        public TimeSpanType SelectedDateTimeType
        {
            get => selectedDateTimeType;
            set
            {
                if (SetProperty(ref selectedDateTimeType, value))
                    SelectDateTime(value);
            }
        }

        private DateTime timeStampText;
        public DateTime TimeStampText
        {
            get => timeStampText;
            set 
            {
                if (SetProperty(ref timeStampText, value))
                    AnnotationInfo.TimeStampText = value.ToString(CommonUtil.TimaSpanToMask(SelectedDateTimeType));
            } 
        }

        private ZStackLabelType selectedZStackLabelType;
        public ZStackLabelType SelectedZStackLabelType
        {
            get => selectedZStackLabelType;
            set
            {
                if (SetProperty(ref selectedZStackLabelType, value))
                {
                    SelectZStackLabelType(value);
                    ZStackLabelUnit = GetZStackUnit(value);
                }
            }
        }

        private float zStackLabelText;
        public float ZStackLabelText
        {
            get => zStackLabelText;
            set
            {
                if (SetProperty(ref zStackLabelText, value))
                    AnnotationInfo.ZStackLabelText = value.ToString(CommonUtil.ZStackLabelToMask(SelectedZStackLabelType)) + " " + ZStackLabelUnit;
            }
        }

        private string zStackLabelUnit;
        public string ZStackLabelUnit
        {
            get => zStackLabelUnit;
            set => SetProperty(ref zStackLabelUnit, value);
        }

        public ICommand AddDrawCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }

        private AnnotationPanel view;

        public AnnotationInfo AnnotationInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public AnnotationPanelViewModel(IContainerExtension container) : base(container)
        {
            AddDrawCommand = new DelegateCommand(AddDraw);
            ClearCommand = new DelegateCommand(Clear);
            AnnotationInfo = Container.Resolve<DataManager>().AnnotationInfo;

            FontItemList = new List<string>();
            FontItemList.Add("돋음");

            SelectedFontItem = FontItemList[0];
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(AnnotationPanel view)
        {
            this.view = view;

            SelectedDateTimeType = TimeSpanType.hh_mm_ss;
            SelectDateTime(SelectedDateTimeType);

            SelectedZStackLabelType = ZStackLabelType.Label1;
            ZStackLabelUnit = GetZStackUnit(SelectedZStackLabelType);
            SelectZStackLabelType(SelectedZStackLabelType);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(AnnotationPanel view)
        {
        }

        /// <summary>
        /// Select DateTime
        /// </summary>
        /// <param name="type"></param>
        private void SelectDateTime(TimeSpanType type)
        {
            view.TimeStampLabel.Mask = CommonUtil.TimaSpanToMask(type);
            AnnotationInfo.TimeStampText = TimeStampText.ToString(CommonUtil.TimaSpanToMask(SelectedDateTimeType));
        }

        /// <summary>
        /// Select ZStackLabelType
        /// </summary>
        /// <param name="type"></param>
        private void SelectZStackLabelType(ZStackLabelType type)
        {
            string mask = CommonUtil.ZStackLabelToMask(type);
            view.ZStackLabel.Mask = mask;
            AnnotationInfo.ZStackLabelText = ZStackLabelText.ToString(mask) + " " + ZStackLabelUnit;
        }

        /// <summary>
        /// Add Draw 이벤트
        /// </summary>
        private void AddDraw()
        {

        }

        /// <summary>
        /// Clear 이벤트
        /// </summary>
        private void Clear()
        {
            EventAggregator.GetEvent<DrawClearEvent>().Publish();
        }

        /// <summary>
        /// GetZStackUnit
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetZStackUnit(ZStackLabelType type)
        {
            switch (type)
            {
                case ZStackLabelType.Label1:
                case ZStackLabelType.Label3:
                    return "μm";
                case ZStackLabelType.Label2:
                case ZStackLabelType.Label4:
                    return "mm";
                default:
                    return "";
            }
        }
    }
}
