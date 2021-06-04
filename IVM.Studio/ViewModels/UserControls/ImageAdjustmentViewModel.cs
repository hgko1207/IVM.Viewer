using IVM.Studio.MvvM;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ImageAdjustmentViewModel : ViewModelBase
    {
        public ICommand LockCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentViewModel(IContainerExtension container) : base(container)
        {
            LockCommand = new DelegateCommand(Lock);
            ResetCommand = new DelegateCommand(Reset);
        }

        /// <summary>
        /// 잠금
        /// </summary>
        private void Lock()
        {

        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Reset()
        {

        }
    }
}
