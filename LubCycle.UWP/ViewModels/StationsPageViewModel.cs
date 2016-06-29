using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using LubCycle.UWP.Views;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Template10.Utils;

namespace LubCycle.UWP.ViewModels
{
    class StationsPageViewModel : ViewModelBase
    {
        private readonly LubCycleHelper _lubcycleHelper;
        public MapControl MapControl;
        public MenuFlyout PinpointFlyout;
        private StationsListViewItem _rightPressedItem = null;
        public ObservableCollection<StationsListViewItem> MapItemsSource;
        public readonly int[] BikeCountItems = new[] { 0, 1, 2, 3, 4, 5 };

        public StationsPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }

            _lubcycleHelper = new LubCycleHelper();
            MapItemsSource = new ObservableCollection<StationsListViewItem>();
        }
        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
            if (CacheData.Position != null)
            {
                await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, StaticData.DefaultMapZoom);
            }
            LoadPageAsync();
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

        private async Task LoadPageAsync()
        {
            try
            {
                SetButtonsEnabled(false);
                await ListHelper.LoadStationsAndPositionAsync(false);
                ListHelper.ReloadList(ref MapItemsSource,
                    item => int.Parse(item.Station.Bikes) >= SelectedBikes
                    );
                if (CacheData.Position != null)
                {
                    await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, StaticData.DefaultMapZoom);
                }
            }
            catch (Exception)
            {
                var dlg = new MessageDialog("Błąd połączenia.");
                await dlg.ShowAsync();
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Value);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0, new SuppressNavigationTransitionInfo());

        //public void GotoPrivacy() =>
        //    NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);


        private string _addressSearch;

        public string AddressSearch
        {
            get { return _addressSearch; }
            set { Set(ref _addressSearch, value); }
        }
        

        private StationsListViewItem _startStation = null;
        private StationsListViewItem _destStation = null;

        public StationsListViewItem StartStation
        {
            get { return _startStation; }
            set { Set(ref _startStation, value); }
        }
        public StationsListViewItem DestStation
        {
            get { return _destStation; }
            set { Set(ref _destStation, value); }
        }

        private int SelectedBikes { get; set; } = 0;

        public async void BikeCountPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? number = (sender as ComboBox).SelectedItem as int?;
            if (number.HasValue)
            {
                try
                {
                    SetButtonsEnabled(false);
                    SelectedBikes = number.Value;
                    ListHelper.ReloadList(
                        ref MapItemsSource,
                        item => int.Parse(item.Station.Bikes) >= SelectedBikes
                        );
                }
                catch
                {
                    var dlg = new MessageDialog("Błąd połączenia.");
                    await dlg.ShowAsync();
                }
                finally
                {
                    SetButtonsEnabled(true);
                }
            }
        }

        public async void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(AddressSearch))
            {
                try
                {
                    SetButtonsEnabled(false);
                    var result = await _lubcycleHelper.GetLocationAsync(AddressSearch);
                    if (result.Status != LocationResponseStatus.Ok)
                    {
                        var dlg = new MessageDialog(@"Błąd zapytania.");
                        await dlg.ShowAsync();
                    }
                    else
                    {
                        await MapControl.TrySetViewAsync(
                            new Geopoint(
                                new BasicGeoposition()
                                {
                                    Latitude = result.Lat.Value,
                                    Longitude = result.Lng.Value
                                })
                            );
                    }

                }
                catch (Exception)
                {
                    
                }
                finally
                {
                    SetButtonsEnabled(true);
                }
            }
        }

        public void AddressSearchASB_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var matchingStations = GetMatchingStations(sender.Text);
                sender.ItemsSource = matchingStations;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {

            }
        }

        public async void AddressSearchASB_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is StationsListViewItem)
            {
                await MapControl.TrySetViewAsync((args.SelectedItem as StationsListViewItem).Geopoint, StaticData.DefaultMapZoom);
            }
        }

        private List<StationsListViewItem> GetMatchingStations(string query)
        {
            return CacheData.StationListViewItems?
                    .Where(x =>
                        x.Station.Name.ToLower().Contains(query.ToLower())
                        || x.Station.Number.ToString().Contains(query)
                        ).ToList();
        }
        
        public async void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SetButtonsEnabled(false);
                await ListHelper.LoadStationsAndPositionAsync(true);
                ListHelper.ReloadList(ref MapItemsSource,
                    item => int.Parse(item.Station.Bikes) >= SelectedBikes
                    );
            }
            catch
            {
                var dlg = new MessageDialog(@"Błąd połączenia z serwisem.");
                await dlg.ShowAsync();
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }
        public async void PositionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (CacheData.Position != null)
            {
                SetButtonsEnabled(false);
                await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, StaticData.DefaultMapZoom);
                SetButtonsEnabled(true);
            }
        }

        public async void StartNavigationButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (StartStation != null && DestStation != null)
            {
                try
                {
                    Views.Busy.SetBusy(true, @"Ładowanie...");
                    var res = await _lubcycleHelper.GetRouteAsync(StartStation.Station.Number, DestStation.Station.Number);
                    CacheData.CurrentStartStation = StartStation;
                    CacheData.CurrentDestStation = DestStation;
                    CacheData.CurrentRoute = res;
                }
                catch (Exception)
                {

                }
                finally
                {
                    Views.Busy.SetBusy(false);
                    NavigationService.Navigate(typeof(RouteOverviewPage), null, new SuppressNavigationTransitionInfo());
                }
            }
        }
        
        private void SetButtonsEnabled(bool val)
        {
            SearchButtonEnabled = val;
            FilterButtonEnabled = val;
            RefreshButtonEnabled = val;
            PositionButtonEnabled = val;
        }

        #region Pinpoint events
        // This function MUST be invoked before NavigateToStationClick/NavigateFromStationClick,
        // because it sets _rightPressedItem !!!
        public void Pinpoint_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            PinpointFlyout.ShowAt(sender as FrameworkElement);
            if ((sender as StackPanel)?.Tag is StationsListViewItem)
            {
                _rightPressedItem = (sender as StackPanel).Tag as StationsListViewItem;
            }
        }

        public async void NavigateToStationClick()
        {
            DestStation = _rightPressedItem;
            if (StartStation == null)
            {
                var toast = NotificationHelper.GetTextOnlyNotification(
                    "Wybrano stację docelową",
                    "Wybierz stację startową."
                    );
                ToastNotificationManager.CreateToastNotifier().Show(toast);
                if (CacheData.Position != null)
                {
                    await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, StaticData.DefaultMapZoom);
                }
            }
            else
            {
                StartNavigationButtonEnabled = true;
            }
        }

        public async void NavigateFromStationClick()
        {
            StartStation = _rightPressedItem;
            if (DestStation == null)
            {
                var toast = NotificationHelper.GetTextOnlyNotification(
                "Wybrano stację startową",
                "Wybierz stację docelową."
                );
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            else
            {
                StartNavigationButtonEnabled = true;
            }
        }
        // END = Pinpoint click functions.
        #endregion

        #region ButtonEnabled Properties
        private bool _searchButtonEnabled = true;

        public bool SearchButtonEnabled
        {
            get { return _searchButtonEnabled; }
            set { Set(ref _searchButtonEnabled, value); }
        }

        private bool _startNavigationButtonEnabled = false;

        public bool StartNavigationButtonEnabled
        {
            get { return _startNavigationButtonEnabled; }
            set { Set(ref _startNavigationButtonEnabled, value); }
        }

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

        private bool _positionButtonEnabled = false;

        public bool PositionButtonEnabled
        {
            get { return _positionButtonEnabled; }
            set { Set(ref _positionButtonEnabled, value); }
        }
        #endregion
    }
}
