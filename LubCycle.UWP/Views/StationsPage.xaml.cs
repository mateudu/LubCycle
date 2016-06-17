﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;
        private LubCycleHelper _lubcycle = new LubCycleHelper();
        ObservableCollection<StationsListViewItem> StationListViewItems = new ObservableCollection<StationsListViewItem>();
        private List<Place> _stations = new List<Place>();
        private Geoposition _position = null;

        private string _locationMessage;
        private string _apiMessage;

        public StationsPage()
        {
            this.InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            stationsMap.MapServiceToken = @"ApcN9o_xREqWLKuVV0MKfqmsd2hapuXA-Jo-mhuhJunA6XLF-Bgi-goFFp4PgEZu";
        }

        private async void StationsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            await ReloadPage();
        }

        private void BikeCountSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ReloadList();
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            await ReloadPage();
        }

        private async Task<Geoposition> GetPositionAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            Geoposition pos;
            try
            {
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                        Geolocator geolocator = new Geolocator {DesiredAccuracyInMeters = 100};
                        pos = await geolocator.GetGeopositionAsync();
                        return pos;

                    case GeolocationAccessStatus.Denied:
                        break;

                    case GeolocationAccessStatus.Unspecified:
                        break;
                }

            }
            catch (Exception)
            {

            }
            return null;
        }


        private async Task ReloadStationsAndPositionAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            var pos = GetPositionAsync();
            var stations = _lubcycle.GetStationsAsync();

            await Task.WhenAll(pos, stations);
            _stations = stations.Result ?? _stations;
            _position = pos.Result ?? _position;

            foreach (var obj in _stations)
            {
                if (obj.Bikes == @"5+")
                    obj.Bikes = "5";
            }

            if (stations.Result == null)
            {
                try
                {
                    var dlg = new MessageDialog("Wystąpił problem z połączeniem z serwisem.");
                    dlg.ShowAsync();
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private async Task ReloadPage()
        {
            try
            {
                stationsMap.Center = new Geopoint(new BasicGeoposition() {Latitude = 51.2465, Longitude = 22.5684});
                refreshButton.IsEnabled = false;
                bikeCountSlider.IsEnabled = false;
                await ReloadStationsAndPositionAsync();
                ReloadList();
                if (_position != null)
                {
                    stationsMap.Center = _position.Coordinate.Point;
                    MapIcon POI = new MapIcon { Location = _position.Coordinate.Point, NormalizedAnchorPoint = new Point(0.5, 1.0), Title = "Moja pozycja", ZIndex = 0 };
                    stationsMap.MapElements.Add(POI);
                }

                stationsMap.ZoomLevel = 15.0;
            }
            catch (Exception exc)
            {
                var dlg = new MessageDialog(exc.Message);
                await dlg.ShowAsync();
            }
            finally
            {
                refreshButton.IsEnabled = true;
                bikeCountSlider.IsEnabled = true;
            }
        }

        private void ReloadList()
        {
            stationsMap.MapElements.Clear();
            var list = new List<StationsListViewItem>();
            _stations.Where(x => int.Parse(x.Bikes) >= (int) bikeCountSlider.Value).ForEach(x=>
            {
                list.Add(
                    new StationsListViewItem
                    {
                        Station = x
                    });
                Geopoint myPoint = new Geopoint(new BasicGeoposition() { Latitude = x.Lat, Longitude = x.Lng });
                MapIcon myPOI = new MapIcon { Location = myPoint, NormalizedAnchorPoint = new Point(0.5, 1.0), Title = x.Name, ZIndex = 0 };
                stationsMap.MapElements.Add(myPOI);
            });
            StationListViewItems.AddRange(list, true);
        }
    }
}
