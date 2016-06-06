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
        private const double EQuatorialEarthRadius = 6378.1370D;
        private const double D2R = (Math.PI / 180D);
        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double dlong = (lng2 - lng1) * D2R;
            double dlat = (lat2 - lat1) * D2R;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * D2R) * Math.Cos(lat2 * D2R) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = EQuatorialEarthRadius * c;
            return d;
        }

        public static double CalcDistance(Models.NextBike.Place start, Models.NextBike.Place destination)
        {
            return CalcDistance(start.Lat, start.Lng, destination.Lat, destination.Lng);
        }


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

        public static List<Place> CalcRoute(string startUid, string destUid)
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
    }
}
