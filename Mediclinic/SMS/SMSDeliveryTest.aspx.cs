using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Specialized;
using System.Net;

public partial class SMSDeliveryTest : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string url = HttpContext.Current.Request.Url.AbsoluteUri.Replace();

        string withoutHost = HttpContext.Current.Request.Url.AbsoluteUri.Substring(0, HttpContext.Current.Request.Url.AbsoluteUri.IndexOf("/SMS/") );
        Http.Post(withoutHost + "/SMS/SMSDeliveryNotice.aspx", new NameValueCollection() {
            { "message_id", "some msg 1" },
            { "mobile",     "some msg 2" },
            { "datetime",   "blegh 3"    },
            { "status",     "ok"         }
        });

    }

    public static class Http
    {
        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            return response;
        }
    }

}