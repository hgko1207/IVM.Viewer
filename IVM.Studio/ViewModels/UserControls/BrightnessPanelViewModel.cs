using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.Windows.Input;

/**
 * @Class Name : BrightnessPanelViewModel.cs
 * @Description : Brightness/Contrast 조정 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.03     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.03
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class BrightnessPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<BrightnessPanel>
    {
        private float allBrightness;
        public float AllBrightness
        {
            get => allBrightness;
            set
            {
                SetProperty(ref allBrightness, value);
                bool refresh = false;
                foreach (ColorChannelModel i in colorChannelInfoMap.Values)
                    refresh |= i.UpdateBrightnessWithoutRefresh(value);
                if (refresh)
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private float allContrast;
        public float AllContrast
        {
            get => allContrast;
            set
            {
                SetProperty(ref allContrast, value);
                bool refresh = false;
                foreach (ColorChannelModel i in colorChannelInfoMap.Values)
                    refresh |= i.UpdateContrastWithoutRefresh(value);
                if (refresh)
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish(dataManager.MainWindowId);
            }
        }

        private bool isLock;
        public bool IsLock
        {
            get => isLock;
            set => SetProperty(ref isLock, value);
        }

        public ICommand ResetCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        public ColorChannelModel DAPIChannel { get; set; }
        public ColorChannelModel GFPChannel { get; set; }
        public ColorChannelModel RFPChannel { get; set; }
        public ColorChannelModel NIRChannel { get; set; }

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public BrightnessPanelViewModel(IContainerExtension container) : base(container)
        {
            ResetCommand = new DelegateCommand(Reset);

            EventAggregator.GetEvent<RefreshMetadataEvent>().Subscribe(RefreshMetadata, ThreadOption.UIThread);

            dataManager = container.Resolve<DataManager>();

            colorChannelInfoMap = dataManager.ColorChannelInfoMap;
            DAPIChannel = colorChannelInfoMap[ChannelType.DAPI];
            GFPChannel = colorChannelInfoMap[ChannelType.GFP];
            RFPChannel = colorChannelInfoMap[ChannelType.RFP];
            NIRChannel = colorChannelInfoMap[ChannelType.NIR];

            Reset();

            IsLock = true;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(BrightnessPanel view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(BrightnessPanel view)
        {
            EventAggregator.GetEvent<RefreshMetadataEvent>().Unsubscribe(RefreshMetadata);
        }

        /// <summary>
        /// 메타데이터 변경 시
        /// </summary>
        /// <param name="param"></param>
        private void RefreshMetadata(DisplayParam param)
        {
            if (!IsLock)
                Reset();
        }

        /// <summary>
        /// Reset 이벤트
        /// </summary>
        private void Reset()
        {
            AllBrightness = 0;
            AllContrast = 1;
        }
    }
}
