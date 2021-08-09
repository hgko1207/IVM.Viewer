using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

/**
 * @Class Name : MoveThumb.cs
 * @Description : Thumb 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.07.31     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.07.31
 * @version 1.0
 */
namespace IVM.Studio.Models
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += MoveThumbDragDelta;
        }

        private void MoveThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DataContext is Control designerItem)
            {
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                Canvas.SetLeft(designerItem, left + e.HorizontalChange);
                Canvas.SetTop(designerItem, top + e.VerticalChange);
            }
        }
    }

    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumbDragDelta;
        }

        private void ResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DataContext is Control designerItem)
            {
                double delta;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        delta = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height -= delta;
                        break;
                    case VerticalAlignment.Top:
                        delta = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + delta);
                        designerItem.Height -= delta;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        delta = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + delta);
                        designerItem.Width -= delta;
                        break;
                    case HorizontalAlignment.Right:
                        delta = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width -= delta;
                        break;
                    default:
                        break;
                }

                // Shift를 누른 상태일시 높이를 너비와 똑같이 바꿔서 정사각형으로 보정함
                // 꼭지점을 잡고 있는 상태에서만 동작
                if (Keyboard.Modifiers == ModifierKeys.Shift && VerticalAlignment != VerticalAlignment.Stretch && HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    delta = designerItem.Height - designerItem.Width;
                    if (VerticalAlignment == VerticalAlignment.Top)
                    {
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + delta);
                    }
                    designerItem.Height -= delta;
                }
            }

            e.Handled = true;
        }
    }
}
