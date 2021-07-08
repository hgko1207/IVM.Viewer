using IVM.Studio.Models;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Ioc;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : ChannelProcessingPanelViewModel.cs
 * @Description : Channel Processing 화면 뷰 모델
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
    public class ChannelProcessingPanelViewModel : ViewModelBase
    {
        private ColocalizationType selectedColocalization;
        public ColocalizationType SelectedColocalization
        {
            get => selectedColocalization;
            set => SetProperty(ref selectedColocalization, value);
        }

        private ProductType selectedProductType;
        public ProductType SelectedProductType
        {
            get => selectedProductType;
            set => SetProperty(ref selectedProductType, value);
        }

        public List<ColorChannelItem> ColorChannelItems { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelProcessingPanelViewModel(IContainerExtension container) : base(container)
        {
            ColorChannelItems = Container.Resolve<DataManager>().ColorChannelItems;

            SelectedColocalization = ColocalizationType.Colocalizaed;
            SelectedProductType = ProductType.Product_33;
        }
    }
}
