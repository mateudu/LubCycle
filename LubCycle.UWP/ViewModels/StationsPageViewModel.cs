using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;

namespace LubCycle.UWP.ViewModels
{
    class StationsPageViewModel : ViewModelBase
    {
        public StationsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }

            MapItemsSource = new ObservableCollection<StationsListViewItem>();
            _lubcycle = new LubCycleHelper();
        }
        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
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

        public async Task OnLoaded(object sender, RoutedEventArgs e)
        {
            await LoadPageAsync();
        }

        ///////////////////////////////////////////////////////
        public readonly int[] BikeCountItems = new[] { 0, 1, 2, 3, 4, 5 };
        private readonly LubCycleHelper _lubcycle;
        public ObservableCollection<StationsListViewItem> MapItemsSource;

        public MapControl MapControl;


        private bool _refreshButtonEnabled = false;

        public bool RefreshButtonEnabled
        {
            get { return _refreshButtonEnabled; }
            set { Set(ref _refreshButtonEnabled, value); }
        }

        private bool _filterButtonEnabled = false;

        public bool FilterButtonEnabled
        {
            get { return _filterButtonEnabled; }
            set { Set(ref _filterButtonEnabled, value); }
        }

        private int SelectedBikes { get; set; } = 0;

        private bool _reloadRequested = false;

        public void BikeCountPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? number = (sender as ComboBox).SelectedItem as int?;
            if (number.HasValue)
            {
                SelectedBikes = number.Value;
                ListHelper.ReloadList( 
                    ref MapItemsSource, 
                    item => int.Parse(item.Station.Bikes)>=SelectedBikes
                    );
            }
        }
        public async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            _reloadRequested = true;
            await LoadPageAsync();
            _reloadRequested = false;
        }
        public async Task StationsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // App may crash without this Try/Catch during Reloading StationsListViewItems.
            try
            {
                var obj = (sender as ListView).SelectedItem as StationsListViewItem;
                await MapControl.TrySetViewAsync(obj.Geopoint, 15.0);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task LoadPageAsync()
        {
            try
            {
                RefreshButtonEnabled = false;
                FilterButtonEnabled = false;
                await ListHelper.LoadStationsAndPositionAsync(_reloadRequested);
                ListHelper.ReloadList(ref MapItemsSource);
                await MapControl.TrySetViewAsync(CacheData.Position != null ? CacheData.Position.Coordinate.Point : StaticData.DefaultMapCenter, 15.0);
            }
            catch (Exception exc)
            {
                var dlg = new MessageDialog(exc.Message);
                await dlg.ShowAsync();
            }
            finally
            {
                RefreshButtonEnabled = true;
                FilterButtonEnabled = true;
            }
        }
    }
}
