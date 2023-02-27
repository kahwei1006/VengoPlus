using Lavie.Data;
using Lavie.Models;
using Lavie.Pages;
using Plugin.Media;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FirebasePushNotification;
using SQLitePCL;
using System.Threading.Tasks;



namespace Lavie
{
    public partial class App : Application
    {
        static LavieDatabase database;
        public static LavieDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new LavieDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LavieDb.db3"));
                }
                return database;
            }
        }


        public App()
        {
            InitializeComponent();
            //CrossMedia.Current.Initialize();
           
            WebViewMessage mesg= new WebViewMessage();
            mesg.Action = "Get";
            mesg.Obj = "QRCode";
         //   const string TAG = "MyFirebaseIIDService";

            //  FirebaseMessaging.getToken();

            //MainPage = new NavigationPage(new TestUI());

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");

                mesg.TID = p.Token;

                

            };
            
            MainPage = new NavigationPage(new DataRoute(mesg));

            //   firebase(mesg);

            // MainPage = new TestNotification();
            // Token event
            // var FCMtoken = FirebaseMessaging.Instance.GetToken();

            //   var refreshedToken = FirebaseInstanceId.Instance.Token;
         //   MyFirebaseIIDService.

    //      CrossFirebasePushNotification.Current.GetTokenAsync().Equals( mesg.TID);

         //   mesg.TID = CrossFirebasePushNotification.Current.OnTokenRefresh.
         
        
           
          

            
        }



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
