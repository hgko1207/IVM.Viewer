using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Ioc;
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
    public class AnnotationPanelViewModel : ViewModelBase
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

        public ICommand AddDrawCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }

        public AnnotationInfo AnnotationInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public AnnotationPanelViewModel(IContainerExtension container) : base(container)
        {
            AddDrawCommand = new DelegateCommand(AddDraw);
            ClearCommand = new DelegateCommand(Clear);
            ExportCommand = new DelegateCommand(Export);

            AnnotationInfo = Container.Resolve<DataManager>().AnnotationInfo;

            FontItemList = new List<string>();
            FontItemList.Add("돋음");

            SelectedFontItem = FontItemList[0];
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
        /// Export 이벤트
        /// </summary>
        private void Export()
        {
            EventAggregator.GetEvent<DrawExportEvent>().Publish();
        }
    }
}
