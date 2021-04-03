using IVM.Studio.MvvM;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;
using System.Windows.Media;

/**
 * @Class Name : DisplayWindowViewModel.cs
 * @Description : 영상 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.04.04     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.04.04
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class DisplayWindowViewModel : ViewModelBase
    {
        private ImageSource displayImage;
        public ImageSource DisplayImage
        {
            get => displayImage;
            set => SetProperty(ref displayImage, value);
        }

        public ICommand ClosedCommand { get; private set; }

        public int Channel;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public DisplayWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Display";

            ClosedCommand = new DelegateCommand(Closed);
        }

        /// <summary>
        /// 닫기 이벤트
        /// </summary>
        private void Closed()
        {

        }
    }
}
