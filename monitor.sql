/*
Navicat MySQL Data Transfer

Source Server         : local
Source Server Version : 50725
Source Host           : localhost:3306
Source Database       : monitor

Target Server Type    : MYSQL
Target Server Version : 50725
File Encoding         : 65001

Date: 2019-03-14 15:59:57
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
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for building
-- ----------------------------

DROP TABLE IF EXISTS `building`;
CREATE TABLE `building` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `aid` int(11) NOT NULL,
  `aname` varchar(18) NOT NULL,
  `name` varchar(18) NOT NULL,
  `address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for camera
-- ----------------------------

DROP TABLE IF EXISTS `device`;
use video;
CREATE TABLE `device` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `lid` int(11) NOT NULL,
  `lname` varchar(18) NOT NULL,
  `ip` varchar(32) NOT NULL,
  `port` int(11) NOT NULL DEFAULT '0',
  `username` varchar(18) NOT NULL,
  `password` varchar(18) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
use video;
INSERT INTO `device` VALUES ('1','131','1教131门口摄像头', '123.111.112.232', '554', 'admin', 'admin');
INSERT INTO `device` VALUES ('2','131','1教131内部摄像头1', '111.213.323.123', '32', 'admin', 'adsadasd');
INSERT INTO `device` values ('3','131','1教131内部摄像头2','192.168.1.100','8000','admin','lab12345678');

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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

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
-- Table structure for sadmin
-- ----------------------------
use video;
DROP TABLE IF EXISTS `sadmin`;
CREATE TABLE `sadmin` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(18) NOT NULL,
  `password` varchar(18) NOT NULL,
  `name` varchar(10) NOT NULL,
  `phone` varchar(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
