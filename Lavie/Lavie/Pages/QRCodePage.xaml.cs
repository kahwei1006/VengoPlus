using Lavie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace Lavie.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRCodePage : ContentPage
    {
        protected string qrcode = "";
        public QRCodePage()
        {
            try
            {
                InitializeComponent();
              //  scanView.Options.DelayBetweenAnalyzingFrames = 5;
              //  scanView.Options.DelayBetweenContinuousScans = 5;
                scanView.OnScanResult += scanView_OnScanResult;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
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

            string uuid = "";
            var t = await App.Database.GetLocalStorageAsync("UUID");
            if (t != null)
            {
                uuid = t.Value;
            }

            mesg.ParamVal = uuid;
            mesg.pageRoute = "NotificationPage";
          //  OnDisappearing();
            App.Current.MainPage = new NotificationPage(mesg);

            //await Navigation.PushAsync(new NotificationPage());
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
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<QRCodePage, string>(this, "QRCode", qrcode);
       }

        public void scanView_OnScanResult(Result result)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    scanView.OnScanResult -= scanView_OnScanResult;
                    qrcode = result.Text;
                    await Navigation.PopModalAsync();
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}