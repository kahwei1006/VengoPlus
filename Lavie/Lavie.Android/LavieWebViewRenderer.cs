using Android.Content;
using Android.Net;
using Java.Net;
using Lavie.CustomViews;
using Lavie.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LavieWebView), typeof(LavieWebViewRenderer))]
namespace Lavie.Droid
{
    public class LavieWebViewRenderer : WebViewRenderer
    {
        const string JavaScriptFunction = "function invokeContainerAction(data){if (jsBridge !=null) {jsBridge.invokeAction(data)} ;}";
        protected Context _context;

        public LavieWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                ((LavieWebView)Element).Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.SetWebViewClient(new JScriptWebViewClient(this, $"javascript: {JavaScriptFunction}"));
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                var uri = new System.Uri(((LavieWebView)Element).Uri);
                if (uri.Scheme == "file")
                {
                    Control.LoadUrl($"file:///android_asset/Content/{uri.IdnHost}");
                }
                else
                {
                    Control.LoadUrl(uri.OriginalString);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((LavieWebView)Element).Cleanup();
            }
            base.Dispose(disposing);
        }
    }
}