using IVM.Studio.Models;

/**
 * @Class Name: CommonUtil.cs
 * @Description: 공통유틸
 * @author 고형균
 * @since 2021.08.03
 * @version 1.0
 */
namespace IVM.Studio.Utils
{
    public class CommonUtil
    {
        /// <summary>
        /// TimaSpan To Mask
        /// </summary>
        /// <param name="type"></param>
        public static string TimaSpanToMask(TimeSpanType type)
        {
            switch (type)
            {
                case TimeSpanType.hh_mm_ss:
                    return "HH:mm:ss";
                case TimeSpanType.hh_mm:
                    return "HH:mm";
                case TimeSpanType.mm_ss:
                    return "mm:ss";
                case TimeSpanType.sec:
                    return "ss 'sec'";
                case TimeSpanType.min:
                    return "mm 'min'";
                case TimeSpanType.h:
                    return "HH 'h'";
                default:
                    return "HH:mm:ss";
            }
        }

        /// <summary>
        /// ZStackLabel To Mask
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ZStackLabelToMask(ZStackLabelType type)
        {
            switch (type)
            {
                case ZStackLabelType.Label1:
                case ZStackLabelType.Label3:
                    return "000.0";
                case ZStackLabelType.Label2:
                case ZStackLabelType.Label4:
                    return "000";
                default:
                    return "";
            }
        }
    }
}
