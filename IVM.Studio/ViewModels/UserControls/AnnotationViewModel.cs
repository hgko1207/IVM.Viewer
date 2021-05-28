using IVM.Studio.MvvM;
using Prism.Ioc;
using Unity;

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
