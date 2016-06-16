using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace LubCycle.UWP.Helpers
{
    class GeoHelper
    {
        public static async Task<Geoposition> GetPositionAsync()
        {
            Geoposition _pos = null;
            try
            {

                // Request permission to access location
                var accessStatus = await Geolocator.RequestAccessAsync();

                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    // Get cancellation token
                    var _cts = new CancellationTokenSource();
                    CancellationToken token = _cts.Token;

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    //Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };
                    Geolocator geolocator = new Geolocator();

                    // Carry out the operation
                    _pos = await geolocator.GetGeopositionAsync().AsTask(token);
                    //stationsMap.Center = _pos.Coordinate.Point;
                    //stationsMap.ZoomLevel = 15.0;
                }
                else
                {
                    //throw new Exception("Problem with location permissions or access");
                    var dlg = new MessageDialog("Dostęp do lokalizacji jest wyłączony!");
                    await dlg.ShowAsync();
                }

            }
            catch (TaskCanceledException tce)
            {
                //throw new Exception("Task cancelled" + tce.Message);
            }
            finally
            {
                //_cts = null;
            }
            return _pos;
        }
    }
}
