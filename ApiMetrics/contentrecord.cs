using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Service
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("contentrecord")]
    public partial class contentrecord
    {
           public contentrecord(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true,ColumnName="id")]
           public object cid {get;set;}

           /// <summary>
           /// Desc:header或body
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="value")]
           public string cvalue {get;set;}

           /// <summary>
           /// Desc:request或者body
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="type")]
           public string ctype {get;set;}

           /// <summary>
           /// Desc:md5
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="md5")]
           public string cmd5 {get;set;}

           /// <summary>
           /// Desc:sha1
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="sha1")]
           public string csha1 {get;set;}

    }
}
