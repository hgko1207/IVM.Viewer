using Prism.Events;
using System;
using System.IO;

/**
 * @Class Name : Events.cs
 * @Description : 이벤트 관리 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.30     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.30
 * @version 1.0
 */
namespace IVM.Studio.Models.Events
{
    /// <summary> 메인 뷰어 윈도우가 종료 될때 </summary>
    public class MainViewerClosedEvent : PubSubEvent { }
    public class MainViewerUnloadEvent : PubSubEvent<int> { }
    public class MainWindowDeactivatedEvent : PubSubEvent<int> { }

    /// <summary> 메인 Viewer를 변경 할 때 </summary>
    public class ViewerPageChangeEvent : PubSubEvent { }
    /// <summary> 메인 Viewer가 변경 될 때 </summary>
    public class ViewerPageChangedEvent : PubSubEvent { }
    public class RefreshFolderEvent : PubSubEvent<DirectoryInfo> { }

    /// <summary>히스토그램 윈도우를 종료 할때</summary>
    public class HistogramCloseEvent : PubSubEvent { }
    /// <summary>히스토그램 윈도우가 종료 될때</summary>
    public class HistogramClosedEvent : PubSubEvent { }
    public class RefreshMainHistogramEvent : PubSubEvent { }
    public class RefreshChHistogramEvent : PubSubEvent<ChannelType> { }

    /// <summary> 메타데이터 새로고침 </summary>
    public class RefreshMetadataEvent : PubSubEvent<DisplayParam> { }

    /// <summary>표시중인 이미지를 갱신하는 이벤트. 밝기, 대비 등의 색상 정보가 변화하거나 반전, 회전 등으로 이미지 정보가 변화할 때마다 발생합니다.</summary>
    public class RefreshImageEvent : PubSubEvent<int> { }

    /// <summary>채널별 윈도우를 종료 할때</summary>
    public class ChViewerWindowCloseEvent : PubSubEvent<int> { }
    /// <summary>채널별 윈도우가 종료 될때</summary>
    public class ChViewerWindowClosedEvent : PubSubEvent<ChannelType> { }

    public class ChHistogramWindowCloseEvent : PubSubEvent<int> { }
    public class ChHistogramWindowClosedEvent : PubSubEvent<ChannelType> { }

    /// <summary>주어진 이미지를 읽어들여 이미지 페이지에 표시하는 이벤트</summary>
    public class DisplayImageEvent : PubSubEvent<DisplayParam> { }
    public class DisplayVideoEvent : PubSubEvent<DisplayParam> { }
    public class DisplayParam
    {
        public readonly FileInfo FileInfo;
        public readonly Metadata Metadata;
        public readonly bool SlideChanged;

        public DisplayParam(FileInfo fileInfo, Metadata metadata, bool slideChanged)
        {
            FileInfo = fileInfo;
            Metadata = metadata;
            SlideChanged = slideChanged;
        }
    }

    /// <summary>Draw 이벤트</summary>
    public class DrawClearEvent : PubSubEvent { }
    public class DrawUndoEvent : PubSubEvent { }
    public class DrawRedoEvent : PubSubEvent { }

    /// <summary>Export 이벤트</summary>
    public class ExportDrawEvent : PubSubEvent { }
    public class ExportDrawAllEvent : PubSubEvent { }

    public class RotationEvent : PubSubEvent<string> { }
    public class ReflectEvent : PubSubEvent<string> { }
    public class RotationResetEvent : PubSubEvent { }

    /// <summary>Slide 관련 이벤트</summary>
    public class DisplaySlideEvent : PubSubEvent<bool> { }

    public class PlaySlideShowEvent : PubSubEvent { }
    public class StopSlideShowEvent : PubSubEvent { }
    public class EnableImageSlidersEvent : PubSubEvent<SlidersParam> { }
    public class SlidersParam
    {
        public string CurrentSlidesPath { get; set; }
        public string SlideName { get; set; }
    }

    /// <summary>Video 관련 이벤트</summary>
    public class PlayVideoEvent : PubSubEvent { }
    public class PauseVideoEvent : PubSubEvent { }
    public class StopVideoEvent : PubSubEvent { }
    public class SeekVideoEvent : PubSubEvent<TimeSpan> { }

