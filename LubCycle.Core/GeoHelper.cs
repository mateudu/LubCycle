using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models;
using LubCycle.Core.Models.Geo;
using LubCycle.Core.Models.NextBike;

namespace LubCycle.Core
{
    public class GeoHelper
    {
        // Invoke 'LoadGraph()' method before calling 'GetRoute()'! 
        //private async Task LoadGraph()
        //{
        //    if (Core.GeoHelper.TravelDurations == null || Core.GeoHelper.TravelDurations.Count == 0)
        //    {
        //        var obj = _context.TravelDurations.ToList();
        //        Core.GeoHelper.TravelDurations = obj;
        //    }
        //    if (Core.GeoHelper.Stations == null)
        //    {
        //        Core.GeoHelper.Stations = await LubCycle.Core.NextBikeHelper.GetStationsAsync(Startup.Configuration["CITY_UIDS"]);
        //    }
        //}

        // Calc distance using Geo-coordinates.
        private const double EQuatorialEarthRadius = 6378.1370D;
        private const double D2R = (Math.PI / 180D);

        /// <summary>
        ///  Returns distance from A to B in km.
        /// </summary>
        /// <param name="lat1">Point A - Latitude</param>
        /// <param name="lng1">Point A - Longitude</param>
        /// <param name="lat2">Point B - Latitude</param>
        /// <param name="lng2">Point B - Longitude</param>
        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double dlong = (lng2 - lng1) * D2R;
            double dlat = (lat2 - lat1) * D2R;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * D2R) * Math.Cos(lat2 * D2R) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = EQuatorialEarthRadius * c;
            return d;
        }

        /// <summary>
        ///  Returns distance between station A and B in km.
        /// </summary>
        /// <param name="start">Station A</param>
        /// <param name="destination">Station B</param>
        public static double CalcDistance(Models.NextBike.Place start, Models.NextBike.Place destination)
        {
            return CalcDistance(start.Lat, start.Lng, destination.Lat, destination.Lng);
        }
        
        // Dijkstra's algorithm graph
        private static List<List<Models.Geo.Edge>> _graph;
        private static Dictionary<string, int> _edges;
        public static List<TravelDuration> TravelDurations;
        public static List<Place> Stations;
        private static bool _isInitialized = false;
        private static void InitializeGraph()
        {
            if (_isInitialized == false)
            {
                int from, to;
                double duration, distance;

                _graph = new List<List<Edge>>();
                _edges = new Dictionary<string, int>();

                for (int i = 0; i < Stations.Count; i++)
                {
                    _edges[Stations[i].Uid] = i;
                    _graph.Add(new List<Edge>());
                }
                for (int i = 0; i < TravelDurations.Count; i++)
                {
                    from = _edges[TravelDurations[i].Station1Uid];
                    to = _edges[TravelDurations[i].Station2Uid];
                    distance = TravelDurations[i].Distance;
                    duration = TravelDurations[i].Duration;
                    _graph[from].Add(new Edge
                    {
                        Distance = distance,
                        Duration = duration,
                        To = to
                    });
                    _graph[to].Add(new Edge
                    {
                        Distance = distance,
                        Duration = duration,
                        To = from
                    });
                }
                _isInitialized = true;
            }
        }

        private static List<Place> CalcRoute(string startUid, string destUid)
        {
            InitializeGraph();
            int i, j, k, u, v;
            Edge pw;
            v = _edges[startUid];

            double[] D = new double[Stations.Count];
            int[] P = new int[Stations.Count];
            bool[] QS = new bool[Stations.Count];
            int[] S = new int[Stations.Count];
            int sptr = 0;

            for (i = 0; i < Stations.Count; i++)
            {
                D[i] = (double)Int32.MaxValue;
                P[i] = -1;
                QS[i] = false;
            }

            D[v] = 0;

            for (i = 0; i < Stations.Count; i++)
            {
                for (j = 0; QS[j]; j++);
                for (u = j++; j < Stations.Count; j++)
                {
                    if (!QS[j] && (D[j] < D[u])) u = j;
                }

                QS[u] = true;

                for (k = 0; k < _graph[u].Count; k++)
                {
                    pw = _graph[u][k];
                    if (!QS[pw.To] && (D[pw.To] > D[u] + pw.Duration))
                    {
                        D[pw.To] = D[u] + pw.Duration;
                        P[pw.To] = u;
                    }
                }
            }

            for (j = _edges[destUid]; j > -1; j = P[j])
                S[sptr++] = j;

            var result = new List<Place>();
            while (sptr > 0) /*cout << S[--sptr] << " ";*/
            {
                result.Add(Stations[S[--sptr]]);
            }

            //return null;
            return result;
        }

        /// <summary>
        ///  Get route from startUid station to destUid station.
        /// </summary>
        /// <param name="startUid">Start station Uid</param>
        /// <param name="destUid">Destination station Uid</param>
        public static Route GetRoute(string startUid, string destUid)
        {
            Route result;

            if (startUid == destUid)
            {
                result = new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "'startUid' and 'destUid' cannot be the same."
                };
                return result;
            }

            var startStation = Core.GeoHelper.Stations.FirstOrDefault(x => x.Uid == startUid);
            var destStation = Core.GeoHelper.Stations.FirstOrDefault(x => x.Uid == destUid);

            if (startStation == null || destStation == null)
            {
                result = new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "'startUid'/'destUid' is incorrect."
                };
                return result;
            }

            var stations = CalcRoute(startUid, destUid);
            double duration = 0, distance = 0;

            for (int i = 1; i < stations.Count; i++)
            {
                var el = Core.GeoHelper.TravelDurations.FirstOrDefault(x => 
                    x.Station1Uid == stations[i - 1].Uid && x.Station2Uid == stations[i].Uid ||
                    x.Station2Uid == stations[i - 1].Uid && x.Station1Uid == stations[i].Uid);
                
                if (el != null)
                {
                    duration += el.Duration;
                    distance += el.Distance;
                }
            }

            if (stations.First()!= startStation || stations.Last()!=destStation)
            {
                result = new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "Route does not exist."
                };
                return result;
            }

            result = new Route()
            {
                Status = RouteStatus.Ok,
                Message = "OK",
                Start = startStation,
                Destination = destStation,
                Distance = distance,
                Duration = duration,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(duration),
                Stations = stations
            };
            return result;
        }
    }
}
