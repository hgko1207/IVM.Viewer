using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using DevExpress.Xpf.Core;

namespace IVM.Studio
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var theme = new Theme("IVM_Theme");
            theme.AssemblyName = "DevExpress.Xpf.Themes.IVM_Theme.v20.1";
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "IVM_Theme";

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<FileService>(); 
            containerRegistry.RegisterSingleton<WindowByChannelService>();
        }
    }
}
