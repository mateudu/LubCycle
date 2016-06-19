using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Utils;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        private Template10.Services.SerializationService.ISerializationService _SerializationService;
        public readonly int[] BikeCountItems = new[] { 0, 1, 2, 3, 4, 5 };
        private int _selectedBikes = 0;

        private readonly LubCycleHelper _lubcycle = new LubCycleHelper();
        private readonly ObservableCollection<StationsListViewItem> MapItemsSource = new ObservableCollection<StationsListViewItem>();
        private List<StationsListViewItem> StationsListViewItems = new List<StationsListViewItem>();
        private List<Place> _stations = new List<Place>();
        private Geoposition _position = null;

        public StationsPage()
        {
            this.InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            stationsMap.MapServiceToken = StaticData.MapServiceToken;
            stationsMap.Center = StaticData.DefaultMapCenter;
            stationsMap.ZoomLevel = 15.0;
        }

        private async void StationsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadPageAsync();
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            await LoadPageAsync();
        }

        private void BikeCountPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? val = (sender as ComboBox).SelectedItem as int?;
            _selectedBikes = val.Value;
            ReloadList();
        }

        private async Task LoadStationsAndPositionAsync()
        {
            var pos = LocationHelper.GetCurrentLocationAsync();
            var stations = _lubcycle.GetStationsAsync();

            await Task.WhenAll(pos, stations);
            _stations = stations.Result ?? _stations;
            _position = pos.Result ?? _position;

            if (stations.Result == null)
            {
                try
                {
                    var dlg = new MessageDialog("Wystąpił problem z połączeniem z serwisem.");
                    await dlg.ShowAsync();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            StationsListViewItems.Clear();

            foreach (var obj in _stations)
            {
                if (obj.Bikes == @"5+")
                    obj.Bikes = "5";
                StationsListViewItems.Add(
                    new StationsListViewItem()
                    {
                        Geopoint = new Geopoint(
                        new BasicGeoposition()
                        {
                            Latitude = obj.Lat,
                            Longitude = obj.Lng,
                        }),
                        Station = obj,
                        Distance = GeoHelper.CalcDistanceInMeters(
                            _position.Coordinate.Latitude,
                            _position.Coordinate.Longitude,
                            obj.Lat,
                            obj.Lng)
                    });
            }
        }

        private async Task LoadPageAsync()
        {
            try
            {
                refreshButton.IsEnabled = false;
                filterButton.IsEnabled = false;
                await LoadStationsAndPositionAsync();
                ReloadList();

                stationsMap.Center = _position != null ? _position.Coordinate.Point : StaticData.DefaultMapCenter;
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
                filterButton.IsEnabled = true;
            }
        }

        private void ReloadList()
        {
            stationsMap.MapElements.Clear();
            var obj = StationsListViewItems.Where(x => int.Parse(x.Station.Bikes) >= _selectedBikes).ToList();
            obj.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            MapItemsSource.AddRange(obj, true);
            if (_position != null)
            {
                var poi = new MapIcon { Location = _position.Coordinate.Point, NormalizedAnchorPoint = new Point(0.5, 1.0), Title = "Moja pozycja", ZIndex = 0 };
                stationsMap.MapElements.Add(poi);
            }
        }

        private async void StationsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = (sender as ListView).SelectedItem as StationsListViewItem;
            await stationsMap.TrySetViewAsync(obj.Geopoint, 15.0);
        }
    }
}