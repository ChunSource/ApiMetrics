/*
 Navicat Premium Data Transfer

 Source Server         : newGZBL
 Source Server Type    : MySQL
 Source Server Version : 80022
 Source Host           : 111.230.187.253:57528
 Source Schema         : handover

 Target Server Type    : MySQL
 Target Server Version : 80022
 File Encoding         : 65001

 Date: 10/11/2023 11:11:25
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for shakehands
-- ----------------------------
DROP TABLE IF EXISTS `shakehands`;
CREATE TABLE `shakehands`  (
  `id` bigint(0) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT '自增id',
  `serviceName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '服务的名称',
  `url` varchar(2084) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT 'url的最长长度',
  `queryValue` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'Get时的查询内容',
  `requestHeaderId` bigint(0) UNSIGNED NULL DEFAULT NULL COMMENT '请求的头部',
  `requestBodyId` bigint(0) UNSIGNED NULL DEFAULT NULL COMMENT '请求的body',
  `responseHeaderId` bigint(0) UNSIGNED NULL DEFAULT NULL COMMENT '响应的头部',
  `responseBodyId` bigint(0) UNSIGNED NULL DEFAULT NULL COMMENT '响应的尾部',
  `requestContent` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '完整的请求内容',
  `responseContent` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '完整的返回内容',
  `statusCode` int(0) NULL DEFAULT NULL COMMENT '返回状态码',
  `startTime` datetime(0) NULL DEFAULT NULL COMMENT '该过程的开始时间',
  `stopTime` datetime(0) NULL DEFAULT NULL COMMENT '该过程的结束时间',
  `method` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '请求方法',
  `spend` int(0) NULL DEFAULT NULL COMMENT '花费时间',
  `clientip` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'ip地址',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `id`(`id`) USING BTREE,
  INDEX `servieName`(`serviceName`) USING BTREE,
  INDEX `queryValue`(`queryValue`) USING BTREE,
  INDEX `statusCode`(`statusCode`) USING BTREE,
  INDEX `spend`(`spend`) USING BTREE,
  INDEX `time`(`startTime`, `stopTime`) USING BTREE,
  INDEX `method`(`method`) USING BTREE,
  INDEX `index_0`(`requestBodyId`, `responseBodyId`) USING BTREE,
  FULLTEXT INDEX `requestContent`(`requestContent`),
  FULLTEXT INDEX `responseContent`(`responseContent`)
) ENGINE = InnoDB AUTO_INCREMENT = 1516009 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
