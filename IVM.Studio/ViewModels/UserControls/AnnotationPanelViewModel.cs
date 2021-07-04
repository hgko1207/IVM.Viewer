using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Ioc;
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
        private AnnotationInfo annotationInfo;
        public AnnotationInfo AnnotationInfo
        {
            get => annotationInfo;
            set => SetProperty(ref annotationInfo, value);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public AnnotationPanelViewModel(IContainerExtension container) : base(container)
        {
            AnnotationInfo = Container.Resolve<DataManager>().AnnotationInfo;
        }
    }
}
