using System;
using LubCycle.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace LubCycle.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            stationsMap.MapServiceToken = StaticData.MapServiceToken;
            stationsMap.Center = StaticData.DefaultMapCenter;
            stationsMap.ZoomLevel = 15.0;
            ViewModel.MapControl = stationsMap;
            ViewModel.PinpointFlyout = Resources["PinpointFlyout"] as MenuFlyout;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().BackRequested -= MainPage_BackRequested;
        }
        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            App.Current.Exit();
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.MainPage_OnLoaded(sender,e);
        }

        private void Pinpoint_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.Pinpoint_OnTapped(sender, e);
        }

        private void Pinpoint_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ViewModel.Pinpoint_OnTapped(sender, null);
        }
    }
}
