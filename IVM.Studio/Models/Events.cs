using Prism.Events;
using System.IO;

namespace IVM.Studio.Models.Events
{
    /// <summary> 메인 이미지 윈도우를 종료 할때 </summary>
    public class ImageViewerCloseEvent : PubSubEvent { }
    /// <summary> 메인 이미지 윈도우가 종료 될때 </summary>
    public class ImageViewerClosedEvent : PubSubEvent { }

    /// <summary>표시중인 이미지를 갱신하는 이벤트. 밝기, 대비 등의 색상 정보가 변화하거나 반전, 회전 등으로 이미지 정보가 변화할 때마다 발생합니다.</summary>
    public class RefreshImageEvent : PubSubEvent { }

    /// <summary>채널별 윈도우가 종료되었을때</summary>
    public class ChWindowClosedEvent : PubSubEvent<int> { }

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
            this.FileInfo = fileInfo;
            this.Metadata = metadata;
            this.SlideChanged = slideChanged;
        }
    }

    public class RotationEvent : PubSubEvent<string> { }
    public class ReflectEvent : PubSubEvent<string> { }
}
