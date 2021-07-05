using Prism.Mvvm;

/**
* @Class Name : SliderControlInfo.cs
* @Description : Slider 제어 모델
* @
* @ 수정일         수정자              수정내용
* @ ----------   ---------   -------------------------------
* @ 2021.07.05     고형균              최초생성
*
* @author 고형균
* @since 2021.07.05
* @version 1.0
*/
namespace IVM.Studio.Models.Views
{
    public class SliderControlInfo : BindableBase
    {
        public int ZSSliderValue { get; set; }
        public int ZSSliderMinimum { get; set; }
        public int ZSSliderMaximum { get; set; }

        public int MSSliderValue { get; set; }
        public int MSSliderMinimum { get; set; }
        public int MSSliderMaximum { get; set; }

        public int MPSliderValue { get; set; }
        public int MPSliderMinimum { get; set; }
        public int MPSliderMaximum { get; set; }

        public int TLSliderValue { get; set; }
        public int TLSliderMinimum { get; set; }
        public int TLSliderMaximum { get; set; }
    }
}
