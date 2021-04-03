using IVM.Studio.ViewModels;
using IVM.Studio.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;

/**
 * @Class Name : WindowByChannelService.cs
 * @Description : 채널별 윈도우 관리 서비스
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.04.04     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.04.04
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class WindowByChannelService
    {
        private DisplayWindow[] displayWindows;

        /// <summary>
        /// 생성자
        /// </summary>
        public WindowByChannelService()
        {
            displayWindows = new DisplayWindow[4] {
                null, null, null, null
            };
        }

        /// <summary>
        /// Display 창 열기
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="alwaysTop"></param>
        public void ShowDisplay(int channel, bool alwaysTop)
        {
            if (channel < 0 || channel >= 4)
                return;

            if (displayWindows[channel] == null)
            {
                displayWindows[channel] = new DisplayWindow();
                ChangeOwner(channel, alwaysTop);
            }

            // 저장된 위치 불러오기
            string pos = ConfigurationManager.AppSettings.Get($"Channel{channel}Position");
            if (pos != null)
            {
                List<double> parsedPosition = pos.Split(';').Select(s => Convert.ToDouble(s)).ToList();
                displayWindows[channel].Top = parsedPosition[0];
                displayWindows[channel].Left = parsedPosition[1];
                displayWindows[channel].Width = parsedPosition[2];
                displayWindows[channel].Height = parsedPosition[3];
            }

            // 띄우기
            displayWindows[channel].Show();
            if (displayWindows[channel].DataContext is DisplayWindowViewModel vm)
            {
                vm.Title = $"Display #{channel + 1}";
                vm.Channel = channel;
            }
        }

        /// <summary>
        /// Display 창 닫기
        /// </summary>
        /// <param name="channel"></param>
        public void CloseDisplay(int channel)
        {
            if (channel < 0 || channel >= 4) 
                return;

            if (displayWindows[channel] != null)
            {
                // 위치 저장
                string key = $"Channel{channel}Position";
                string value = $"{displayWindows[channel].Top};{displayWindows[channel].Left};{displayWindows[channel].Width};{displayWindows[channel].Height}";

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element != null)
                    element.Value = value;
                else 
                    config.AppSettings.Settings.Add(key, value);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // 닫기
                displayWindows[channel].Close();
                displayWindows[channel] = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="image"></param>
        public void DisplayImage(int channel, ImageSource image)
        {
            if (channel < 0 || channel >= 4 || displayWindows[channel] == null)
                return;

            displayWindows[channel].Dispatcher.Invoke(() => {
                if (displayWindows[channel].DataContext is DisplayWindowViewModel vm)
                {
                    vm.DisplayImage = image;
                }
            });
        }

        /// <summary>
        /// Owner 전환
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="enableAlwaysTop"></param>
        public void ChangeOwner(int channel, bool enableAlwaysTop)
        {
            if (channel < 0 || channel >= 4 || displayWindows[channel] == null)
                return;

            if (enableAlwaysTop)
            {
                displayWindows[channel].Owner = Application.Current.MainWindow;
                displayWindows[channel].Activate();
            }
            else
            {
                displayWindows[channel].Owner = null;
                Application.Current.MainWindow.Activate();
            }
        }
    }
}
