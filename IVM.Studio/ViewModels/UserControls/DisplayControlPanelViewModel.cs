using IVM.Studio.Mvvm;
using Prism.Ioc;

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
    public class DisplayControlPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public DisplayControlPanelViewModel(IContainerExtension container) : base(container)
        {
        }
    }
}
