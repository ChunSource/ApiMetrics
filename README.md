# ApiMetrics

> 没有好用的Http api调用统计工具，或者要收费，遂自己写了个简单的，可以单独使用，也可以配合net core，界面也非常简单

## 安装
- 安装了VS第一遍编译会提示你缺什么库，直接安装就是了
- shakehands.sql 保存简单的调用信息
- contentrecord保存详细的http报文信息，header是明文保存，body是base64过后的。可以根据自己需求改动，或者提个意见我改也可以
- Command.cs里面的connstring改成自己的数据库连接即可

## 使用

- net core 代码使用
```
app.UseRequestMetrics(prop=>{
  pro.serviceName="CustomCurrentServiceName"
})
```
- 其他程序请按以下json结构发送
```
POST /api/Recv/Recv HTTP/1.1
Host: localhost:8003
traceparent: 00-4b0da76daf1796c8acbe8f55c00960c1-02a0ef72f86d3ae1-00
Content-Type: application/json; charset=utf-8
Content-Length: 1234

{
  "cid": 0,
  "cserviceName": "PublicQuery",
  "curl": "/api/Vesslinfo",
  "cqueryValue": "",
  "crequestHeaderId": "Accept: */*\nConnection: keep-alive\nHost: 127.0.0.1:8000\nUser-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0\nAccept-Encoding: gzip, deflate, br\nAccept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6\nContent-Type: application/x-www-form-urlencoded\nReferer: http://127.0.0.1:8000/myapi/index.html\nsec-ch-ua: \"Microsoft Edge\";v=\"119\", \"Chromium\";v=\"119\", \"Not?A_Brand\";v=\"24\"\nknife4j-gateway-code: ROOT\nsec-ch-ua-mobile: ?0\nRequest-Origion: Knife4j\nsec-ch-ua-platform: \"Windows\"\nSec-Fetch-Site: same-origin\nSec-Fetch-Mode: cors\nSec-Fetch-Dest: empty\n",
  "crequestBodyId": "",
  "cresponseHeaderId": "Content-Type: application/json; charset=utf-8\nDate: Fri, 10 Nov 2023 05:49:45 GMT\nServer: Kestrel\nContent-Length: 46\n",
  "cresponseBodyId": "eyJjb2RlIjo0MDAsIm1lc3NhZ2UiOiLor4Hkuablj7fkuI3lj6/kuLrnqboifQ==",
  "cstatusCode": 200,
  "cstartTime": "2023-11-10T13:49:46+08:00",
  "cstopTime": "2023-11-10T13:49:46+08:00",
  "cmethod": "GET",
  "cspend": 0,
  "clientip": ""
}
```

![](https://github.com/ChunSource/ApiMetrics/blob/main/1.png?raw=true)

![](https://github.com/ChunSource/ApiMetrics/blob/main/2.png?raw=true)

![](https://github.com/ChunSource/ApiMetrics/blob/main/3.png?raw=true)