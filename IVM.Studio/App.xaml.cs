using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;
using DevExpress.Xpf.Core;
using Prism.Mvvm;
using IVM.Studio.Mvvm;

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
            theme.AssemblyName = "DevExpress.Xpf.Themes.IVM_Theme.v21.1";
            Theme.RegisterTheme(theme);
            ApplicationThemeHelper.ApplicationThemeName = "IVM_Theme";

            //Dark color variation  
            //Telerik.Windows.Controls.Office2019Palette.LoadPreset(Telerik.Windows.Controls.Office2019Palette.ColorVariation.Dark);
            Telerik.Windows.Controls.FluentPalette.LoadPreset(Telerik.Windows.Controls.FluentPalette.ColorVariation.Dark);

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<FileService>();
            containerRegistry.RegisterSingleton<ImageService>();
            containerRegistry.RegisterSingleton<WindowByChannelService>();
            containerRegistry.RegisterSingleton<WindowByHistogramService>();
            containerRegistry.RegisterSingleton<SlideShowService>();
            containerRegistry.RegisterSingleton<DataManager>();
            containerRegistry.RegisterSingleton<VideoTrimService>();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(new ViewModelResolver(() => Container).UseDefaultConfigure().ResolveViewModelForView);
        }
    }
}
