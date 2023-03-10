
using Lavie.Models;
using Newtonsoft.Json;
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
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            try
            {
                InitializeComponent();
                webview.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
                webview.RegisterAction(new Action<string>(ParseData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public MainPage(string url) : base()
        {
            try
            {
                InitializeComponent();
                webview.Uri = url;
                webview.On<Android>().SetMixedContentMode(MixedContentHandling.AlwaysAllow);
                webview.RegisterAction(new Action<string>(ParseData));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        protected void ParseData(string data)
        {
            try
            {
                var mesg = JsonConvert.DeserializeObject<WebViewMessage>(data);
                switch(mesg.Obj.Trim().ToLower())
                {
                    case "alert":
                        DisplayAlert("Alert", mesg.ParamVal, "OK");
                        break;
                    case "photo":
                        PerformPhotoAction(mesg);
                        break;
                    case "qrcode":
                        PerformQRCodeAction(mesg);
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
                        MessagingCenter.Subscribe<QRCodePage, string>(this, "QRCode", (sender, arg) =>
                        {
                            MessagingCenter.Unsubscribe<QRCodePage, string>(this, "QRCode");
                            mesg.ParamType = "String";
                            mesg.ParamVal = arg;
                            mesg.ErrorMesg = (arg != "") ? "OK" : "Failed To Scan QR Code";
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
                    if (!string.IsNullOrEmpty(mesg.CallBack))
                    {
                        await webview.EvaluateJavaScriptAsync($"{mesg.CallBack}('{JsonConvert.SerializeObject(mesg)}');");
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
            switch(key.Trim().ToLower())
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
