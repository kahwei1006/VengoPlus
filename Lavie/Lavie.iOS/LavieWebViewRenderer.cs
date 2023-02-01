using System.IO;
using Foundation;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Lavie;
using Lavie.iOS;
using Lavie.CustomViews;


[assembly: ExportRenderer(typeof(LavieWebView), typeof(LavieWebViewRenderer))]
namespace Lavie.iOS
{
    public class LavieWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
    {
        const string JavaScriptFunction = "function invokeContainerAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;

        public LavieWebViewRenderer() : this(new WKWebViewConfiguration())
        {
        }

        public LavieWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            userController = config.UserContentController;
            var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            userController.AddUserScript(script);
            userController.AddScriptMessageHandler(this, "invokeAction");
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                userController.RemoveAllUserScripts();
                userController.RemoveScriptMessageHandler("invokeAction");
                LavieWebView hybridWebView = e.OldElement as LavieWebView;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                var uri = new System.Uri(((LavieWebView)Element).Uri);
                if (uri.Scheme == "file")
                {
                    LoadRequest(new NSUrlRequest(new NSUrl(Path.Combine(NSBundle.MainBundle.BundlePath, $"Content/{uri.IdnHost}"), false)));
                }
                else
                {
                    LoadRequest(new NSUrlRequest(new NSUrl(uri.OriginalString)));
                }
            }
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ((LavieWebView)Element).InvokeAction(message.Body.ToString());
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