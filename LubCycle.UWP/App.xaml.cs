using Windows.UI.Xaml;
using System.Threading.Tasks;
using LubCycle.UWP.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using LubCycle.UWP.Helpers;

namespace LubCycle.UWP
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);
            //ApplicationView.PreferredLaunchWindowingMode=ApplicationViewWindowingMode.Auto;
            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null)
            {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }

            // Assign brushes
            try
            {
                if (this.Resources["CustomColor"] != null)
                {
                    CacheData.CustomColor = (Color) this.Resources["CustomColor"];
                }
                if (this.Resources["ContrastColor"] != null)
                {
                    CacheData.ContrastColor = (Color)this.Resources["ContrastColor"];
                }
                if (this.Resources["SystemAccentColor"] != null)
                {
                    CacheData.SystemAccentColor = (Color)this.Resources["SystemAccentColor"];
                }
            }
            catch (Exception)
            {
                
            }

            //PC customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = CacheData.SystemAccentColor;
                    titleBar.ButtonForegroundColor = CacheData.ContrastColor;
                    titleBar.BackgroundColor = CacheData.CustomColor;
                    titleBar.ForegroundColor = CacheData.ContrastColor;
                }
            }

            //Mobile customization
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {

                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = CacheData.CustomColor;
                    statusBar.ForegroundColor = CacheData.ContrastColor;
                }
            }

            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            var locationTask = LocationHelper.GetCurrentLocationAsync();
            await Task.Delay(2000);

            NavigationService.Navigate(typeof(Views.MainPage));

            await Task.CompletedTask;
        }
    }
}

