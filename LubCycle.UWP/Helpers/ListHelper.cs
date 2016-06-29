using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using LubCycle.UWP.Models;
using Template10.Utils;

namespace LubCycle.UWP.Helpers
{
    class ListHelper
    {
        public static void ReloadList(ref ObservableCollection<StationsListViewItem> obs, Func<StationsListViewItem,bool> pred = null)
        {
            List<StationsListViewItem> obj;
            if (pred != null)
            {
                obj = CacheData.StationListViewItems?.Where(pred).ToList();
            }
            else
            {
                obj = CacheData.StationListViewItems?.ToList();
            }
            obj?.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            obs.AddRange(obj, true);
        }

        public static async Task LoadStationsAndPositionAsync(bool reloadRequested = false)
        {
            var pos = LocationHelper.GetCurrentLocationAsync();
            if (reloadRequested == true || CacheData.Stations == null || CacheData.Stations.Count == 0 || 
                CacheData.StationListViewItems==null || CacheData.StationListViewItems.Count==0)
            {
                var stations = new LubCycleHelper().GetStationsAsync();
                await Task.WhenAll(pos, stations);

                CacheData.Position = pos.Result ?? CacheData.Position;
                CacheData.Stations = stations.Result ?? CacheData.Stations;
                if (CacheData.Stations != null)
                {
                    CacheData.StationListViewItems = new List<StationsListViewItem>();
                    foreach (var obj in CacheData.Stations)
                    {
                        if (obj.Bikes == @"5+")
                            obj.Bikes = "5";
                        CacheData.StationListViewItems.Add(
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
                                    CacheData.Position?.Coordinate.Point.Position.Latitude,
                                    CacheData.Position?.Coordinate.Point.Position.Longitude,
                                    obj.Lat,
                                    obj.Lng)
                            });
                    }
                }
                else
                {
                    throw new Exception("API connection failure.");
                }
            }
            else
            {
                await Task.WhenAll(pos);
                CacheData.Position = pos.Result ?? CacheData.Position;
                foreach (var obj in CacheData.StationListViewItems)
                {
                    obj.Distance = GeoHelper.CalcDistanceInMeters(
                        CacheData.Position?.Coordinate.Point.Position.Latitude,
                        CacheData.Position?.Coordinate.Point.Position.Longitude,
                        obj.Geopoint.Position.Latitude,
                        obj.Geopoint.Position.Longitude);
                }
            }
        }
    }
}
