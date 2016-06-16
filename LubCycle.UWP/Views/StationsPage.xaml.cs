using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;
        public StationsPage()
        {
            this.InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            stationsMap.MapServiceToken = @"ApcN9o_xREqWLKuVV0MKfqmsd2hapuXA-Jo-mhuhJunA6XLF-Bgi-goFFp4PgEZu";

        }

        private async void StationsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
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
                    var _pos = await geolocator.GetGeopositionAsync().AsTask(token);
                    stationsMap.Center = _pos.Coordinate.Point;
                    stationsMap.ZoomLevel = 15.0;
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
        }
    }
}
