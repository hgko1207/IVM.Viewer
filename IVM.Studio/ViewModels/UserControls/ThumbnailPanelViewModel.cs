using IVM.Studio.Mvvm;
using Prism.Ioc;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ThumbnailPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ThumbnailPanelViewModel(IContainerExtension container) : base(container)
        {
        }
    }
}
