using IVM.Studio.Models;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : ColormapViewModel.cs
 * @Description : 컬러 맵 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.10     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.10
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class ColormapViewModel : ViewModelBase
    {
        private List<ColorChannelItem> colorChannelItems;
        public List<ColorChannelItem> ColorChannelItems
        {
            get => colorChannelItems;
            set => SetProperty(ref colorChannelItems, value);
        }

        private ColorChannelItem selectedChannel;
        public ColorChannelItem SelectedChannel
        {
            get => selectedChannel;
            set => SetProperty(ref selectedChannel, value);
        }

        private ColorMap selectedColorMap;
        public ColorMap SelectedColorMap
        {
            get => selectedColorMap;
            set => SetProperty(ref selectedColorMap, value);
        }

        public IEnumerable<ColorMap> ColorMaps
        {
            get
            {
                return new[] { ColorMap.Hot, ColorMap.Cool, ColorMap.Inferno, ColorMap.Bone, ColorMap.Jet,
                    ColorMap.Rainbow, ColorMap.Autumn, ColorMap.Ocean, ColorMap.TwilightShifted,
                    ColorMap.Winter, ColorMap.Summer, ColorMap.Spring, ColorMap.Hsv, ColorMap.Pink,
                    ColorMap.Parula, ColorMap.Magma, ColorMap.Plasma, ColorMap.Viridis, ColorMap.Cividis, ColorMap.Twilight
                };
            }
        }

        public ICommand ApplyCommand { get; private set; }
        public ICommand ExportLabelCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ColormapViewModel(IContainerExtension container) : base(container)
        {
            ApplyCommand = new DelegateCommand(Apply);
            ExportLabelCommand = new DelegateCommand(ExportLabel);

            SelectedColorMap = ColorMaps.SingleOrDefault(color => color == ColorMap.Hot);

            ColorChannelItems = Container.Resolve<DataManager>().ColorChannelItems.Where(item => item.Type != ChannelType.ALL).ToList();
            SelectedChannel = ColorChannelItems[0];

            colorChannelInfoMap = Container.Resolve<DataManager>().ColorChannelInfoMap;
        }

        /// <summary>
        /// 적용 이벤트
        /// </summary>
        private void Apply()
        {
            colorChannelInfoMap[SelectedChannel.Type].ColorMap = SelectedColorMap;
            colorChannelInfoMap[SelectedChannel.Type].ColorMapEnabled = true;
        }

        /// <summary>
        /// ExportLabel
        /// </summary>
        private void ExportLabel()
        {

        }
    }
}
