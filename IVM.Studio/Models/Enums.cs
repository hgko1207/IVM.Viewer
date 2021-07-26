using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/**
 * @Class Name : Enums.cs
 * @Description : Enum 공통 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.08     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.08
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public class Enums
    {
    }

    public enum PositionType
    {
        [Display(Name = "Left", Description = "Left", Order = 1)]
        LEFT,
        [Display(Name = "Right", Description = "Right", Order = 2)]
        RIGHT
    }

    public enum ZStackProjectionType
    {
        [Description("Maximum")]
        Maximum,
        [Description("Minimum")]
        Minimum,
        [Description("Average intensity")]
        Average
    }

    public enum EdgeCropType
    {
        [Description("None")]
        None,
        [Description("Default")]
        Default,
        [Description("Minimum")]
        Minimum,
        [Description("Maximum")]
        Maximum
    }

    public enum ColocalizationType
    {
        [Description("Marker Ch.")]
        Marker,
        [Description("Target Ch.")]
        Target,
        [Description("Colocalizaed")]
        Colocalizaed
    }

    public enum ProductType
    {
        [Description("0.10X")]
        Product_10,
        [Description("0.20X")]
        Product_20,
        [Description("0.25X")]
        Product_25,
        [Description("0.33X")]
        Product_33,
        [Description("0.5X")]
        Product_50,
        [Description("0.75X")]
        Product_75
    }
}
