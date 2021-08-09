using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using IVM.Studio.I3D;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DSliceViewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<I3DSliceViewer>
    {
        private I3DSliceViewer view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DSliceViewerViewModel(IContainerExtension container) : base(container)
        {
        }

        public void OnLoaded(I3DSliceViewer view)
        {
            this.view = view;

            view.isv.param.RENDER_MODE = I3DRenderMode.OBLIQUE;

            EventAggregator.GetEvent<I3DOpenEvent>().Subscribe(Open, ThreadOption.UIThread);
        }

        public void OnUnloaded(I3DSliceViewer view)
        {
            EventAggregator.GetEvent<I3DOpenEvent>().Unsubscribe(Open);
        }

        private void Open(string path)
        {
            view.isv.Open(path);
        }
    }
}
