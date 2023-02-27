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
using Android.Gms.Common;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Android.Content;
using Android.Media;

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


            base.OnCreate(savedInstanceState);


            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();                 // ZXing QRCode Scanner
            LoadApplication(new App());
            IsPlayServicesAvailable();
            FirebasePushNotificationManager.ProcessIntent(this, Intent);


        }


        public bool IsPlayServicesAvailable()
        {
            const string TAG = "MyFirebaseIIDService";

            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            Log.Debug(TAG, "IsPlayServicesAvailable:" + resultCode);
            if (resultCode != ConnectionResult.Success)
            {

                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }

                else
                {
                    // msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                // do whatever if play service is not available
                //msgText.Text = "Google Play Services is available.";
                return true;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults); // ZXing QRCode Scanner

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }


    }


}