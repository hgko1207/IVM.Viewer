using Prism.Mvvm;

/**
 * @Class Name : MeasurementInfo.cs
 * @Description : Measurement 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.08.17     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.08.17
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public class MeasurementData
    {
        public int Line { get; set; }

        public int Seq { get; set; }

        public string Length { get; set; }

        public int StartX { get; set; }

        public int StartY { get; set; }

        public int EndX { get; set; }

        public int EndY { get; set; }
    }
}
