/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50725
Source Host           : localhost:3306
Source Database       : video

Target Server Type    : MYSQL
Target Server Version : 50725
File Encoding         : 65001

Date: 2019-02-28 14:58:51
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for device
-- ----------------------------
DROP TABLE IF EXISTS `device`;
CREATE TABLE `device` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ip` varchar(19) NOT NULL,
  `port` varchar(255) NOT NULL DEFAULT '554',
  `user` varchar(255) NOT NULL,
  `pwd` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `idx` (`ip`,`port`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=100  DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of device
-- ----------------------------
INSERT INTO `device` VALUES ('1', '123.111.112.232', '554', 'admin', 'admin');
INSERT INTO `device` VALUES ('2', '111.213.323.123', '32', 'adasd', 'adsadasd');
use video;
INSERT INTO `device` values ('3','192.168.1.100','8000','admin','lab12345678');
