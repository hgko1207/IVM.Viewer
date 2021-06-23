using IVM.Studio.ViewModels;
using IVM.Studio.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * @Class Name : VideoTrimService.cs
 * @Description : Video Trim 관리 서비스
 * @author 고형균
 * @since 2021.06.22
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class VideoTrimService
    {
        private VideoTrimWindow videoTrimWindow;

        public void ShowTrimWindow(FileInfo file, TimeSpan length)
        {
            if (videoTrimWindow == null)
            {
                videoTrimWindow = new VideoTrimWindow();
                if (videoTrimWindow.DataContext is VideoTrimWindowViewModel vm)
                {
                    vm.File = file;
                    vm.Length = length;
                    vm.FromMin = 0;
                    vm.FromSec = 0;
                    vm.ToMin = (int)length.TotalMinutes;
                    vm.ToSec = (int)length.TotalSeconds;
                }
                videoTrimWindow.Show();
            }
            else
            {
                videoTrimWindow.Activate();
            }
        }

        public void ClosedWindow()
        {
            videoTrimWindow = null;
        }
    }
}
