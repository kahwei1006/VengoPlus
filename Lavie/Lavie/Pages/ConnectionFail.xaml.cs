using Lavie.Models;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Lavie.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionFail : ContentPage
    {
        public ConnectionFail()
        {


            InitializeComponent();


        }
        public bool IsConnectionAvailable()
        {
            if (!CrossConnectivity.IsSupported)
                return false;

            bool isConnected = CrossConnectivity.Current.IsConnected;
            //return CrossConnectivity.Current.IsConnected;
            return isConnected;
        }
        private async void btn_Clicked(object sender, EventArgs e)
        {

            if (IsConnectionAvailable())
            {
                // download content from external db to device (SQLite db)
                App.Current.MainPage = new Page1();
            }
            else
            {
                DisplayAlert("No internet connection found.", "Please check your internet connection.", "OK");
            }
           
            //await Navigation.PushAsync(new Page1());
        }

    }
}
  