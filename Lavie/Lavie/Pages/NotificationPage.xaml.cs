using Lavie.CustomViews;
using Lavie.Models;
using Plugin.Connectivity;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Lavie.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPage : ContentPage
    {
        public NotificationPage(WebViewMessage msg)
        {
            if (IsConnectionAvailable())
            {
                string newURL = "https://erp.letach.com.sg/portal/vengoplus/notifications.asp?UID=" + msg.ParamVal + "&tid=" + msg.TID;


                InitializeComponent();
                webview.Uri = newURL;
            }
            else
            {

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    App.Current.MainPage = new ConnectionFail();
                });
            }

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
            // await Navigation.PushAsync(new Page1());
            App.Current.MainPage = new Page1();
        }

        private async void btnNotification_Clicked(object sender, EventArgs e)
        {
            WebViewMessage mesg = new WebViewMessage();
            string tid = "";
            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            var token = await App.Database.GetLocalStorageAsync("TID");
            if (t != null && token != null)
            {
                uuid = t.Value;
                tid = token.Value;
            }
            else
            {
                uuid = Guid.NewGuid().ToString();
                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });

                tid = CrossFirebasePushNotification.Current.Token;

                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "TID", Value = tid });

            }

            mesg.ParamVal = uuid;
            mesg.TID = tid;
            //  OnDisappearing();
            App.Current.MainPage = new NotificationPage(mesg);
            //await Navigation.PushAsync(new NotificationPage());
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            WebViewMessage mesg = new WebViewMessage();
            string tid = "";
            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            var token = await App.Database.GetLocalStorageAsync("TID");
            if (t != null && token != null)
            {
                uuid = t.Value;
                tid = token.Value;
            }
            else
            {
                uuid = Guid.NewGuid().ToString();
                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });

                tid = CrossFirebasePushNotification.Current.Token;

                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "TID", Value = tid });

            }

            mesg.Id = DateTime.Now.ToString();
            mesg.Action = "Get";
            mesg.Obj = "QRCode";
            mesg.ParamType = "";
            mesg.ParamVal = uuid;
            mesg.TID = tid;
            mesg.CallBack = "containerCallBack";
            mesg.ErrorMesg = "";

            //await Navigation.PushAsync(new DataRoute(mesg));

            App.Current.MainPage = new DataRoute(mesg);
            // await Navigation.PushAsync(new QRCodePage());
        }
        private async void btnSetting_Clicked(object sender, EventArgs e)
        {
            WebViewMessage mesg = new WebViewMessage();
            string tid = "";
            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            var token = await App.Database.GetLocalStorageAsync("TID");
            if (t != null && token != null)
            {
                uuid = t.Value;
                tid = token.Value;
            }
            else
            {
                uuid = Guid.NewGuid().ToString();
                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });

                tid = CrossFirebasePushNotification.Current.Token;

                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "TID", Value = tid });

            }
            mesg.TID = tid;
            mesg.ParamVal = uuid;


            App.Current.MainPage = new SettingPage(mesg);
            // await Navigation.PushAsync(new SettingPage());
        }
        private async void btnMore_Clicked(object sender, EventArgs e)
        {

            WebViewMessage mesg = new WebViewMessage();
            string tid = "";
            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            var token = await App.Database.GetLocalStorageAsync("TID");
            if (t != null && token != null)
            {
                uuid = t.Value;
                tid = token.Value;
            }
            else
            {
                uuid = Guid.NewGuid().ToString();
                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });

                tid = CrossFirebasePushNotification.Current.Token;

                await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "TID", Value = tid });

            }

            mesg.ParamVal = uuid;
            mesg.TID = tid;

            App.Current.MainPage = new More(mesg);
            // await Navigation.PushAsync(new SettingPage());
        }
    }
}