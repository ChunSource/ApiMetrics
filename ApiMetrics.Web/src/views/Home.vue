<template>
  <div class="Home">
    <el-form label-width="80px">
      <el-row>
        <el-col :md="8">
          <el-form-item label="日期范围">
            <el-date-picker size="small" v-model="requestArgs.choeseDate" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" :picker-options="dateOptions">
            </el-date-picker>
          </el-form-item>
        </el-col>
        <el-col :md="3">
          <el-form-item label="方法">
            <el-select size="small" v-model="requestArgs.methodValue" placeholder="请选择">
              <el-option v-for="item in requestArgs.methodOptions" :key="item.value" :label="item.label" :value="item.value">
              </el-option>
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :md="4">
          <el-form-item label="服务名">
            <el-input size="small" v-model="requestArgs.serviceName" placeholder="服务名称"></el-input>
          </el-form-item>
        </el-col>
        <el-col :md="4">
          <el-form-item label="url">
            <el-input size="small" v-model="requestArgs.url" placeholder="url"></el-input>
          </el-form-item>
        </el-col>
        <el-col :md="2">
          <el-form-item label="">
            <el-button size="small" type="success" @click="getMetrics">查询</el-button>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :md="24">
          <el-input type="textarea" :autosize="{ minRows: 2}" placeholder="请输入需要过滤的内容" v-model="requestArgs.content">
          </el-input>
        </el-col>
      </el-row>

    </el-form>

    <!-- 下面 -->

    <!-- 下面是数据图 -->
    <el-row>
      <el-col :md="8">
        <div id="area-time-axis" style="width:100%;height:200px"></div>
      </el-col>
      <el-col :md="16">
        <div id="rank" style="width:100%;height:200px"></div>
      </el-col>
    </el-row>

    <!-- 下面是请求列表 -->
    <el-table size="small" v-loading="tableIsLoading" element-loading-text="拼命加载中" element-loading-spinner="el-icon-loading" element-loading-background="rgba(0, 0, 0, 0.8)" :data="tableMetricsData" stripe style="width: 100%" :max-height="tableMaxHeight">
      <el-table-column prop="cid" label="id" width="100">
      </el-table-column>
      <el-table-column prop="cserviceName" label="服务名称" width="100">
      </el-table-column>
      <el-table-column prop="cmethod" label="方法" width="70">
      </el-table-column>
      <el-table-column prop="curl" label="url" width="200">
      </el-table-column>
      <el-table-column prop="cqueryValue" label="QueryValue" width="250">
      </el-table-column>
      <el-table-column prop="cstartTime" label="开始时间" width="150">
      </el-table-column>
      <el-table-column prop="cspend" label="耗时" width="70">
      </el-table-column>
      <el-table-column prop="cstatusCode" label="状态码" width="70">
      </el-table-column>
      <el-table-column prop="clientip" label="ip地址" width="140">
      </el-table-column>
      <el-table-column label="报文">
        <template #default="scope">
          <el-button size="mini" @click="showPlantext(scope.row)">查看报文</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination v-model:current-page="requestArgs.pageIndex" background layout="sizes,prev, pager, next" :page-size="requestArgs.pageSize" :page-sizes="[10, 50, 100, 300]" :page-count="requestArgs.totalPage" @size-change="getMetrics" @current-change="getMetrics">
    </el-pagination>

    <!-- 报文显示 -->
    <el-dialog title="报文详情" v-model="showPlanTextModel" width="90%">
      <div>请求报文</div>
      <el-input type="textarea" :autosize="{ minRows: 2, maxRows: 10}" placeholder="请求报文" v-model="currentShowPlanText.request">
      </el-input>
      <el-divider></el-divider>
      <div>响应报文</div>
      <el-input type="textarea" :autosize="{ minRows: 2, maxRows: 10}" placeholder="请求报文" v-model="currentShowPlanText.response">
      </el-input>
    </el-dialog>
  </div>
</template>

<script>
import Api from '../api/api'
import pako from 'pako'
import * as echarts from 'echarts';

