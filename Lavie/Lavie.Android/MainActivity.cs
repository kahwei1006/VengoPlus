using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using Firebase.Messaging;
using Android.Gms.Extensions;
using Firebase.Iid;
using Android.Nfc;
using Android.Util;
using Firebase.Installations;

namespace Lavie.Droid
{
    //[Activity(Label = "Lavie", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [Activity(Label = "VengoPlus", Icon = "@mipmap/ic_launcher_round", Theme = "@style/MyTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            SetTheme(Resource.Style.MainTheme); // <-- Added
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

          
            // MyFirebaseIIDService.OnTokenRefresh();
            //
            //var refreshedToken = FirebaseInstanceId.Instance.Token;
            //  Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            //   var FCMtoken = FirebaseMessaging.Instance.GetToken() .GetResult(Java.Lang.Class.FromType(typeof(InstallationTokenResult))).ToString();//
            //  FirebaseMessaging.Instance.GetToken();
            base.OnCreate(savedInstanceState);
      //      var refreshedToken = FirebaseInstanceId.Instance.Token;
        //    var token = FirebaseMessaging.Instance.GetToken();
       //     token.ToString();

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();                 // ZXing QRCode Scanner
            LoadApplication(new App());

            FirebasePushNotificationManager.ProcessIntent(this, Intent);


        }
 

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults); // ZXing QRCode Scanner
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }


}