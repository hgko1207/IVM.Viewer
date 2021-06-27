using IVM.Studio.Mvvm;
using IVM.Studio.Views;
using Prism.Ioc;
using System.Windows.Media;

/**
 * @Class Name : ChannelHistogramWindowViewModel.cs
 * @Description : 채널별 히스토그램 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.27     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.27
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class ChannelHistogramWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ChannelHistogramWindow>
    {
        private ImageSource displayHistogram;
        public ImageSource DisplayHistogram
        {
            get => displayHistogram;
            set => SetProperty(ref displayHistogram, value);
        }

        public int Channel { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelHistogramWindowViewModel(IContainerExtension container) : base(container)
        {
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ChannelHistogramWindow view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ChannelHistogramWindow view)
        {
        }
    }
}
