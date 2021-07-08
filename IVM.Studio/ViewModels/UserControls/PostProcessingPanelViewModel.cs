using IVM.Studio.Models;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Ioc;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : PostProcessingPanelViewModel.cs
 * @Description : Post Processing 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.08     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.08
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class PostProcessingPanelViewModel : ViewModelBase
    {
        private ColorChannelItem selectedTimeLapseItem;
        public ColorChannelItem SelectedTimeLapseItem
        {
            get => selectedTimeLapseItem;
            set => SetProperty(ref selectedTimeLapseItem, value);
        }

        private ColorChannelItem selectedZStackItem;
        public ColorChannelItem SelectedZStackItem
        {
            get => selectedZStackItem;
            set => SetProperty(ref selectedZStackItem, value);
        }

        private ColorChannelItem selectedRotationItem;
        public ColorChannelItem SelectedRotationItem
        {
            get => selectedRotationItem;
            set => SetProperty(ref selectedRotationItem, value);
        }

        private ZStackProjectionType selectedProjection;
        public ZStackProjectionType SelectedProjection
        {
            get => selectedProjection;
            set => SetProperty(ref selectedProjection, value);
        }

        private EdgeCropType selectedEdgeCrop;
        public EdgeCropType SelectedEdgeCrop
        {
            get => selectedEdgeCrop;
            set => SetProperty(ref selectedEdgeCrop, value);
        }

        public List<ColorChannelItem> ColorChannelItems { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public PostProcessingPanelViewModel(IContainerExtension container) : base(container)
        {
            ColorChannelItems = Container.Resolve<DataManager>().ColorChannelItems;

            SelectedTimeLapseItem = ColorChannelItems[0];
            SelectedZStackItem = ColorChannelItems[0];
            SelectedRotationItem = ColorChannelItems[0];

            SelectedProjection = ZStackProjectionType.Maximum;
            SelectedEdgeCrop = EdgeCropType.Minimum;
        }
    }
}
