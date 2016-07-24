using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using LubCycle.Core.Mobile.Helpers;
using LubCycle.Core.Mobile.Models;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;

namespace LubCycle.UWP.ViewModels
{
    class RouteOverviewPageViewModel : ViewModelBase
    {
        private LubCycleHelper _lubCycleHelper;
        public MapControl MapControl;
        public readonly ObservableCollection<StationsListViewItem> MapItemsSource;
        public RouteOverviewPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
            _lubCycleHelper = new LubCycleHelper();
            MapItemsSource = new ObservableCollection<StationsListViewItem>();
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
            Route = CacheData.CurrentRoute;
            LoadRouteOnMap();
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

        private Route _route;

        public Route Route
        {
            get { return _route; }
            set { Set(ref _route, value); }
        }

        private async Task LoadRouteOnMap()
        {
            try
            {
                var obj = Route.Stations.Select(x => new StationsListViewItem()
                {
                    Station = x,
                    Geopoint = new Geopoint(
                        new BasicGeoposition()
                        {
                            Latitude = x.Lat,
                            Longitude = x.Lng
                        })
                });
                if (obj != null)
                {
                    MapItemsSource.AddRange(obj, true);
                }

                for (int i = 1; i < Route.Stations.Count; i++)
                {
                    var startPos = new BasicGeoposition()
                    {
                        Latitude = Route.Stations[i - 1].Lat,
                        Longitude = Route.Stations[i - 1].Lng
                    };
                    var endPos = new BasicGeoposition()
                    {
                        Latitude = Route.Stations[i].Lat,
                        Longitude = Route.Stations[i].Lng
                    };
                    var routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                        new Geopoint(startPos),
                        new Geopoint(endPos),
                        MapRouteOptimization.Distance,
                        MapRouteRestrictions.None);
                    if (routeResult.Status == MapRouteFinderStatus.Success)
                    {
                        // Use the route to initialize a MapRouteView.
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = StaticData.RouteColors[i % StaticData.RouteColors.Length];
                        viewOfRoute.OutlineColor = Colors.Black;

                        // Add the new MapRouteView to the Routes collection
                        // of the MapControl.
                        MapControl.Routes.Add(viewOfRoute);

                        // Fit the MapControl to the route.
                        await MapControl.TrySetViewBoundsAsync(
                              routeResult.Route.BoundingBox,
                              null,
                              Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                    }
                }
                await MapControl.TrySetViewAsync(
                    new Geopoint(new BasicGeoposition()
                    {
                        Latitude = Route.Start.Lat,
                        Longitude = Route.Start.Lng
                    }),
                    StaticData.DefaultMapZoom);
            }
            catch (Exception)
            {
                
            }
            
        }
        public async void RemoveSLVI_OnClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is StationsListViewItem)
            {
                var obj = (sender as Button).Tag as StationsListViewItem;
                MapItemsSource.Remove(obj);
            }
            if (MapItemsSource.Count >= 1)
            {
                await MapControl.TrySetViewAsync(MapItemsSource[0].Geopoint, StaticData.DefaultMapZoom);
            }
        }

        public async void NavigateSLVI_OnClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is StationsListViewItem)
            {
                var obj = (sender as Button).Tag as StationsListViewItem;
                if (obj != null)
                {
                    var uri = new Uri($"ms-walk-to:destination.latitude={obj.Station.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&" +
                                  $"destination.longitude={obj.Station.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
                    await Launcher.LaunchUriAsync(uri);
                }
            }
        }
    }
}
