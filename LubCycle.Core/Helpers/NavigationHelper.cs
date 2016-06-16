using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubCycle.Core.Models.Navigation;
using LubCycle.Core.Models.NextBike;

namespace LubCycle.Core.Helpers
{
    public class NavigationHelper
    {
        public NavigationHelper(NavigationHelperSettings settings)
        {
            this.RouteStatistics = settings.RouteStatistics;
            this.Stations = settings.Stations;
            this.MaximalSingleDuration = settings.MaximalSingleDuration;
            this.MaximalSingleDistance = settings.MaximalSingleDistance;
        }

        private List<List<Models.Navigation.Edge>> _graph;
        private Dictionary<string, int> _edges;
        public List<Models.Navigation.RouteStatistic> RouteStatistics;
        public List<Place> Stations;
        private bool _isInitialized = false;
        public double MaximalSingleDuration;
        public double MaximalSingleDistance;

        // Initialize directed graph
        private void InitializeGraph()
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
                foreach (RouteStatistic t in RouteStatistics)
                {
                    if (t.Duration <= MaximalSingleDuration && t.Distance<= MaximalSingleDistance)
                    {
                        from = _edges[t.Station1Uid];
                        to = _edges[t.Station2Uid];
                        distance = t.Distance;
                        duration = t.Duration;
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
                }
                _isInitialized = true;
            }
        }

        // Reinitialize directed graph
        public void ReinitializeGraph()
        {
            _isInitialized = false;
            InitializeGraph();
        }

        private List<Place> CalcRoute(string startUid, string destUid)
        {
            InitializeGraph();
            int i, j;
            int v;
            v = _edges[startUid];

            double[] D = new double[Stations.Count];
            int[] P = new int[Stations.Count];
            bool[] QS = new bool[Stations.Count];
            int[] S = new int[Stations.Count];
            int sptr = 0;

            #region Dijkstra Algorithm

            for (i = 0; i < Stations.Count; i++)
            {
                D[i] = (double)Int32.MaxValue;
                P[i] = -1;
                QS[i] = false;
            }

            D[v] = 0;

            for (i = 0; i < Stations.Count; i++)
            {
                for (j = 0; QS[j]; j++) ;
                int u;
                for (u = j++; j < Stations.Count; j++)
                {
                    if (!QS[j] && (D[j] < D[u])) u = j;
                }

                QS[u] = true;
                
                for (int k = 0; k < _graph[u].Count; k++)
                {
                    var pw = _graph[u][k];
                    if (!QS[pw.To] && (D[pw.To] > D[u] + pw.Distance))
                    {
                        D[pw.To] = D[u] + pw.Distance;
                        P[pw.To] = u;
                    }
                }
            }

            for (j = _edges[destUid]; j > -1; j = P[j])
                S[sptr++] = j;
            #endregion
            var result = new List<Place>();
            while (sptr > 0)
            {
                result.Add(Stations[S[--sptr]]);
            }
            
            return result;
        }

        /// <summary>
        ///  Get route from startUid station to destUid station.
        /// </summary>
        /// <param name="start">Start station Uid</param>
        /// <param name="dest">Destination station Uid</param>
        /// <param name="numberType">'Number'/'Uid'</param>
        public Route GetRoute(string start, string dest, StationNumberType numberType)
        {
            if (start == dest)
            {
                return new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "'start' and 'dest' cannot be the same."
                };
            }

            var startStation = GetStation(start,numberType);
            var destStation = GetStation(dest, numberType);

            if (startStation == null || destStation == null)
            {
                return new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "'startUid'/'destUid' is incorrect."
                };
            }


            var stations = CalcRoute(startStation.Uid, destStation.Uid);
            double duration = 0, distance = 0;

            for (int i = 1; i < stations.Count; i++)
            {
                var el = RouteStatistics.FirstOrDefault(x =>
                    x.Station1Uid == stations[i - 1].Uid && x.Station2Uid == stations[i].Uid ||
                    x.Station2Uid == stations[i - 1].Uid && x.Station1Uid == stations[i].Uid);

                if (el != null)
                {
                    duration += el.Duration;
                    distance += el.Distance;
                }
            }

            if (stations.First() != startStation || stations.Last() != destStation)
            {
                return new Route()
                {
                    Status = RouteStatus.IncorrectArguments,
                    Message = "Route does not exist."
                };
            }

            return new Route()
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
        }

        private Place GetStation(string number, StationNumberType numberType)
        {
            switch (numberType)
            {
                case StationNumberType.StationNumber:
                    return Stations.FirstOrDefault(x => x.Number.ToString() == number);
                case StationNumberType.StationUid:
                    return Stations.FirstOrDefault(x => x.Uid == number);
                default:
                    return null;
            }
        }
    }
}