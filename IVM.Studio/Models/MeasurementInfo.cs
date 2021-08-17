using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class MeasurementInfo
    {
        public int Line { get; set; }

        public int Seq { get; set; }

        public double Length { get; set; }

        public int StartX { get; set; }

        public int StartY { get; set; }

        public int EndX { get; set; }

        public int EndY { get; set; }
    }
}
