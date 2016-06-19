using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace LubCycle.UWP.Helpers
{
    public static class LocationHelper
    {
        private static string routeFinderUnavailableMessage = "Unable to access map route finder service.";

        /// <summary>
        /// Gets the Geolocator singleton used by the LocationHelper.
        /// </summary>
        public static Geolocator Geolocator { get; } = new Geolocator();

        /// <summary>
        /// Gets or sets the CancellationTokenSource used to enable Geolocator.GetGeopositionAsync cancellation.
        /// </summary>
        private static CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// Initializes the LocationHelper.
        /// </summary>
        static LocationHelper()
        {
            // TODO Replace the placeholder string below with your own Bing Maps key from https://www.bingmapsportal.com
            MapService.ServiceToken = StaticData.MapServiceToken;
        }

        /// <summary>
        /// Gets the current location if the geolocator is available.
        /// </summary>
        /// <returns>The current location.</returns>
        public static async Task<Geoposition> GetCurrentLocationAsync()
        {
            try
            {
                // Request permission to access the user's location.
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:

                        LocationHelper.CancellationTokenSource = new CancellationTokenSource();
                        var token = LocationHelper.CancellationTokenSource.Token;

                        Geoposition position = await Geolocator.GetGeopositionAsync().AsTask(token);
                        //return new LocationData { Position = position.Coordinate.Point.Position };
                        return position;

                    case GeolocationAccessStatus.Denied:
                    case GeolocationAccessStatus.Unspecified:
                    default:
                        return null;
                }
            }
            catch (TaskCanceledException)
            {
                // Do nothing.
            }
            finally
            {
                LocationHelper.CancellationTokenSource = null;
            }
            return null;
        }

        /// <summary>
        /// Cancels any waiting GetGeopositionAsync call.
        /// </summary>
        public static void CancelGetCurrentLocation()
        {
            if (LocationHelper.CancellationTokenSource != null)
            {
                LocationHelper.CancellationTokenSource.Cancel();
                LocationHelper.CancellationTokenSource = null;
            }
        }
    }
}