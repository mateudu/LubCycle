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
        public static void ReloadList(ref List<StationsListViewItem> slvi, ref ObservableCollection<StationsListViewItem> obs, Func<StationsListViewItem,bool> pred = null)
        {
            //MapControl.MapElements.Clear();
            List<StationsListViewItem> obj;
            if (pred != null)
            {
                obj = slvi.Where(pred).ToList();
            }
            else
            {
                obj = slvi.ToList();
            }
            obj.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            obs.AddRange(obj, true);

            //if (Position != null)
            //{
            //    var poi = new MapIcon { Location = Position.Coordinate.Point, NormalizedAnchorPoint = new Point(0.5, 1.0), Title = "Moja pozycja", ZIndex = 0 };
            //    MapControl.MapElements.Add(poi);
            //}
        }

        public static async Task<List<StationsListViewItem>> LoadStationsAndPositionAsync(bool reloadRequested = false)
        {
            var list = new List<StationsListViewItem>();
            var pos = LocationHelper.GetCurrentLocationAsync();
            if (reloadRequested == true || CacheData.Stations == null || CacheData.Stations.Count == 0)
            {
                var stations = new LubCycleHelper().GetStationsAsync();
                await Task.WhenAll(pos, stations);

                CacheData.Stations = stations.Result ?? CacheData.Stations;
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
            }
            else
            {
                await Task.WhenAll(pos);
            }

            CacheData.Position = pos.Result ?? CacheData.Position;
            if (CacheData.Stations != null)
            {
                foreach (var obj in CacheData.Stations)
                {
                    if (obj.Bikes == @"5+")
                        obj.Bikes = "5";
                    list.Add(
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
                                CacheData.Position?.Coordinate.Latitude,
                                CacheData.Position?.Coordinate.Longitude,
                                obj.Lat,
                                obj.Lng)
                        });
                }
            }
            return list;
        }
    }
}
