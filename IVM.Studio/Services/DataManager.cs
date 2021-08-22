using IVM.Studio.Models;
using IVM.Studio.Models.Views;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static IVM.Studio.Models.Common;
using WPFDrawing = System.Windows.Media;

/**
 * @Class Name : DataManager.cs
 * @Description : 데이터 관리 서비스
 * @author 고형균
 * @since 2021.06.05
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class DataManager
    {
        /// <summary> 현재 선택된 슬라이드 정보 </summary>
        public SlideInfo SelectedSlideInfo { get; set; }

        public string CurrentSlidesPath { get; set; }

        /// <summary>표시 되고 있는 파일</summary>
        public FileInfo CurrentFile { get; set; }

        /// <summary>메타데이터 정보</summary>
        public Metadata Metadata { get; set; }

        public WindowInfo WindowInfo { get; set; }

        public string ViewerName { get; set; }

        public Dictionary<ChannelType, ColorChannelModel> ColorChannelInfoMap { get; set; }

        public List<ColorChannelItem> ColorChannelItems { get; set; }

        public WPFDrawing.ImageSource HistogramImage { get; set; }

        public AnnotationInfo AnnotationInfo { get; set; }

        public SliderControlInfo SliderControlInfo { get; set; }

        public MeasurementInfo MeasurementInfo { get; set; }

        public I3DChannelInfo I3DChannelInfo1 { get; set; }
        public I3DChannelInfo I3DChannelInfo2 { get; set; }
        
        public I3DBackgroundInfo I3DBackgroundInfo1 { get; set; }
        public I3DBackgroundInfo I3DBackgroundInfo2 { get; set; }

        public I3DRecordInfo I3DRecordInfo { get; set; }

        public int MainWindowSeq { get; set; }

        public int MainWindowId { get; set; }

        public IEnumerable<string> ImageFileExtensions;
        public IEnumerable<string> VideoFileExtensions;
        public IEnumerable<string> ApprovedExtensions => Enumerable.Concat(ImageFileExtensions, VideoFileExtensions);

        public void Init(IContainerExtension container, IEventAggregator eventAggregator)
        {
            ColorChannelInfoMap = new Dictionary<ChannelType, ColorChannelModel>
            {
                { ChannelType.DAPI, new ColorChannelModel(ChannelType.DAPI, "DAPI (425-465)", true, Colors.Red, false, 0, 1, 0, 255, container, eventAggregator) },
                { ChannelType.GFP, new ColorChannelModel(ChannelType.GFP, "GFP (500-550)", true, Colors.Green, false, 0, 1, 0, 255, container, eventAggregator) },
                { ChannelType.RFP, new ColorChannelModel(ChannelType.RFP, "RFP (582-618)", true, Colors.Blue, false, 0, 1, 0, 255, container, eventAggregator) },
                { ChannelType.NIR, new ColorChannelModel(ChannelType.NIR, "NIR (663-733)", false, Colors.None, false, 0, 1, 0, 255, container, eventAggregator) }
            };

            ColorChannelItems = new List<ColorChannelItem>() {
                new ColorChannelItem() { Name = "DAPI", Type = ChannelType.DAPI },
                new ColorChannelItem() { Name = "GFP", Type = ChannelType.GFP },
                new ColorChannelItem() { Name = "RFP", Type = ChannelType.RFP },
                new ColorChannelItem() { Name = "NIR", Type = ChannelType.NIR }
            };

            AnnotationInfo = new AnnotationInfo(container, eventAggregator);
            SliderControlInfo = new SliderControlInfo(container, eventAggregator);
            MeasurementInfo = new MeasurementInfo(container, eventAggregator);
            I3DChannelInfo1 = new I3DChannelInfo(container, eventAggregator, 1);
            I3DChannelInfo2 = new I3DChannelInfo(container, eventAggregator, 2);
            I3DBackgroundInfo1 = new I3DBackgroundInfo(container, eventAggregator, 1);
            I3DBackgroundInfo2 = new I3DBackgroundInfo(container, eventAggregator, 2);
            I3DRecordInfo = new I3DRecordInfo(container, eventAggregator);

            MainWindowSeq = 0;

            ImageFileExtensions = new[] { ".ivm" };
            VideoFileExtensions = new[] { ".avi" };
        }
    }
}