    public class PlayingVideoEvent : PubSubEvent<PlayingVideoParam> { }
    public class PlayingVideoParam
    {
        public readonly TimeSpan VideoLength;
        public readonly TimeSpan VideoCurrentTime;

        public PlayingVideoParam(TimeSpan VideoCurrentTime, TimeSpan VideoLength)
        {
            this.VideoLength = VideoLength;
            this.VideoCurrentTime = VideoCurrentTime;
        }
    }
    public class TrimWindowClosedEvent : PubSubEvent { }

    /// <summary>TextAnnotation 관련 이벤트</summary>
    public class TextAnnotationDialogEvent : PubSubEvent<TextAnnotationDialogParam> { }
    public class TextAnnotationDialogParam
    {
        public readonly string Title;
        public readonly string Content;
        public readonly int X;
        public readonly int Y;

        public TextAnnotationDialogParam(string Title, string Content, int X, int Y)
        {
            this.Title = Title;
            this.Content = Content;
            this.X = X;
            this.Y = Y;
        }
    }

    public class TextAnnotationEvent : PubSubEvent<TextAnnotationParam> { }
    public class TextAnnotationParam
    {
        public readonly int X;
        public readonly int Y;
        public readonly string Text;

        public TextAnnotationParam(int X, int Y, string Text)
        {
            this.X = X;
            this.Y = Y;
            this.Text = Text;
        }
    }

    public class ZoomRatioControlEvent : PubSubEvent<int> { }

    public class EnableDrawEvent : PubSubEvent { }
    public class DisableDrawEvent : PubSubEvent { }

    /// <summary>Crop 관련 이벤트</summary>
    public class EnableCropEvent : PubSubEvent { }
    public class DisableCropEvent : PubSubEvent { }

    public class DrawCropBoxEvent : PubSubEvent<DrawParam> { }
    public class DrawCropCircleEvent : PubSubEvent<DrawParam> { }
    public class DrawCropTriangleEvent : PubSubEvent<DrawParam> { }
    public class DrawParam
    {
        public readonly double Left;
        public readonly double Top;
        public readonly double Width;
        public readonly double Height;

        public DrawParam(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }

    public class GetPositionToCropEvent : PubSubEvent<GetPositionToCropParam> { }
    public class GetPositionToCropParam
    {
        public double HorizontalOffset;
        public double VerticalOffset;
        public double ViewportWidth;
        public double ViewportHeight;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public bool Routed;
    }

    public class ExportCropEvent : PubSubEvent { }
    public class ExportAllCropEvent : PubSubEvent { }

    public class NavigatorChangeEvent : PubSubEvent<NavigatorParam> { }
    public class NavigatorParam
    {
        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }
        public int ZoomRatio { get; set; }
    }

    public class DrawMeasurementEvent : PubSubEvent<bool> { }
    public class AddMeasurementEvent : PubSubEvent<MeasurementData> { }

    public class I3DWindowLoadedEvent : PubSubEvent<int> { }

    public class I3DMetaLoadedParam
    {
        public int width { get; set; }
        public int height { get; set; }
        public int depth { get; set; }

        public float umWidth { get; set; }
        public float umHeight { get; set; }
        public float umPerPixelZ { get; set; }
    }

    public class I3DMetaLoadedEvent : PubSubEvent<I3DMetaLoadedParam> { }

    public class I3DCameraUpdateParam
    {
        public int viewtype { get; set; }
        public float px { get; set; }
        public float py { get; set; }
        public float pz { get; set; }
        public float ax { get; set; }
        public float ay { get; set; }
        public float az { get; set; }
        public float s { get; set; }
    }

    public class I3DCameraUpdateEvent : PubSubEvent<I3DCameraUpdateParam> { }

    public class I3DFirstRenderEvent : PubSubEvent<int> { }

    public class I3DMainViewVisibleChangedEvent : PubSubEvent<bool> { }
    public class I3DSliceViewVisibleChangedEvent : PubSubEvent<bool> { }
    
}
