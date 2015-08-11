using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Mvc;


/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
    class CapturingResponseFilter : Stream
    {
        private Stream _sink;
        private MemoryStream mem;

        public APIAccessLogItem AccessLogItem { get; set; }

        public CapturingResponseFilter(Stream sink, APIAccessLogItem item)
        {
            _sink = sink;
            AccessLogItem = item;
            mem = new MemoryStream();
        }

        public override bool CanRead
        {
            get { return _sink.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _sink.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _sink.CanWrite; }
        }

        public override long Length
        {
            get { return _sink.Length; }
        }

        public override long Position
        {
            get
            {
                return _sink.Position;
            }
            set
            {
                _sink.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin direction)
        {
            return _sink.Seek(offset, direction);
        }

        public override void SetLength(long length)
        {
            _sink.SetLength(length);
        }

        public override void Close()
        {
            _sink.Close();
            mem.Close();
        }

        public override void Flush()
        {
            _sink.Flush();

            AccessLogItem.ResponseBody = GetContents(new UTF8Encoding(false));
            //YOU CAN STORE YOUR DATA TO YOUR DATABASE HERE
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _sink.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            mem.Write(buffer, 0, count);
            _sink.Write(buffer, offset, count);
        }

        public string GetContents(Encoding enc)
        {
            var buffer = new byte[mem.Length];
            mem.Position = 0;
            mem.Read(buffer, 0, buffer.Length);
            return enc.GetString(buffer, 0, buffer.Length);
        }
    }


    public class LoggerFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            HttpRequestBase request = filterContext.HttpContext.Request;

            APIAccessLogItem item;

            try
            {

                var controller = filterContext.Controller;
                var routeData = request.RequestContext.RouteData;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < request.Headers.Count; i++)
                    sb.AppendFormat("{0}={1};", request.Headers.Keys[i],
                    request.Headers[i].ToString());

                item = new APIAccessLogItem()
                {
                    RequestDate = DateTime.Now,
                    RequestType = request.RequestType,
                    Url = request.RawUrl,
                    IPAddress = request.UserHostAddress,
                    Controller = (string)routeData.Values["controller"],
                    Action = (string)routeData.Values["action"],
                    RequestHeader = sb.ToString()
                };

                sb = null;

                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    try
                    {
                        request.InputStream.Position = 0;
                        item.RequestBody = reader.ReadToEnd();
                    }
                    catch (Exception ex)
                    {
                        item.RequestBody = string.Empty;
                        //log errors
                    }
                    finally
                    {
                        request.InputStream.Position = 0;
                    }
                }

                filterContext.HttpContext.Response.Filter = new CapturingResponseFilter(filterContext.HttpContext.Response.Filter, item);
            }
            catch (Exception ex)
            {
                //log errors
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            try
            {
                HttpResponseBase response = filterContext.HttpContext.Response;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < response.Headers.Count; i++)
                    sb.AppendFormat("{0}={1};", response.Headers.Keys[i],
                    response.Headers[i].ToString());

                var filter = (CapturingResponseFilter)filterContext.HttpContext.Response.Filter;
                var item = filter.AccessLogItem;

                item.ResponseDate = DateTime.Now;
                item.ResponseHeader = sb.ToString();

                sb = null;
            }
            catch (Exception ex)
            {
                //log errors
            }
        }
    }


    public class APIAccessLogItem
    {
        public long ID { get; set; }
        public string Url { get; set; }
        public string RequestType { get; set; }
        public string RequestHeader { get; set; }
        public string RequestBody { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public DateTime RequestDate { get; set; }
        public string ResponseHeader { get; set; }
        public string ResponseBody { get; set; }
        public DateTime ResponseDate { get; set; }
    }

}