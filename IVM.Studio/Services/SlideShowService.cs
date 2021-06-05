using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* 슬라이드쇼 서비스의 사용 방법
* 
* 1. StartNewSlideshow 메서드를 호출해서 몇회 반복할지 횟수(슬라이더 길이 - 1)와 반복 딜레이를 지정한다.
* 2. 첫 이미지를 표시한다. 슬라이더가 1번 위치에 있지 않으면 이동시킨다.
* 3. 이미지 표시가 완료되거나 동영상 재생이 완료되거나 재생 중지되면, ContinueSlideshow 메서드를 호출한다.
* 3-1. (2)에서 슬라이더가 이미 1번 위치에 있을 경우 (3)을 즉시 수동으로 처리한다.
* 4. ContinueSlideshow 메서드는 (1)에서 지정한 딜레이만큼 대기한 후 반복 횟수를 1 감소시키고 PlaySlideshowEvent 이벤트를 발생시킨다.
* 5. 뷰 모델 측에서 PlaySlideshowEvent를 받아 슬라이더를 한 칸 진행시킨다.
* 6. 3~5를 반복하다가 슬라이더가 마지막 위치로 오면 뷰 모델에서 이를 감지하여 종료한다.
*/
namespace IVM.Studio.Services
{
    public class SlideShowService
    {
        protected IEventAggregator EventAggregator;

        public SlideShowService(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        /// <summary>
        /// 기존 슬라이드쇼 태스크의 카운트를 1 진행합니다. 카운트가 진행될 때마다 지정한 시간만큼 대기한 후 <seealso cref="PlaySlideshowEvent"/> 이벤트를 발생시킵니다.
        /// </summary>
        /// <returns>이미 슬라이드쇼가 진행중이 아닌 경우 실행되지 않으며 이 경우 false가 반환됩니다. 실행에 성공하면 true를 반환합니다.</returns>
        public bool ContinueSlideshow()
        {
            return false;
        }
    }
}
