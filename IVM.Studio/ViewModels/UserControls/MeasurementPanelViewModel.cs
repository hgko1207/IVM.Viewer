using IVM.Studio.Models;
using IVM.Studio.Mvvm;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Ioc;
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
        private ObservableCollection<MeasurementInfo> measurementInfoCollection;
        public ObservableCollection<MeasurementInfo> MeasurementInfoCollection => measurementInfoCollection ?? (measurementInfoCollection = new ObservableCollection<MeasurementInfo>());

        private MeasurementInfo selectedMeasurementInfo;
        public MeasurementInfo SelectedMeasurementInfo
        {
            get => selectedMeasurementInfo;
            set => SetProperty(ref selectedMeasurementInfo, value);
        }

        public ICommand AddCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MeasurementPanelViewModel(IContainerExtension container) : base(container)
        {
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete);
            SaveCommand = new DelegateCommand(Save);
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
        /// 추가 이벤트
        /// </summary>
        private void Add()
        {

        }

        /// <summary>
        /// 삭제 이벤트
        /// </summary>
        private void Delete()
        {

        }

        /// <summary>
        /// 표 전체를 csv로 저장
        /// </summary>
        private void Save()
        {

        }
    }
}
