using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LubCycle.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteOverviewPage : Page
    {
        public RouteOverviewPage()
        {
            this.InitializeComponent();
            routeMap.MapServiceToken = StaticData.MapServiceToken;
            ViewModel.MapControl = routeMap;
        }

        public void RemoveSLVI_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSLVI_OnClick(sender,e);
        }
        public void NavigateSLVI_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateSLVI_OnClick(sender,e);
        }
    }
}