export default {
  name: 'Home',
  components: {

  },
  mounted() {
    this.tableMaxHeight = window.screen.height - 350
  },
  data() {
    return {
      requestArgs: {
        pageIndex: 1,
        pageSize: 10,
        totalRow: 0,
        serviceName: '',
        url: '',
        content: '',
        choeseDate: [],
        methodValue: '',
        methodOptions: [{
          label: 'ALL',
          value: ''
        },
        {
          label: 'GET',
          value: 'GET'
        },
        {
          label: 'POST',
          value: 'POST'
        }]
      },
      //日期快捷选项
      dateOptions: {
        shortcuts: [{
          text: '最近一天',
          onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24);
            picker.$emit('pick', [start, end]);
          }
        }, {
          text: '最近一周',
          onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
            picker.$emit('pick', [start, end]);
          }
        }, {
          text: '最近一个月',
          onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
            picker.$emit('pick', [start, end]);
          }
        }, {
          text: '最近三个月',
          onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
            picker.$emit('pick', [start, end]);
          }
        }]
      },
      //是否正在加载数据
      tableIsLoading: false,
      //表的最大高度
      tableMaxHeight: 200,
      //表的数据
      tableMetricsData: [],
      //是否显示报文
      showPlanTextModel: false,
      //当前显示的报文
      currentShowPlanText: {
        request: '',
        response: ''
      },
      //过滤报文内容
      contentValueFilter: '',
      //雨量图echarts对象
      area_time_axis_obj: null,
      //排行榜echarts对象
      rankd_obj: null,
    }
  },
  methods: {
    getMetrics() {
      this.tableIsLoading = true
      var pageIndex = this.requestArgs.pageIndex
      var pageSize = this.requestArgs.pageSize
      var startDate = null
      var stopDate = null

      var serviceName = this.requestArgs.serviceName
      var url = this.requestArgs.url
      var method = this.requestArgs.methodValue
      var content = this.requestArgs.content
      if (this.requestArgs.choeseDate.length == 2) {
        startDate = this.requestArgs.choeseDate[0];
        stopDate = this.requestArgs.choeseDate[1];
      }

      Api.getMetrics(pageIndex, pageSize, startDate, stopDate, method, serviceName, url, content).then(res => {
        console.log(res)
        this.tableMetricsData = res.data.data
        this.requestArgs.totalRow = res.data.totalRow
        this.requestArgs.totalPage = res.data.totalPage
        this.tableIsLoading = false
      }).catch(error => {
        this.tableIsLoading = false
      })

      this.initarea_time_axis_obj()
      this.init_rank_charts()
    },
    showPlantext(row) {
      var that = this
      var count = 0
      var reqHeaderTask = Api.getContent(row.crequestHeaderId)
      var reqBodyTask = Api.getContent(row.crequestBodyId)
      var repHeaderTask = Api.getContent(row.cresponseHeaderId)
      var repBodyTask = Api.getContent(row.cresponseBodyId)

      var reqHeaderText;
      var reqBodyText;
      var repHeaderText;
      var repBodyText

      reqHeaderTask.then(res => {
        reqHeaderText = res.data.data.cvalue
        count++
        if (count == 4) {
          //拼接请求
          var req = row.cmethod + " " + row.curl + row.cqueryValue + "\n"
          req = req + reqHeaderText + "\n"
          req = req + (reqBodyText == null ? "" : decodeURIComponent(escape(window.atob(reqBodyText))))
          that.currentShowPlanText.request = req
          //拼接响应
          var rep = repHeaderText + decodeURIComponent(escape(window.atob(repBodyText)))
          that.currentShowPlanText.response = rep
          that.showPlanTextModel = true
        }
      })

      reqBodyTask.then(res => {
        reqBodyText = res.data.data.cvalue
        count++
        if (count == 4) {
          //拼接请求
          var req = row.cmethod + " " + row.curl + row.cqueryValue + "\n"
          req = req + reqHeaderText + "\n"
          req = req + (reqBodyText == null ? "" : decodeURIComponent(escape(window.atob(reqBodyText))))
          that.currentShowPlanText.request = req
          //拼接响应
          var rep = repHeaderText + decodeURIComponent(escape(window.atob(repBodyText)))
          that.currentShowPlanText.response = rep
          that.showPlanTextModel = true
        }
      })

      repHeaderTask.then(res => {
        repHeaderText = res.data.data.cvalue
        count++
        if (count == 4) {
          //拼接请求
          var req = row.cmethod + " " + row.curl + row.cqueryValue + "\n"
          req = req + reqHeaderText + "\n"
          req = req + (reqBodyText == null ? "" : decodeURIComponent(escape(window.atob(reqBodyText))))
          that.currentShowPlanText.request = req
          //拼接响应
          var rep = repHeaderText + decodeURIComponent(escape(window.atob(repBodyText)))
          that.currentShowPlanText.response = rep
          that.showPlanTextModel = true
        }
      })

      repBodyTask.then(res => {
        repBodyText = res.data.data.cvalue
        count++
        if (count == 4) {
          //拼接请求
          var req = row.cmethod + " " + row.curl + row.cqueryValue + "\n"
          req = req + reqHeaderText + "\n"
          req = req + (reqBodyText == null ? "" : decodeURIComponent(escape(window.atob(reqBodyText))))
          that.currentShowPlanText.request = req
          //拼接响应
          var rep = repHeaderText + decodeURIComponent(escape(window.atob(repBodyText)))
          that.currentShowPlanText.response = rep
          that.showPlanTextModel = true
        }
      })
    },
    //设置时间段访问量图表
    initarea_time_axis_obj() {

      var pageIndex = this.requestArgs.pageIndex
      var pageSize = this.requestArgs.pageSize
      var startDate = null
      var stopDate = null

      var serviceName = this.requestArgs.serviceName
      var url = this.requestArgs.url
      var method = this.requestArgs.methodValue

      if (this.requestArgs.choeseDate.length == 2) {
        startDate = this.requestArgs.choeseDate[0];
        stopDate = this.requestArgs.choeseDate[1];
      }
      var that = this
      Api.getTimeDistribution(pageIndex, pageSize, startDate, stopDate, method, serviceName, url).then(res => {
        console.log(res)
        var dataset = []

        var data = res.data.data
        data.requestTimeDistribution.forEach(element => {
          dataset.push([element.cstarttime, element.count])
        });

        var options = {
          tooltip: {
            trigger: 'axis',
            position: function (pt) {
              return [pt[0], '10%'];
            }
          },
          grid: {

          },
          title: {
            left: 'center',
            text: '访问量'
          },
          xAxis: {
            type: 'time',
            boundaryGap: false,
          },
          yAxis: {
            type: 'value',
          },
          dataZoom: [
            {
              type: 'inside',
              start: 0,
              end: 20
            },
            {
              start: 0,
              end: 20
            }
          ],
          series: [
            {
              name: '请求数',
              type: 'line',
              smooth: true,
              symbol: 'none',
              areaStyle: {},
              data: dataset
            }
          ]
        };

        //如果没有初始化过就初始化
        if (that.area_time_axis_obj == null) {
          var chartDom = document.getElementById('area-time-axis');
          that.area_time_axis_obj = echarts.init(chartDom);
        }

        that.area_time_axis_obj.setOption(options, true)

      }).catch(error => {
      })
    },
    //接口访问排行榜
    init_rank_charts() {
      var pageIndex = this.requestArgs.pageIndex
      var pageSize = this.requestArgs.pageSize
      var startDate = null
      var stopDate = null

      var serviceName = this.requestArgs.serviceName
      var url = this.requestArgs.url
      var method = this.requestArgs.methodValue

      if (this.requestArgs.choeseDate.length == 2) {
        startDate = this.requestArgs.choeseDate[0];
        stopDate = this.requestArgs.choeseDate[1];
      }
      var that = this
      Api.getRank(pageIndex, pageSize, startDate, stopDate, method, serviceName, url)
        .then(res => {
          var data = res.data.data

          var dataset = []
          data.forEach(element => {
            dataset.push([element.count, element.curl])
          });
          dataset = dataset.reverse()
          console.log('RankDataSet')
          console.log(dataset)

          if (that.rankd_obj == null) {
            var chartDom = document.getElementById('rank');
            that.rankd_obj = echarts.init(chartDom);
          }

          var option = {
            title: {
              text: '方法调用排行榜'
            },
            tooltip: {
              trigger: 'axis',
              axisPointer: {
                type: 'shadow'
              }
            },
            grid: {
              width: '100%',
              height: '70%',
              containLabel: true
            },
            xAxis: {
              type: 'value',
              boundaryGap: ['20%', '0%']
            },
            yAxis: {
              //show: false,
              type: 'category',
            },
            series: [
              {
                name: '排行榜',
                type: 'bar',
                data: dataset,
              }
            ]
          };

          that.rankd_obj.setOption(option, true)

        })
    },
  }
}
</script>

<style scoped>
.el-table {
  max-height: none !important;
}
</style>