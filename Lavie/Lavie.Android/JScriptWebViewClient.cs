using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Lavie.Droid
{
    public class JScriptWebViewClient : FormsWebViewClient
    {
        string _javascript;

        public JScriptWebViewClient(LavieWebViewRenderer renderer, string javascript) : base(renderer)
        {
            _javascript = javascript;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                view.EvaluateJavascript(_javascript, null);
            }
            else
            {
                view.LoadUrl(_javascript);
            }
        }
    }
}