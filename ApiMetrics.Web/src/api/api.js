import axios from "axios";

let Api = {
  /*用户接口*/
  mpost(url, data) {
    return new Promise((reslove, reject) => {
      axios.post(url, data).then((res) => {
        reslove(res);
      });
    });
  },
  mget(url, mparams) {
    return new Promise((reslove, reject) => {
      // axios.get(url,{params: { 'certno': mparams }}).then(res => {
      // 	reslove(res)
      // })

      axios.get(url, { params: mparams }).then((res) => {
        reslove(res);
      });
    });
  },
  //获取请求列表
  getMetrics(pageIndex, pageSize, startDate, stopDate, method, serviceName,url,content) {
    var par = {
      pageIndex,
      pageSize,
      startDate,
      stopDate,
      method,
      serviceName,
      url,
      content
    };
    return Api.mpost("/api/Metrics/Get", par);
  },
  //获取时间线
  // getMetricsAreaTime(pageIndex, pageSize, startDate, stopDate, method, serviceName,url){
  //   var par = {
  //     pageIndex,
  //     pageSize,
  //     startDate,
  //     stopDate,
  //     method,
  //     serviceName,
  //     url
  //   };
  //   return Api.mpost("/api/Metrics/Metrics", par);
  // },
  //获取报文正文
  getContent(id) {
    var par = {
      id,
    };
    return Api.mget("/api/Metrics/Content", par);
  },
  //获取每小时的访问量
  getTimeDistribution(pageIndex, pageSize, startDate, stopDate, method, serviceName,url){
    var par = {
      pageIndex,
      pageSize,
      startDate,
      stopDate,
      method,
      serviceName,
      url
    };
    return Api.mpost("/api/Metrics/Metrics", par);
  },
  //获取排行榜
  getRank(pageIndex, pageSize, startDate, stopDate, method, serviceName,url){
    var par = {
      pageIndex,
      pageSize,
      startDate,
      stopDate,
      method,
      serviceName,
      url
    };
    return Api.mpost("/api/Metrics/Rank", par);
  }
};
export default Api;
