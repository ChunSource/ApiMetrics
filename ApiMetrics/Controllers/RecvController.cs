using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PublicQuery.Command;
using Service;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using OracleInternal.Secure.Network;

namespace ApiMetrics.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/Recv")]
    [ApiController]
    public class RecvController : ControllerBase
    {
        public partial class Request
        {
            /// <summary>
            /// Desc:自增id
            /// Default:
            /// Nullable:False
            /// </summary>           

            public object cid { get; set; }

            /// <summary>
            /// Desc:服务的名称
            /// Default:
            /// Nullable:False
            /// </summary>           

            public string cserviceName { get; set; }

            /// <summary>
            /// Desc:url的最长长度
            /// Default:
            /// Nullable:False
            /// </summary>           

            public string curl { get; set; }

            /// <summary>
            /// Desc:Get时的查询内容
            /// Default:
            /// Nullable:True
            /// </summary>           

            public string cqueryValue { get; set; }

            /// <summary>
            /// Desc:请求的头部
            /// Default:
            /// Nullable:True
            /// </summary>           

            public object crequestHeaderId { get; set; }

            /// <summary>
            /// Desc:请求的body
            /// Default:
            /// Nullable:True
            /// </summary>           

            public object crequestBodyId { get; set; }

            /// <summary>
            /// Desc:响应的头部
            /// Default:
            /// Nullable:True
            /// </summary>           

            public object cresponseHeaderId { get; set; }

            /// <summary>
            /// Desc:响应的尾部
            /// Default:
            /// Nullable:True
            /// </summary>           

            public object cresponseBodyId { get; set; }

            /// <summary>
            /// Desc:返回状态码
            /// Default:
            /// Nullable:True
            /// </summary>           

            public int? cstatusCode { get; set; }

            /// <summary>
            /// Desc:该过程的开始时间
            /// Default:
            /// Nullable:True
            /// </summary>           

            public DateTime? cstartTime { get; set; }

            /// <summary>
            /// Desc:该过程的结束时间
            /// Default:
            /// Nullable:True
            /// </summary>           

            public DateTime? cstopTime { get; set; }

            /// <summary>
            /// Desc:请求方法
            /// Default:
            /// Nullable:True
            /// </summary>           

            public string cmethod { get; set; }

            /// <summary>
            /// Desc:花费时间
            /// Default:
            /// Nullable:True
            /// </summary>           

            public int? cspend { get; set; }

        }

        /// <summary>
        /// 接收请求指标
        /// </summary>
        /// <returns></returns>
        [HttpPost("Recv")]
        public async Task<JObject> Recv([FromBody] shakehands sh)
        {
            HttpMessage msg = new HttpMessage();

            var db = Command.GetDataBase();

            string requestHeader = (string)sh.crequestHeaderId;
            string requestBody = (string)sh.crequestBodyId;
            string responseHeader = (string)sh.cresponseHeaderId;
            string responseBody = (string)sh.cresponseBodyId;

            var requestHeaderIdTask = GetValueId(requestHeader, "header");
            var requestBodyIdTask = GetValueId(requestBody, "body");
            var responseHeaderIdTask = GetValueId(responseHeader, "header");
            var responseBodyIdTask = GetValueId(responseBody, "body");

            requestHeaderIdTask.Wait();
            requestBodyIdTask.Wait();
            responseHeaderIdTask.Wait();
            responseBodyIdTask.Wait();

            sh.cid = 0;
            sh.crequestHeaderId = requestHeaderIdTask.Result;
            sh.crequestBodyId = requestBodyIdTask.Result;
            sh.cresponseHeaderId = responseHeaderIdTask.Result;
            sh.cresponseBodyId = responseBodyIdTask.Result;

            await db.Insertable(sh).ExecuteCommandAsync();

            return msg.Result();
        }

        private async Task<UInt64> GetValueId(string value, string type)
        {
            var db = Command.GetDataBase();

            string md5 = MD5(value);
            string sha1 = SHA1(value);

            var record = await db.Queryable<contentrecord>().Where(x => x.csha1 == sha1 && x.cmd5 == md5).FirstAsync();

            if (record == null)
            {
                //插入一个新的
                record = new contentrecord();
                record.csha1 = sha1;
                record.cvalue = value;
                record.cmd5 = md5;
                record.ctype = type;
                var newID = await db.Insertable(record).ExecuteReturnBigIdentityAsync();
                return (ulong)newID;
            }

            return (UInt64)record.cid;
        }

        private static string MD5(string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string enc = BitConverter.ToString(md5.ComputeHash(bytes));
            enc = enc.Replace("-", "").ToLower();
            return enc;
        }

        private static string SHA1(string input)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string enc = BitConverter.ToString(sha1.ComputeHash(bytes));
            enc = enc.Replace("-", "").ToLower();
            return enc;
        }
    }
}
