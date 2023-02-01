﻿using Lavie.CustomViews;
using Lavie.Models;
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
            //  var newUrl = msg.ParamVal;
            //  LavieWebView.BindingContext = newUrl;

            //   webview.SetBinding(Label.ScaleProperty, new Binding("Value", source: webview));
            // if (mesg.MID != "")
            //  {
            //     App.Current.MainPage = new Main((mesg.FullURL));
            //  }
            string newURL = "https://erp.letach.com.sg/portal/vengoplus/notifications.asp?UID=" +msg.ParamVal;

            // webview.Uri = newURL;
            // await Navigation.PushAsync(new Main(newURL));
            //  App.Current.MainPage = new Main(newURL);
            InitializeComponent();
            webview.Uri = newURL;
              

        }
        private async void btn_Clicked(object sender, EventArgs e)
        {
            // await Navigation.PushAsync(new Page1());
            App.Current.MainPage = new Page1();
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

            mesg.ParamVal = uuid;
            mesg.pageRoute = "NotificationPage";

            App.Current.MainPage = new NotificationPage(mesg);
            //await Navigation.PushAsync(new NotificationPage());
        }

        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            WebViewMessage mesg= new WebViewMessage();
            mesg.Id = DateTime.Now.ToString();
            mesg.Action = "Get";
            mesg.Obj = "QRCode";
            mesg.ParamType = "";
            mesg.ParamVal= "";
            mesg.CallBack = "containerCallBack";
            mesg.ErrorMesg = "";

            App.Current.MainPage = new DataRoute(mesg);
           // await Navigation.PushAsync(new QRCodePage());
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
    }
}