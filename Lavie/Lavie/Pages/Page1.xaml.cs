using Lavie.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;
using Plugin.FirebasePushNotification;

namespace Lavie.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1(string url) : base()
        {

            try
            {
                InitializeComponent();
                if (IsConnectionAvailable())
                {
                    // download content from external db to device (SQLite db)
                    webview.Uri = url;
                    webview.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
                }
                else
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = new ConnectionFail();
                    });
                }
            

              
               // webview.RegisterAction(new Action<string>(ParseData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public Page1()
        {
        
        
        InitializeComponent();
            if (IsConnectionAvailable())
            {
          
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
            //await Navigation.PushAsync(new DataRoute(mesg));
        }

        
        private async void btn_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new Page1();

            //await Navigation.PushAsync(new Page1());
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
       
        protected void SendCallBack(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //    if (!string.IsNullOrEmpty(mesg.CallBack))
                    //    {
                    //        await webview.EvaluateJavaScriptAsync($"{mesg.CallBack}('{JsonConvert.SerializeObject(mesg)}');");
                    //    }
                    //});

                    if (mesg.FullURL != null)
                    {

                        await Navigation.PushAsync(new Main(mesg.FullURL));

                    }
                    else
                    {
                        SendExceptionMessage(mesg, new Exception("Invalid Action"));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                DisplayAlert("Container Exception", ex.Message, "OK");
            }
        }
        protected void SendExceptionMessage(WebViewMessage mesg, Exception e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (!string.IsNullOrEmpty(mesg.CallBack))
                    {
                        mesg.ErrorMesg = e.Message;
                        await webview.EvaluateJavaScriptAsync($"{mesg.CallBack}('{JsonConvert.SerializeObject(mesg)}');");
                    }
                    else
                    {
                        await DisplayAlert("Container Exception", e.Message, "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                DisplayAlert("Container Exception", ex.Message, "OK");
            }
        }
    }
}