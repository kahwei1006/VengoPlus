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
using Java.Interop;
using Lavie.CustomViews;

namespace Lavie.Droid
{
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<LavieWebViewRenderer> hybridWebViewRenderer;

        public JSBridge(LavieWebViewRenderer hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<LavieWebViewRenderer>(hybridRenderer);
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public void InvokeAction(string data)
        {
            LavieWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                ((LavieWebView)hybridRenderer.Element).InvokeAction(data);
            }
        }
    }
}