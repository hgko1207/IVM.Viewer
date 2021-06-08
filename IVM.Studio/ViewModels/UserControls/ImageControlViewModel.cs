using IVM.Studio.Mvvm;
using Prism.Ioc;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

/**
 * @Class Name : ImageControlViewModel.cs
 * @Description : 이미지 컨트롤 화면 뷰 모델
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
    public class ImageControlViewModel : ViewModelBase
    {
        public List<int> PenThicknessList { get; }
        public List<int> EraserSizeList { get; }
        public List<int> FontSizeList { get; }
        public List<FontItem> FontItemList { get; }

        private int selectedPenThickness;
        public int SelectedPenThickness
        {
            get => selectedPenThickness;
            set => SetProperty(ref selectedPenThickness, value);
        }

        public int selectedEraserSize;
        public int SelectedEraserSize
        {
            get => selectedEraserSize;
            set => SetProperty(ref selectedEraserSize, value);
        }

        public int selectedFontSize;
        public int SelectedFontSize
        {
            get => selectedFontSize;
            set => SetProperty(ref selectedFontSize, value);
        }

        public FontItem selectedFontItem;
        public FontItem SelectedFontItem
        {
            get => selectedFontItem;
            set => SetProperty(ref selectedFontItem, value);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ImageControlViewModel(IContainerExtension container) : base(container)
        {
            PenThicknessList = new List<int>();
            for (int i = 1; i <= 10; i++)
                PenThicknessList.Add(i);

            EraserSizeList = new List<int>();
            for (int i = 1; i <= 50; i++)
                EraserSizeList.Add(i);

            FontSizeList = new List<int>();
            for (int i = 1; i <= 100; i++)
                FontSizeList.Add(i);

            FontItemList = new List<FontItem>();
            FontItemList.Add(new FontItem() { Type = "1", Name = "Times New Roman" });

            SelectedPenThickness = 1;
            SelectedEraserSize = 30;
            SelectedFontSize = 40;
            SelectedFontItem = FontItemList[0];
        }
    }
}
