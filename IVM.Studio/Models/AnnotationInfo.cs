using IVM.Studio.Models.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * @Class Name : Annotation.cs
 * @Description : Annotation 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.23     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.23
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public class AnnotationInfo : BindableBase
    {
        public int PenThickness { get; set; }

        private int scaleBarSize;
        public int ScaleBarSize
        {
            get => scaleBarSize;
            set
            {
                if (SetProperty(ref scaleBarSize, value))
                    EventAggregator.GetEvent<RefreshImageEvent>().Publish();
            }
        }

        public IEventAggregator EventAggregator;

        public AnnotationInfo(IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
        }
    }
}
