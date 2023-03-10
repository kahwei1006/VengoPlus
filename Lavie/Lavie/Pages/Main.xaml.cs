
using Lavie.Models;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.FirebasePushNotification;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;


namespace Lavie.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class Main : ContentPage
    {
        public Main()
        {
            try
            {
                InitializeComponent();

                if (IsConnectionAvailable())
                {
                    webview.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
                    webview.RegisterAction(new Action<string>(ParseData));
                }
                else
                {
                    App.Current.MainPage = new ConnectionFail();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public Main(string url) : base()
        {
            try
            {
                InitializeComponent();
                if (IsConnectionAvailable())
                {
                    webview.Uri = url;
                    webview.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
                    webview.RegisterAction(new Action<string>(ParseData));
                }
                else
                {
                    App.Current.MainPage = new ConnectionFail();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
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

            App.Current.MainPage = new NotificationPage(mesg);
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

            App.Current.MainPage = new DataRoute(mesg);
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
                    case "alert":
                        DisplayAlert("Alert", mesg.ParamVal, "OK");
                        break;
                    case "photo":
                        PerformPhotoAction(mesg);
                        break;
                    case "qrcode":
                        App.Current.MainPage = new Page1();
                        break;
                    case "gps":
                        PerformGPSAction(mesg);
                        break;
                    case "database":
                        PerformDbAction(mesg);
                        break;
                    case "uuid":
                        PerformUUIDAction(mesg);
                        break;
                    case "page":
                        PerformPageAction(mesg);
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

        protected void PerformPhotoAction(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        switch (mesg.Action.Trim().ToLower())
                        {
                            case "get":
                            case "set":
                            case "create":
                                MediaFile photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                                {
                                    CompressionQuality = 100,
                                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                                    Directory = "Photo",
                                    Name = "LaviePhoto.jpg"
                                });
                                if (photo != null)
                                {
                                    var path = photo.Path;
                                    var stream = photo.GetStream();
                                    var mem = new MemoryStream();
                                    var buffer = new byte[16384];
                                    var byteRead = 0;
                                    byteRead = stream.Read(buffer, 0, buffer.Length);
                                    while (byteRead > 0)
                                    {
                                        mem.Write(buffer, 0, byteRead);
                                        byteRead = stream.Read(buffer, 0, buffer.Length);
                                    }
                                    photo.Dispose();
                                    File.Delete(path);
                                    mesg.ParamType = "Base64";
                                    mesg.ParamVal = Convert.ToBase64String(mem.ToArray());
                                    mesg.ErrorMesg = "OK";
                                    SendCallBack(mesg);
                                }
                                else
                                {
                                    SendExceptionMessage(mesg, new Exception("Failed To Take Photo."));
                                }
                                break;
                            case "delete":
                                SendExceptionMessage(mesg, new Exception("Invalid Action"));
                                break;
                            default:
                                SendExceptionMessage(mesg, new Exception("Unknown Action"));
                                break;
                        }
                    }
                    else
                    {
                        SendExceptionMessage(mesg, new Exception("Camera Not Available."));
                    }
                });
            }
            catch (Exception e)
            {
                SendExceptionMessage(mesg, e);
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
                        string tid = "";
                        var t = await App.Database.GetLocalStorageAsync("UUID");
                        var token= await App.Database.GetLocalStorageAsync("TID");
                        if (t != null &&tid!=null)
                        {
                            uuid = t.Value;
                        }
                        else
                        {
                            uuid = Guid.NewGuid().ToString();
                            await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "UUID", Value = uuid });

                            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
                            {
                                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");

                                tid = p.Token;



                            };

                            await App.Database.SaveLocalStorageAsync(new LocalStorage { Key = "TID", Value = tid });

                        }

                        MessagingCenter.Subscribe<QRCodePage, string>(this, "QRCode", (sender, arg) =>
                        {
                            MessagingCenter.Unsubscribe<QRCodePage, string>(this, "QRCode");
                            mesg.ParamType = "String";
                            mesg.ParamVal = uuid;
                            mesg.ErrorMesg = (arg != "") ? "OK" : "Failed To Scan QR Code";
                            string urlSend = "https://erp.letach.com.sg/portal/vengoplus/scan.asp?UID=" + mesg.ParamVal + "&MID=" + arg;  ;
                           mesg.FullURL= urlSend;
                            
                            SendCallBack(mesg);
                        });
                        await Navigation.PushModalAsync(new QRCodePage(mesg));
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

        protected void PerformGPSAction(WebViewMessage mesg)
        {
            try
            {
                Task.Run(async () =>
                {
                    if (mesg.Action.Trim().ToLower() == "get")
                    {
                        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                        var location = await Geolocation.GetLocationAsync(request);
                        if (location != null)
                        {
                            mesg.ParamType = "Location";
                            mesg.ParamVal = JsonConvert.SerializeObject(location);
                            mesg.ErrorMesg = "OK";
                           
                            SendCallBack(mesg);
                        }
                        else
                        {
                            SendExceptionMessage(mesg, new Exception("Location Is Null."));
                        }
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

        protected void PerformDbAction(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (mesg.ParamType.Trim().ToLower() == "localstorage")
                    {
                        var model = JsonConvert.DeserializeObject<LocalStorage>(mesg.ParamVal);
                        switch (mesg.Action.Trim().ToLower())
                        {
                            case "get":
                                if (model.Key.Trim() == "*")
                                {
                                    var list = await App.Database.GetLocalStorageAsync();
                                    if (list != null)
                                    {
                                        mesg.ParamType = "LocalStorageList";
                                        mesg.ParamVal = JsonConvert.SerializeObject(list);
                                        mesg.ErrorMesg = "OK";
                                        SendCallBack(mesg);
                                    }
                                    else
                                    {
                                        SendExceptionMessage(mesg, new Exception("No record found."));
                                    }
                                }
                                else
                                {
                                    var t = await App.Database.GetLocalStorageAsync(model.Key);
                                    if (t != null)
                                    {
                                        mesg.ParamType = "LocalStorage";
                                        mesg.ParamVal = JsonConvert.SerializeObject(t);
                                        mesg.ErrorMesg = "OK";
                                        SendCallBack(mesg);
                                    }
                                    else
                                    {
                                        SendExceptionMessage(mesg, new Exception("No record found."));
                                    }
                                }
                                break;
                            case "set":
                            case "create":
                                if (!IsReservedKeyName(model.Key))
                                {
                                    await App.Database.SaveLocalStorageAsync(model);
                                    mesg.ParamType = "LocalStorage";
                                    mesg.ParamVal = JsonConvert.SerializeObject(model);
                                    mesg.ErrorMesg = "OK";
                                    SendCallBack(mesg);
                                }
                                else
                                {
                                    SendExceptionMessage(mesg, new Exception("Invalid Key Name"));
                                }
                                break;
                            case "delete":
                                await App.Database.DeleteLocalStorageAsync(model);
                                mesg.ErrorMesg = "OK";
                                SendCallBack(mesg);
                                break;
                            default:
                                SendExceptionMessage(mesg, new Exception("Unknown Action"));
                                break;
                        }
                    }
                    else
                    {
                        SendExceptionMessage(mesg, new Exception("Unknown ParamType"));
                    }
                });
            }
            catch (Exception e)
            {
                SendExceptionMessage(mesg, e);
            }
        }

        protected void PerformUUIDAction(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    string uuid = "";

                    if (mesg.Action.Trim().ToLower() == "get")
                    {
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
                        mesg.ParamType = "String";
                        mesg.ParamVal = uuid;
                        mesg.ErrorMesg = "OK";
                        SendCallBack(mesg);
                    }
                    else
                    {
                        SendExceptionMessage(mesg, new Exception("Invalid Action"));
                    }
                });
            }
            catch (Exception e)
            {
                SendExceptionMessage(mesg, e);
            }
        }

        protected void PerformPageAction(WebViewMessage mesg)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (mesg.Action.Trim().ToLower() == "create")
                    {
                        if (mesg.ParamType.Trim().ToLower() == "url")
                        {
                            await Navigation.PushAsync(new MainPage(mesg.ParamVal));
                        }
                        else
                        {
                            SendExceptionMessage(mesg, new Exception("Invalid ParamType."));
                        }
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
