using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

/**
 * @Class Name : MeasurementInfo.cs
 * @Description : 측정 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.08.22     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.08.22
 * @version 1.0
 */
namespace IVM.Studio.Models.Views
{
    public class MeasurementInfo : BindableBase
    {
        private bool measurementEnabled;
        public bool MeasurementEnabled
        {
            get => measurementEnabled;
            set
            {
                if (SetProperty(ref measurementEnabled, value))
                {
                    if (value)
                        container.Resolve<DataManager>().AnnotationInfo.AllUnChecked();

                    eventAggregator.GetEvent<DrawMeasurementEvent>().Publish(value);
                }
            }

        }

        private readonly IContainerExtension container;
        private readonly IEventAggregator eventAggregator;

        public MeasurementInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.container = container;
            this.eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Reset Checked
        /// </summary>
        public void ResetChecked()
        {
            if (MeasurementEnabled)
            {
                MeasurementEnabled = false;
                MeasurementEnabled = true;
            }
        }
    }
}
