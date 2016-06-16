using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LubCycle.UWP.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StationsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;
        private LubCycleHelper _lubcycle = new LubCycleHelper();
        ObservableCollection<StationsListViewItem> StationListViewItems = new ObservableCollection<StationsListViewItem>();
        private List<Place> _stations = new List<Place>();
        private Geoposition _position = null;

        public StationsPage()
        {
            this.InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            stationsMap.MapServiceToken = @"ApcN9o_xREqWLKuVV0MKfqmsd2hapuXA-Jo-mhuhJunA6XLF-Bgi-goFFp4PgEZu";
        }

        private async void StationsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_position == null)
            {
                refreshButton.IsEnabled = false;
                await ReloadPage();
                refreshButton.IsEnabled = true;
            }
        }

        private async Task ReloadStationsAndPosition()
        {
            var pos = GeoHelper.GetPositionAsync();
            var stations = _lubcycle.GetStationsAsync();
            await Task.WhenAll(pos, stations);
            _stations = stations.Result;
            _position = pos.Result;

            foreach (var obj in _stations)
            {
                if (obj.Bikes == @"5+")
                    obj.Bikes = "5";
            }
        }

        private async Task ReloadPage()
        {
            try
            {
                await ReloadStationsAndPosition();
                ReloadList();
                stationsMap.Center = _position.Coordinate.Point;
                stationsMap.ZoomLevel = 15.0;
            }
            catch (Exception)
            {

            }
            finally
            {
                
            }
        }

        private void ReloadList()
        {
            StationListViewItems.Clear();

            foreach (var obj in _stations)
            {
                if (int.Parse(obj.Bikes) >= (int)bikeCountSlider.Value)
                {
                    StationListViewItems.Add(
                    new StationsListViewItem
                    {
                        Station = obj
                    });
                }
            }
        }

        private void BikeCountSlider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ReloadList();
        }

        private async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            refreshButton.IsEnabled = false;
            await ReloadPage();
            refreshButton.IsEnabled = true;
        }
    }
}
