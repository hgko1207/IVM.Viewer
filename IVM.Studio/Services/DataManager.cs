using IVM.Studio.Models;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>표시 되고 있는 파일</summary>
        public FileInfo CurrentFile { get; set; }

        /// <summary>메타데이터 정보</summary>
        public Metadata Metadata { get; set; }

        /// <summary>선택된 파일 정보</summary>
        public SlideInfo SelectedSlideInfo { get; set; }

        public Dictionary<ChannelType, ColorChannelModel> ColorChannelInfoMap { get; set; }

        /// <summary>선택된 패널 정보</summary>
        public ColorChannelModel SelectedChannel { get; set; }

        public WPFDrawing.ImageSource HistogramImage { get; set; }

        public void Init(IContainerExtension container, IEventAggregator eventAggregator)
        {
            ColorChannelInfoMap = new Dictionary<ChannelType, ColorChannelModel>();
            ColorChannelInfoMap.Add(ChannelType.DAPI, new ColorChannelModel(ChannelType.DAPI, "DAPI (425-465)", true, Colors.Red, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.GFP, new ColorChannelModel(ChannelType.GFP, "GFP (500-550)", true, Colors.Green, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.RFP, new ColorChannelModel(ChannelType.RFP, "RFP (582-618)", true, Colors.Blue, false, 0, 1, 0, 255, container, eventAggregator));
            ColorChannelInfoMap.Add(ChannelType.NIR, new ColorChannelModel(ChannelType.NIR, "NIR (663-733)", false, Colors.None, false, 0, 1, 0, 255, container, eventAggregator));

            SelectedChannel = ColorChannelInfoMap[ChannelType.DAPI];
        }
    }
}
