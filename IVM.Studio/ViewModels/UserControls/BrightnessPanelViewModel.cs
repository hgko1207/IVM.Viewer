using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Commands;
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
    public class BrightnessPanelViewModel : ViewModelBase
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
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        private float _DAPIBrightness;
        public float DAPIBrightness
        {
            get => _DAPIBrightness;
            set
            {
                if (SetProperty(ref _DAPIBrightness, value))
                    colorChannelInfoMap[ChannelType.DAPI].Brightness = value;
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
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        public ICommand LockCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        public ColorChannelModel DAPIChannel { get; set; }
        public ColorChannelModel GFPChannel { get; set; }
        public ColorChannelModel RFPChannel { get; set; }
        public ColorChannelModel NIRChannel { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public BrightnessPanelViewModel(IContainerExtension container) : base(container)
        {
            LockCommand = new DelegateCommand(Lock);
            ResetCommand = new DelegateCommand(Reset);

            colorChannelInfoMap = container.Resolve<DataManager>().ColorChannelInfoMap;

            AllBrightness = 0;
            DAPIBrightness = 0;

            AllContrast = 1;

            DAPIChannel = colorChannelInfoMap[ChannelType.DAPI];
            GFPChannel = colorChannelInfoMap[ChannelType.GFP];
            RFPChannel = colorChannelInfoMap[ChannelType.RFP];
            NIRChannel = colorChannelInfoMap[ChannelType.NIR];
        }

        /// <summary>
        /// Lock 이벤트
        /// </summary>
        private void Lock()
        {

        }

        /// <summary>
        /// Reset 이벤트
        /// </summary>
        private void Reset()
        {

        }
    }
}
