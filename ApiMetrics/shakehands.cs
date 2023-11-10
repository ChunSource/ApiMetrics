using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Service
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("shakehands")]
    public partial class shakehands
    {
        public shakehands()
        {


        }
        /// <summary>
        /// Desc:自增id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
        public object cid { get; set; }

        /// <summary>
        /// Desc:服务的名称
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "serviceName")]
        public string cserviceName { get; set; }

        /// <summary>
        /// Desc:url的最长长度
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "url")]
        public string curl { get; set; }

        /// <summary>
        /// Desc:Get时的查询内容
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "queryValue")]
        public string cqueryValue { get; set; }

        /// <summary>
        /// Desc:请求的头部
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "requestHeaderId")]
        public object crequestHeaderId { get; set; }

        /// <summary>
        /// Desc:请求的body
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "requestBodyId")]
        public object crequestBodyId { get; set; }

        /// <summary>
        /// Desc:响应的头部
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "responseHeaderId")]
        public object cresponseHeaderId { get; set; }

        /// <summary>
        /// Desc:响应的尾部
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "responseBodyId")]
        public object cresponseBodyId { get; set; }

        /// <summary>
        /// Desc:返回状态码
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "statusCode")]
        public int? cstatusCode { get; set; }

        /// <summary>
        /// Desc:该过程的开始时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "startTime")]
        public DateTime? cstartTime { get; set; }

        /// <summary>
        /// Desc:该过程的结束时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "stopTime")]
        public DateTime? cstopTime { get; set; }

        /// <summary>
        /// Desc:请求方法
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "method")]
        public string cmethod { get; set; }

        /// <summary>
        /// Desc:花费时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "spend")]
        public int? cspend { get; set; }

        /// <summary>
        /// Desc:ip地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "clientip")]
        public string clientip { get; set; }
    }
}
