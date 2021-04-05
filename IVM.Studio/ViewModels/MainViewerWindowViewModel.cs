using IVM.Studio.MvvM;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVM.Studio.ViewModels
{
    public class MainViewerWindowViewModel : ViewModelBase
    {
        public MainViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Viewer";
        }
    }
}
