/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50725
Source Host           : localhost:3306
Source Database       : monitor

Target Server Type    : MYSQL
Target Server Version : 50725
File Encoding         : 65001

Date: 2019-03-21 15:14:34
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for admin
-- ----------------------------
DROP TABLE IF EXISTS `admin`;
CREATE TABLE `admin` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(18) NOT NULL,
  `password` varchar(18) NOT NULL,
  `name` varchar(10) NOT NULL,
  `phone` varchar(11) NOT NULL,
  `bid` int(11) DEFAULT NULL,
  `bname` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of admin
-- ----------------------------
INSERT INTO `admin` VALUES ('1', 'admin', 'admin', '郭峰', '18758372833', '1', '第一教学楼');
INSERT INTO `admin` VALUES ('2', 'muamua', 'muamua', '冯静恩', '19348573855', '2', '第三教学楼');
INSERT INTO `admin` VALUES ('3', 'shishi', 'shishi', ' 李凤娟', '13123840588', '3', '第十一教学楼');

-- ----------------------------
-- Table structure for building
-- ----------------------------
DROP TABLE IF EXISTS `building`;
CREATE TABLE `building` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(18) NOT NULL,
  `address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of building
-- ----------------------------
INSERT INTO `building` VALUES ('1', '第一教学楼', '1教');
INSERT INTO `building` VALUES ('3', '第三教学楼', '3教');
INSERT INTO `building` VALUES ('10', '第十教学楼', '10教');
INSERT INTO `building` VALUES ('11', '第十一教学楼', '11教');

-- ----------------------------
-- Table structure for camera
-- ----------------------------
DROP TABLE IF EXISTS `camera`;
CREATE TABLE `camera` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `lid` int(11) NOT NULL,
  `lname` varchar(18) NOT NULL,
  `ip` varchar(32) NOT NULL,
  `port` int(11) NOT NULL DEFAULT '0',
  `address` varchar(255) NOT NULL,
  `username` varchar(18) NOT NULL,
  `password` varchar(18) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of camera
-- ----------------------------
INSERT INTO `camera` VALUES ('1', '1教131门口摄像头', '1', '软件测试实验室', '123.111.112.232', '554', '1教131', 'admin', 'admin');
INSERT INTO `camera` VALUES ('2', '1教131内部摄像头1', '1', '软件测试实验室', '111.213.323.123', '32', '1教131', 'admin', 'adsadasd');
INSERT INTO `camera` VALUES ('3', '1教131内部摄像头2', '1', '软件测试实验室', '192.168.1.100', '8000', '1教131', 'admin', 'lab12345678');

-- ----------------------------
-- Table structure for lab
-- ----------------------------
DROP TABLE IF EXISTS `lab`;
CREATE TABLE `lab` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bid` int(11) NOT NULL,
  `bname` varchar(18) NOT NULL,
  `name` varchar(18) NOT NULL,
  `address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of lab
-- ----------------------------
INSERT INTO `lab` VALUES ('1', '1', '第一教学楼', '软件测试实验室', '1教131');
INSERT INTO `lab` VALUES ('2', '1', '第一教学楼', '硬件测试实验室', '1教225');
INSERT INTO `lab` VALUES ('3', '1', '第一教学楼', '软件应用实验室', '1教233');
INSERT INTO `lab` VALUES ('4', '1', '第一教学楼', '硬件实验室', '1教115');
INSERT INTO `lab` VALUES ('5', '2', '第三教学楼', '苹果实验室', '3教317');
INSERT INTO `lab` VALUES ('6', '3', '第十一教学楼', '创新实验室', '11教124');
INSERT INTO `lab` VALUES ('7', '1', '第一教学楼', '应用实验室', '1-235');

-- ----------------------------
-- Table structure for record
-- ----------------------------
DROP TABLE IF EXISTS `record`;
CREATE TABLE `record` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cid` int(11) NOT NULL,
  `cname` varchar(18) NOT NULL,
  `date` date NOT NULL,
  `path` varchar(255) NOT NULL,
  `filename` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of record
-- ----------------------------

-- ----------------------------
-- Table structure for sadmin
-- ----------------------------
DROP TABLE IF EXISTS `sadmin`;
CREATE TABLE `sadmin` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(18) NOT NULL,
  `password` varchar(18) NOT NULL,
  `name` varchar(10) NOT NULL,
  `phone` varchar(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of sadmin
-- ----------------------------
INSERT INTO `sadmin` VALUES ('1', 'sadmin', 'sadmin', '超级管理员', '18768214388');
INSERT INTO `sadmin` VALUES ('2', 'chaochao', 'chaochao', '莫大超', '17422394888');
