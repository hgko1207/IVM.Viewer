using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.MvvM;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
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
                        new ImageViewerWindow().Show();
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

        public ICommand ColorResetCommand { get; set; }
        public ICommand AllVisibleCommand { get; set; }

        public ICommand LevelLockCommand { get; private set; }
        public ICommand LevelResetCommand { get; private set; }

        public Dictionary<int, ColorChannelModel> ColorChannelInfoMap { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageAdjustmentViewModel(IContainerExtension container) : base(container)
        {
            ColorResetCommand = new DelegateCommand(ColorReset);
            AllVisibleCommand = new DelegateCommand(AllVisible);
            LevelLockCommand = new DelegateCommand(LevelLock);
            LevelResetCommand = new DelegateCommand(LevelReset);

            ColorChannelInfoMap = container.Resolve<DataManager>().ColorChannelInfoMap;
        }

        private void ColorReset()
        {

        }

        private void AllVisible()
        {

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
