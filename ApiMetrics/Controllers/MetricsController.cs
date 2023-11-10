using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PublicQuery.Command;
using Service;
using SqlSugar;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetricsController : ControllerBase
    {
        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="metricsRequest"></param>
        /// <returns></returns>
        [HttpPost("Get")]
        public async Task<JObject> Get(MetricsRequest metricsRequest)
        {
            HttpMessage msg = new HttpMessage();

            var db = Command.GetDataBase();
            var sql = db.Queryable<shakehands, contentrecord>((x, cr) => (x.crequestBodyId == cr.cid || x.cresponseBodyId == cr.cid))
                 .WhereIF(metricsRequest.startDate != null && metricsRequest.stopDate != null, x => SqlFunc.Between(x.cstartTime, metricsRequest.startDate, metricsRequest.stopDate))
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.serviceName), (x, cr) => SqlFunc.Contains(x.cserviceName, metricsRequest.serviceName))
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.method), (x, cr) => x.cmethod == metricsRequest.method)
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.url), (x, cr) => x.curl == metricsRequest.url)
                 //筛选符合内容的记录
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.content),
                 (x, cr) => SqlFunc.IIF<string>(cr.ctype == "body",
                                        SqlFunc.MappingColumn<string>("from_base64(value)"),
                                         cr.cvalue).Contains(metricsRequest.content))
                 .OrderBy((x, cr) => x.cstartTime, OrderByType.Desc)
                 .Select((x) => new shakehands()
                 {
                     cid=x.cid,
                     cserviceName = x.cserviceName
                 },true);

            var jsonReult = await sql.ToJsonPageAsync(metricsRequest.pageIndex, metricsRequest.pageSize);

            var totalRow = await sql.CountAsync();
            var result = JArray.Parse(jsonReult);
            msg.data = result;
            msg["totalPage"] = (int)Math.Ceiling((double)totalRow / metricsRequest.pageSize);
            return msg.Result();
        }

        /// <summary>
        /// 获取请求的内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Content")]
        public async Task<JObject> Content(int id = 0)
        {
            HttpMessage msg = new HttpMessage();

            var db = Command.GetDataBase();

            var taskValue = await db.Queryable<contentrecord>().Where(x => (int)(x.cid) == id).FirstAsync();

            if (taskValue != null)
            {
                msg.data = JObject.FromObject(taskValue);
            }


            return msg.Result();
        }

        /// <summary>
        /// 获取每个时间段的访问状况，支撑时间轴图
        /// </summary>
        /// <param name="metricsRequest"></param>
        /// <returns></returns>
        [HttpPost("Metrics")]
        public JObject Metrics(MetricsRequest metricsRequest)
        {
            HttpMessage msg = new HttpMessage();
            var db = Command.GetDataBase();
            var db2 = Command.GetDataBase();
            //获取条件范围内的总数量
            var requestCountTask = db.Queryable<shakehands>()
                 .WhereIF(metricsRequest.startDate != null && metricsRequest.stopDate != null, x => SqlFunc.Between(x.cstartTime, metricsRequest.startDate, metricsRequest.stopDate))
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.serviceName), x => x.cserviceName == metricsRequest.serviceName)
                 .WhereIF(!string.IsNullOrEmpty(metricsRequest.method), x => x.cmethod == metricsRequest.method)

                 .CountAsync();

            var requestTimeDistributionTask =
                db2.SqlQueryable<dynamic>("select CONCAT(LEFT(shakehands.startTime,13),':00:00') as 'cstarttime',count(*) as 'count' from shakehands group by CONCAT(LEFT(shakehands.startTime,13),':00:00')")
                .ToJsonAsync();

            requestCountTask.Wait();
            requestTimeDistributionTask.Wait();

            var obj = new JObject();

            obj["requestCount"] = requestCountTask.Result;
            obj["requestTimeDistribution"] = string.IsNullOrEmpty(requestTimeDistributionTask.Result) ? null : JArray.Parse(requestTimeDistributionTask.Result);

            msg.data = obj;

            return msg.Result();

        }

        /// <summary>
        /// 获取一定条件内的Api调用排序
        /// </summary>
        /// <param name="metricsRequest"></param>
        /// <returns></returns>
        [HttpPost("Rank")]
        public JObject Rank(MetricsRequest metricsRequest)
        {
            HttpMessage msg = new HttpMessage();

            var db = Command.GetDataBase();
            var jsonStr = db.Queryable<shakehands>()
                .WhereIF(metricsRequest.startDate != null && metricsRequest.stopDate != null, x => SqlFunc.Between(x.cstartTime, metricsRequest.startDate, metricsRequest.stopDate))
                .WhereIF(!string.IsNullOrEmpty(metricsRequest.serviceName), x => x.cserviceName == metricsRequest.serviceName)
                .WhereIF(!string.IsNullOrEmpty(metricsRequest.method), x => x.cmethod == metricsRequest.method)
                .GroupBy(x => x.curl)
                .Select(x => new
                {
                    x.curl,
                    count = SqlFunc.AggregateCount(x.curl)
                })
                .OrderBy(x => SqlFunc.AggregateCount(x.curl), OrderByType.Desc)
                .ToJsonPage(1, 5);

            msg.data = JArray.Parse(jsonStr);

            return msg.Result();
        }
    }

    /// <summary>
    /// 请求参数实体
    /// </summary>
    public class MetricsRequest
    {
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 100;
        public DateTime? startDate { get; set; }
        public DateTime? stopDate { get; set; }
        public string method { get; set; }
        public string serviceName { get; set; }
        public string url { get; set; }
        public string? content { get; set; }
    }
}
