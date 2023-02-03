﻿
using Lavie.Models;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Plugin.Connectivity;

namespace Lavie.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataRoute : ContentPage
    {
        public DataRoute(WebViewMessage msg)
        {


               InitializeComponent();
           
                string newURL = "https://erp.letach.com.sg/portal/vengoplus/home.asp";
                webview.Uri = newURL;
                //  var  = JsonConvert.DeserializeObject<WebViewMessage>(msg);
                var msgJson = JsonConvert.SerializeObject(msg);
                ParseData(msgJson);
           
        
            
            //invokeContainerAction(JSON.stringify(mesg));
            // webview.RegisterAction(new Action<string>(ParseData));

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
            App.Current.MainPage = new Page1();
            //await Navigation.PushAsync(new Page1());
        }

        private async void btnNotification_Clicked(object sender, EventArgs e)
        {

            WebViewMessage mesg = new WebViewMessage();

            string uuid = "";
                    var t = await App.Database.GetLocalStorageAsync("UUID");
                    if (t != null)
                    {
                        uuid = t.Value;
                    }
                
                    mesg.ParamVal= uuid;
            mesg.pageRoute = "NotificationPage";
            SendCallBack(mesg);
            
            App.Current.MainPage = new NotificationPage(mesg);
            //await Navigation.PushAsync(new NotificationPage());
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            WebViewMessage mesg = new WebViewMessage();
            mesg.Id = DateTime.Now.ToString();
            mesg.Action = "Get";
            mesg.Obj = "QRCode";
            mesg.ParamType = "";
            mesg.ParamVal = "";
            mesg.CallBack = "containerCallBack";
            mesg.ErrorMesg = "";

            //await Navigation.PushAsync(new DataRoute(mesg));

            App.Current.MainPage = new DataRoute(mesg);
            //  await Navigation.PushAsync(new DataRoute(mesg));
        }
        private async void btnSetting_Clicked(object sender, EventArgs e)
        {

            WebViewMessage mesg = new WebViewMessage();

            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            if (t != null)
            {
                uuid = t.Value;
            }

            mesg.ParamVal = uuid;

         
            App.Current.MainPage = new SettingPage(mesg);
            // await Navigation.PushAsync(new SettingPage());
        }
        private async void btnMore_Clicked(object sender, EventArgs e)
        {

            WebViewMessage mesg = new WebViewMessage();

            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            if (t != null)
            {
                uuid = t.Value;
            }

            mesg.ParamVal = uuid;


            App.Current.MainPage = new More(mesg);
            // await Navigation.PushAsync(new SettingPage());
        }
        protected void ParseData(string data)
        {
            try
            {
                var mesg = JsonConvert.DeserializeObject<WebViewMessage>(data);
                switch (mesg.Obj.Trim().ToLower())
                {

                    case "qrcode":
                        PerformQRCodeAction(mesg);
                        break;
                    default:
                        SendExceptionMessage(mesg, new Exception("Unknown Object"));
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                DisplayAlert("Container Exception", ex.Message, "OK");
            }
        }
       
        protected void PerformQRCodeAction(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (mesg.Action.Trim().ToLower() == "get")
                    {
                        string uuid = "";
                        var t = await App.Database.GetLocalStorageAsync("UUID");
                        if (t != null)
                        {
                            uuid = t.Value;
                        }
                        else
                        {
                            uuid = Guid.NewGuid().ToString();
                            await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });
                        }

                        MessagingCenter.Subscribe<QRCodePage, string>(this, "QRCode", (sender, arg) =>
                        {
                            MessagingCenter.Unsubscribe<QRCodePage, string>(this, "QRCode");
                            mesg.ParamType = "String";
                            mesg.ParamVal = uuid;
                            mesg.MID = arg;
                            mesg.ErrorMesg = (arg != "") ? "OK" : "Failed To Scan QR Code";
                            string urlSend = "https://erp.letach.com.sg/portal/vengoplus/scan.asp?UID=" + mesg.ParamVal + "&MID=" + mesg.MID;
                           
                            // string url = HttpUtility.UrlEncode(urlSend);

                            mesg.FullURL = urlSend;

                           // if (mesg.MID != "")
                           // {
                               SendCallBack(mesg);
                         //   }

                            
                        });
                        //App.Current.MainPage = new QRCodePage();
                       
                            await Navigation.PushModalAsync(new QRCodePage());
                       
                     
                            
                    }
                    else
                    {
                        SendExceptionMessage(mesg, new Exception("Invalid Action."));
                    }
                });
            }
            catch (Exception e)
            {
                SendExceptionMessage(mesg, e);
            }
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
                   
                    if(mesg.MID != "")
                    {
                        App.Current.MainPage = new Main(mesg.FullURL);
                    }

                
                    else
                    {
                        // App.Current.MainPage = new Main((mesg.FullURL));
                        // App.Current.MainPage = new Main((mesg.FullURL));
                 
                       
                            mesg.Obj = "";
                            mesg.ParamType = "";
                            mesg.ParamVal = "";
                            mesg.FullURL = "";
                            mesg.Action = "";
                            mesg.ErrorMesg = "";
                       

                     


                       // this.Navigation.PopAsync();
                       // this.Navigation.PopToRootAsync();

                        // App.Current.MainPage = new Page1();
                        // SendExceptionMessage(mesg, new Exception("Invalid Action"));
                        // App.Current.MainPage = new Main()
                        //  App.Current.MainPage = new Main((mesg.FullURL));
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
                       // await webview.EvaluateJavaScriptAsync($"{mesg.CallBack}('{JsonConvert.SerializeObject(mesg)}');");
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

        protected bool IsReservedKeyName(string key)
        {
            switch (key.Trim().ToLower())
            {
                case "uuid":
                case "*":
                    return true;
                default:
                    return false;
            }
        }
    }
}
