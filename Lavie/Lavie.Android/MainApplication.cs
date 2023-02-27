using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.Core.App;
using AndroidX.Core.Graphics.Drawable;
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Media;
using Android.Nfc;
using Android.OS;
using Android.Renderscripts;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using Lavie.Models;
using Lavie.Pages;
using Plugin.CurrentActivity;
using Plugin.FirebasePushNotification;
using System.Runtime.Remoting.Contexts;

namespace Lavie.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
	[Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {

        }

        public void onMessageReceived(RemoteMessage message)
        {
            var notification = message.GetNotification();
            var data = message.Data;
            string body = "";
            string title = "";
            if (data != null && data.ContainsKey("body") && data.ContainsKey("title"))
            {
                body = data["body"];
                title = data["title"];
            }
            else if (notification != null)
            {
                body = message.GetNotification().Body;
                title = message.GetNotification().Title;
            }
        }


        public override void OnCreate()
        {
            base.OnCreate();

            CrossCurrentActivity.Current.Init(this);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel("VengoDefaultNotification", "VengoDefaultNotification", NotificationImportance.High);
                channel.EnableVibration(true);
                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);

                // Set the default notification channel for the app
                manager.CreateNotificationChannel(channel);
            }





            //If debug you should reset the token each time.
            //#if DEBUG
            //      FirebasePushNotificationManager.Initialize(this, true);
            //#else
            //        FirebasePushNotificationManager.Initialize(this, false);
            //#endif

            //Handle notification when app is closed here




            //// Push message received event
                CrossFirebasePushNotification.Current.OnNotificationReceived += async(s, p) =>
                 {

                     if (p.Data.ContainsKey("title") && p.Data.ContainsKey("body"))
                     {
                         string title = p.Data["title"].ToString();
                         string body = p.Data["body"].ToString();
                         string url = p.Data.ContainsKey("url") ? p.Data["url"].ToString() : null;

                         var notification = new NotificationCompat.Builder(this, "VengoDefaultNotification")
                               .SetSmallIcon(Resource.Drawable.ic_launcher_round)
                                 .SetContentTitle(title)
                                 .SetContentText(body)
                                  .SetAutoCancel(true)
                                    .Build();
                         NotificationManagerCompat.From(this).Notify(0, notification);



                        // NotificationManager notificationManager = GetSystemService(NotificationService) as NotificationManager;
                        // notificationManager.Notify(0, builder.Build());


                         await Xamarin.Forms.Device.InvokeOnMainThreadAsync(async () =>
                         {
                             await App.Current.MainPage.Navigation.PushAsync(new Page1());
                         });

                     }

                 };

           

            //Push message received event
            CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
     {
         System.Diagnostics.Debug.WriteLine("Opened");
         foreach (var data in p.Data)
         {
             System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
         }

     };

 }
}
}