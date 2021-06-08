using IVM.Studio.Mvvm;
using Prism.Ioc;

/**
 * @Class Name : AnnotationViewModel.cs
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
    public class AnnotationViewModel : ViewModelBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public AnnotationViewModel(IContainerExtension container) : base(container)
        {
        }
    }
}
