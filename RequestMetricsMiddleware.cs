using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Newtonsoft.Json.Linq;
using NLog.Internal;
using Service;
using SqlSugar;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PublicQuery.Middleware
{
    public static class RequestMetricsMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestMetrics(
           this IApplicationBuilder builder, Action<RequestMetricsMiddlewareOptions> options)
        {
            var option = new RequestMetricsMiddlewareOptions();
            options(option);
            return builder.UseMiddleware<RequestMetricsMiddleware>(option);
        }
    }

    public class RequestMetricsMiddlewareOptions
    {
        public string serviceName { get; set; }
    }


    //这里我们捕捉我们需要用到的信息，并发送到服务器上面去
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestMetricsMiddlewareOptions _options;
        public RequestMetricsMiddleware(RequestDelegate next,
            RequestMetricsMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            Stream oldBody = null;
            MemoryStream responseReader = null;
            try
            {
                //打开buffer，允许读取request
                httpContext.Request.EnableBuffering();

                var requestReader = new MemoryStream();

                httpContext.Request.Body.Position = 0;
                httpContext.Request.Body.CopyToAsync(requestReader).Wait();
                httpContext.Request.Body.Position = 0;
                var requestBody = Encoding.UTF8.GetString(requestReader.ToArray());

                

                shakehands sh = new shakehands();
                //记录开始时间
                var startTime = new DateTimeOffset(DateTime.UtcNow).ToLocalTime().ToString();

                //新的stream用于替换原来的body中的stream
                responseReader = new MemoryStream();
                responseReader.Position = 0;
                oldBody = httpContext.Response.Body;
                httpContext.Response.Body = responseReader;

                //----------------等待下游处理完毕回来-------------------
                await _next(httpContext);
                //----------------等待下游处理完毕回来-------------------
                

                string ClientIP = httpContext.Request.Headers["X-Real-IP"];

                //获取request
                var method = httpContext.Request.Method;
                var url = httpContext.Request.Path;
                var query = httpContext.Request.QueryString.Value;
                var reqHeader = httpContext.Request.Headers;
                

                string requestHeaders = string.Empty;
                foreach (var head in reqHeader)
                {
                    string key = head.Key;
                    string value = head.Value;
                    requestHeaders = requestHeaders + key + ": " + value + "\n";
                }

                //获取response

                //结束时间
                var stopTime = new DateTimeOffset(DateTime.UtcNow).ToLocalTime().ToString();

                responseReader.Position = 0;
                //状态码
                var statusCode = httpContext.Response.StatusCode;
                await responseReader.CopyToAsync(oldBody);
                responseReader.Position = 0;
                //header
                var respHeader = httpContext.Response.Headers;
                //body
                string respbody = Encoding.UTF8.GetString(responseReader.ToArray());
                //用时
                var spend = DateTime.Parse(startTime) - DateTime.Parse(startTime);

                string responseHeaders = string.Empty;
                foreach (var head in respHeader)
                {
                    string key = head.Key;
                    string value = head.Value;
                    responseHeaders = responseHeaders + key + ": " + value + "\n";
                }

                sh.cid = 0;
                sh.cserviceName = _options.serviceName;
                sh.curl = url;
                sh.cmethod = method;
                sh.cqueryValue = query;
                sh.cstartTime = DateTime.Parse(startTime);

                sh.crequestHeaderId = requestHeaders;
                //杂乱的东西多,base64处理一下
                sh.crequestBodyId = Command.Command.Base64Encode(requestBody);
                sh.cresponseHeaderId = responseHeaders;
                sh.cresponseBodyId = Command.Command.Base64Encode(respbody);
                sh.cstatusCode = statusCode;
                sh.cstopTime = DateTime.Parse(stopTime);
                sh.cspend = spend.Milliseconds;
                sh.clientip = string.IsNullOrEmpty(ClientIP) ? "" : ClientIP;
#if DEBUG
#else
                var task = Task.Run(() =>
                {
                    try
                    {
                        string url = "http://localhost:8003/api/Recv/Recv";

                        JObject par = JObject.FromObject(sh);
                        Stopwatch watch = new Stopwatch();
                        watch.Start();
                        HttpClient client = new HttpClient();

                        var content = new StringContent(par.ToString(), Encoding.UTF8, "application/json");

                        var task = client.PostAsync(url, content);
                        task.Wait();
                        HttpResponseMessage response = task.Result;
                        byte[] result = response.Content.ReadAsByteArrayAsync().Result;
                        watch.Stop();
                    }catch(Exception e)
                    {
                        Console.WriteLine("上传请求时发生异常");
                    }
                    
                });
#endif
                httpContext.Response.Body = oldBody;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                {
                    if(responseReader != null)
                    {
                        await responseReader.CopyToAsync(oldBody);
                        httpContext.Response.Body = oldBody;
                    }
                    
                }
            }
            
        }
    
        private string ascillToutf8(string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            byte[] result = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, bytes);
            return Encoding.UTF8.GetString(result);
        }
    }

}

