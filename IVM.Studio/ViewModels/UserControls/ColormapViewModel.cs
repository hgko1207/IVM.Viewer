using IVM.Studio.Models;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Ioc;
using System.Collections.Generic;

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
        private ColorChannelModel selectedChannel;
        public ColorChannelModel SelectedChannel
        {
            get => selectedChannel;
            set => SetProperty(ref selectedChannel, value);
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

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ColormapViewModel(IContainerExtension container) : base(container)
        {
            SelectedChannel = Container.Resolve<DataManager>().SelectedChannel;
        }
    }
}
