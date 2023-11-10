// const { defineConfig } = require('@vue/cli-service')
// module.exports = defineConfig({
//   transpileDependencies: true
// })

module.exports = {
  devServer: {
    proxy: {
      "/": {
        target: "http://localhost:8003/", //接口域名
        changeOrigin: true, //是否跨域
        ws: false, //是否代理 websockets
        secure: false, //是否https接口
        pathRewrite: {

        },
      },
    },
  },
};