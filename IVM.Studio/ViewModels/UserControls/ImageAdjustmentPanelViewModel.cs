using IVM.Studio.MvvM;
using Prism.Ioc;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ImageAdjustmentPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentPanelViewModel(IContainerExtension container) : base(container)
        {
        }
    }
}
