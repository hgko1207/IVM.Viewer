using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Models.Views;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

/**
 * @Class Name : MeasurementPanelViewModel.cs
 * @Description : 측정 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.08.17     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.08.17
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class MeasurementPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<MeasurementPanel>
    {
        private ObservableCollection<MeasurementData> measurementInfoCollection;
        public ObservableCollection<MeasurementData> MeasurementInfoCollection => measurementInfoCollection ?? (measurementInfoCollection = new ObservableCollection<MeasurementData>());

        private MeasurementData selectedMeasurementInfo;
        public MeasurementData SelectedMeasurementInfo
        {
            get => selectedMeasurementInfo;
            set => SetProperty(ref selectedMeasurementInfo, value);
        }

        public ICommand DeleteCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public MeasurementInfo MeasurementInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MeasurementPanelViewModel(IContainerExtension container) : base(container)
        {
            DeleteCommand = new DelegateCommand(Delete);
            ClearCommand = new DelegateCommand(Clear);
            SaveCommand = new DelegateCommand(Save);

            MeasurementInfo = Container.Resolve<DataManager>().MeasurementInfo;

            EventAggregator.GetEvent<AddMeasurementEvent>().Subscribe(AddMeasurement);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MeasurementPanel view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MeasurementPanel view)
        {
        }

        /// <summary>
        /// Add Measurement
        /// </summary>
        /// <param name="measurementInfo"></param>
        private void AddMeasurement(MeasurementData measurementInfo)
        {
            double length = Math.Sqrt(Math.Pow(measurementInfo.StartX - measurementInfo.EndX, 2) 
                + Math.Pow(measurementInfo.StartY - measurementInfo.EndY, 2));
            measurementInfo.Length = length.ToString("F2");

            MeasurementInfoCollection.Add(measurementInfo);
        }

        /// <summary>
        /// 삭제 이벤트
        /// </summary>
        private void Delete()
        {
            if (SelectedMeasurementInfo != null)
            {
                MeasurementInfoCollection.Remove(SelectedMeasurementInfo);
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Clear()
        {
            MeasurementInfoCollection.Clear();
        }

        /// <summary>
        /// 표 전체를 csv로 저장
        /// </summary>
        private void Save()
        {

        }
    }
}
