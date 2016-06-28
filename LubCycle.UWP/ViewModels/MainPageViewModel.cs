using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Utils;

namespace LubCycle.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly LubCycleHelper _lubcycleHelper;
        public MapControl MapControl;
        public MenuFlyout PinpointFlyout;
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
            _lubcycleHelper = new LubCycleHelper();
            MapItemsSource = new ObservableCollection<StationsListViewItem>();
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
            if (CacheData.Position != null)
            {
                await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, 15.0);
            }
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


        ////////////////////////////////////////////////
        private bool _reloadRequested = false;
        private StationsListViewItem _rightPressedItem = null;
        public ObservableCollection<StationsListViewItem> MapItemsSource;

        public void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadPageAsync();
        }

        private async Task LoadPageAsync()
        {
            await ListHelper.LoadStationsAndPositionAsync(_reloadRequested);
            ListHelper.ReloadList(ref MapItemsSource);
            if (CacheData.Position != null)
            {
                await MapControl.TrySetViewAsync(CacheData.Position.Coordinate.Point, 15.0);
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


        public async void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(AddressSearch))
            {
                SearchButtonEnabled = false;
                var result = await _lubcycleHelper.GetLocationAsync(AddressSearch);
                var dlg = new MessageDialog($"{result.Message}");
                await dlg.ShowAsync();
                SearchButtonEnabled = true;
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
                await MapControl.TrySetViewAsync((args.SelectedItem as StationsListViewItem).Geopoint, 15.0);
            }
        }

        private List<StationsListViewItem> GetMatchingStations(string query)
        {
            return CacheData.StationListViewItems?
                    .Where(x =>
                        x.Station.Name.ToLower().Contains(query.ToLower())
                        ).ToList();
        }


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
                    "Wybrano stacjê docelow¹",
                    "Wybierz stacjê startow¹."
                    );
                ToastNotificationManager.CreateToastNotifier().Show(toast);
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
                "Wybrano stacjê startow¹",
                "Wybierz stacjê docelow¹."
                );
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            else
            {
                StartNavigationButtonEnabled = true;
            }
        }
        // END = Pinpoint click functions.
    }
}

