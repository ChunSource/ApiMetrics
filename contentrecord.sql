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

 Date: 10/11/2023 11:11:11
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for contentrecord
-- ----------------------------
DROP TABLE IF EXISTS `contentrecord`;
CREATE TABLE `contentrecord`  (
  `id` bigint(0) UNSIGNED NOT NULL AUTO_INCREMENT,
  `value` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'header或body的内容',
  `type` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'request或者body',
  `md5` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'md5',
  `sha1` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL COMMENT 'sha1',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `hash`(`md5`, `sha1`) USING BTREE COMMENT '应该不会发生两个值其两个hash一样的情况吧',
  INDEX `id`(`id`) USING BTREE,
  INDEX `type`(`type`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3356139 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;
