using IVM.Studio.Mvvm;
using IVM.Studio.Views;
using Prism.Ioc;

/**
* @Class Name : HistogramWindowViewModel.cs
* @Description : 히스토그램 뷰어 뷰 모델
* @
* @ 수정일         수정자              수정내용
* @ ----------   ---------   -------------------------------
* @ 2021.06.06     고형균              최초생성
*
* @author 고형균
* @since 2021.06.06
* @version 1.0
*/
namespace IVM.Studio.ViewModels
{
    public class HistogramWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<HistogramWindow>
    {
        private HistogramWindow view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public HistogramWindowViewModel(IContainerExtension container) : base(container)
        {
        }

        public void OnLoaded(HistogramWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(HistogramWindow view)
        {
        }
    }
}
