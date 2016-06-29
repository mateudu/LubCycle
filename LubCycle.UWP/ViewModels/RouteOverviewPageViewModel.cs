using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using LubCycle.UWP.Helpers;
using LubCycle.UWP.Models;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace LubCycle.UWP.ViewModels
{
    class RouteOverviewPageViewModel : ViewModelBase
    {
        private LubCycleHelper _lubCycleHelper;
        public RouteOverviewPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
            _lubCycleHelper = new LubCycleHelper();
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
            Route = CacheData.CurrentRoute;
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

        private Route _route;

        public Route Route
        {
            get { return _route; }
            set { Set(ref _route, value); }
        }
    }
}
