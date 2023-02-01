using Lavie.Data;
using Lavie.Models;
using Lavie.Pages;
using Plugin.Media;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

           MainPage = new NavigationPage(new DataRoute(mesg));
           //MainPage = new NavigationPage(new TestUI());
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
