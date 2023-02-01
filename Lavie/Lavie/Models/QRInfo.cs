using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Lavie.Models
{
    internal class QRInfo
    {
        public string Id { get; set; } = "";                // To Match The State Between Calling and JavaScript Callback
        public string Action { get; set; } = "";            // Get, Set, Create, Delete
        public string Obj { get; set; } = "";               // QRCode, GPS, UUID, NewPage etc
        public string ParamType { get; set; } = "";         // Describe the type of Parameter (exception, string, integer, float, base64 etc)
        public string ParamVal { get; set; } = "";          // Parameter Payload
        public string CallBack { get; set; } = "";          // JavaScript Callback
        public string ErrorMesg { get; set; } = "";
        public string UID { get; set; } = "";
    }
}
