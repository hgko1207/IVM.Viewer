using IVM.Studio.Models.Events;
using IVM.Studio.MvvM;
using IVM.Studio.Views;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;

namespace IVM.Studio.ViewModels.UserControls
{
    public class ImageAdjustmentViewModel : ViewModelBase
    {
        private bool allChecked;
        public bool AllChecked
        {
            get => allChecked;
            set
            {
                if (SetProperty(ref allChecked, value))
                {
                    if (value)
                        new MainViewerWindow().Show();
                    else
                        EventAggregator.GetEvent<MainViewerCloseEvent>().Publish();
                }
            }
        }

        private bool dapiChecked;
        public bool DAPIChecked
        {
            get => dapiChecked;
            set
            {
                if (SetProperty(ref dapiChecked, value))
                {
                    if (value)
                        new ChannelViewerWindow().Show();
                    else
                        EventAggregator.GetEvent<ChWindowClosedEvent>().Publish(1);
                }
            }
        }

        public ICommand LevelLockCommand { get; private set; }
        public ICommand LevelResetCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentViewModel(IContainerExtension container) : base(container)
        {
            LevelLockCommand = new DelegateCommand(LevelLock);
            LevelResetCommand = new DelegateCommand(LevelReset);
        }

        /// <summary>
        /// 잠금
        /// </summary>
        private void LevelLock()
        {

        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void LevelReset()
        {

        }
    }
}
