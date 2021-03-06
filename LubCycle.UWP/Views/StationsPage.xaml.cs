﻿using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        private Template10.Services.SerializationService.ISerializationService _SerializationService;
        
        public StationsPage()
        {
            this.InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            stationsMap.MapServiceToken = StaticData.MapServiceToken;
            stationsMap.Center = StaticData.DefaultMapCenter;
            stationsMap.ZoomLevel = StaticData.DefaultMapZoom;
            ViewModel.MapControl = stationsMap;
            ViewModel.PinpointFlyout = Resources["PinpointFlyout"] as MenuFlyout;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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