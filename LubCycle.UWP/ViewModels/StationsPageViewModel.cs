using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;

namespace LubCycle.UWP.ViewModels
{
    class StationsPageViewModel : ViewModelBase
    {
        public StationsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }

            MapItemsSource = new ObservableCollection<StationsListViewItem>();
            Position = null;
            StationsListViewItems = new List<StationsListViewItem>();
            _lubcycle = new LubCycleHelper();
            _stations = new List<Place>();
        }
        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadPageAsync();
        }

        ///////////////////////////////////////////////////////
        public readonly int[] BikeCountItems = new[] { 0, 1, 2, 3, 4, 5 };
        private readonly LubCycleHelper _lubcycle;
        private List<Place> _stations;
        private Geoposition position;
        private int selectedBikes = 0;
        public readonly ObservableCollection<StationsListViewItem> MapItemsSource;
        private List<StationsListViewItem> StationsListViewItems;

        public MapControl MapControl;


        private bool _refreshButtonEnabled = false;

        public bool RefreshButtonEnabled
        {
            get { return _refreshButtonEnabled; }
            set { Set(ref _refreshButtonEnabled, value); }
        }

        private bool _filterButtonEnabled = false;

        public bool FilterButtonEnabled
        {
            get { return _filterButtonEnabled; }
            set { Set(ref _filterButtonEnabled, value); }
        }

        public int SelectedBikes
        {
            get { return SelectedBikes1; }
            set { SelectedBikes1 = value; }
        }

        public int SelectedBikes1
        {
            get { return selectedBikes; }
            set { selectedBikes = value; }
        }

        public Geoposition Position
        {
            get { return position; }
            set { position = value; }
        }

        public void BikeCountPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? number = (sender as ComboBox).SelectedItem as int?;
            if (number.HasValue)
            {
                SelectedBikes = number.Value;
                ReloadList();
            }
        }
        public async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            await LoadPageAsync();
        }
        public async Task StationsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // App may crash without this Try/Catch during Reloading StationsListViewItems.
            try
            {
                var obj = (sender as ListView).SelectedItem as StationsListViewItem;
                await MapControl.TrySetViewAsync(obj.Geopoint, 15.0);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task LoadPageAsync()
        {
            try
            {
                RefreshButtonEnabled = false;
                FilterButtonEnabled = false;
                await LoadStationsAndPositionAsync();
                ReloadList();
                await MapControl.TrySetViewAsync(Position != null ? Position.Coordinate.Point : StaticData.DefaultMapCenter, 15.0);
                
            }
            catch (Exception exc)
            {
                var dlg = new MessageDialog(exc.Message);
                await dlg.ShowAsync();
            }
            finally
            {
                RefreshButtonEnabled = true;
                FilterButtonEnabled = true;
            }
        }

        private void ReloadList()
        {
            MapControl.MapElements.Clear();
            var obj = StationsListViewItems.Where(x => int.Parse(x.Station.Bikes) >= SelectedBikes).ToList();
            obj.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            MapItemsSource.AddRange(obj, true);

            if (Position != null)
            {
                var poi = new MapIcon { Location = Position.Coordinate.Point, NormalizedAnchorPoint = new Point(0.5, 1.0), Title = "Moja pozycja", ZIndex = 0 };
                MapControl.MapElements.Add(poi);
            }
        }
        private async Task LoadStationsAndPositionAsync()
        {
            var pos = LocationHelper.GetCurrentLocationAsync();
            var stations = _lubcycle.GetStationsAsync();

            await Task.WhenAll(pos, stations);
            _stations = stations.Result ?? _stations;
            Position = pos.Result ?? Position;

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
                            Position?.Coordinate.Latitude,
                            Position?.Coordinate.Longitude,
                            obj.Lat,
                            obj.Lng)
                    });
            }
        }
    }
}
