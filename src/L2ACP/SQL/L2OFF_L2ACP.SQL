use lin2db
go

ALTER  TABLE user_auth add donatepoints int NOT NULL DEFAULT 0;

use lin2world
go

ALTER  TABLE npc_boss add level int NOT NULL DEFAULT 1;

-- ----------------------------
-- Table structure for l2acp_donateitems
-- ----------------------------
DROP TABLE IF EXISTS l2acp_donateitems;
CREATE TABLE l2acp_donateitems (
  itemId int NOT NULL,
  itemCount int NOT NULL DEFAULT '1',
  enchant int NOT NULL DEFAULT '0',
  price int NOT NULL DEFAULT '0'
)

-- ----------------------------
-- Records of l2acp_donateitems
-- ----------------------------
INSERT INTO l2acp_donateitems VALUES ('57', '100000', '0', '1');
INSERT INTO l2acp_donateitems VALUES ('6608', '1', '16', '1');
INSERT INTO l2acp_donateitems VALUES ('6598', '1', '16', '5');
INSERT INTO l2acp_donateitems VALUES ('6373', '1', '16', '5');
INSERT INTO l2acp_donateitems VALUES ('6379', '1', '0', '5');
INSERT INTO l2acp_donateitems VALUES ('5779', '1', '16', '5');
INSERT INTO l2acp_donateitems VALUES ('5767', '1', '16', '5');
INSERT INTO l2acp_donateitems VALUES ('512', '1', '16', '5');
INSERT INTO l2acp_donateitems VALUES ('2407', '1', '16', '5');

-- ----------------------------
-- Table structure for l2acp_donateservices
-- ----------------------------
DROP TABLE IF EXISTS l2acp_donateservices
CREATE TABLE l2acp_donateservices (
  serviceid int NOT NULL,
  servicename varchar(30) NOT NULL,
  price int NOT NULL DEFAULT '0',
  PRIMARY KEY (serviceid)
);

-- ----------------------------
-- Records of l2acp_donateservices
-- ----------------------------
INSERT INTO l2acp_donateservices VALUES ('1', 'Change player''s name', '1');
INSERT INTO l2acp_donateservices VALUES ('2', 'Set player nobless', '1');
INSERT INTO l2acp_donateservices VALUES ('3', 'Reset player''s PK', '1');
INSERT INTO l2acp_donateservices VALUES ('4', 'Change player''s sex', '1');

-- ----------------------------
-- Table structure for l2acp_donations
-- ----------------------------
DROP TABLE IF EXISTS l2acp_donations;
CREATE TABLE l2acp_donations (
  id int NOT NULL IDENTITY(1, 1),
  accountName varchar(20) NOT NULL,
  amount int DEFAULT NULL,
  transactionid varchar(60) DEFAULT NULL,
  verificationSign varchar(60) DEFAULT NULL,
  PRIMARY KEY (id)
);

-- ----------------------------
-- Table structure for l2acp_luckywheelitems
-- ----------------------------
DROP TABLE IF EXISTS l2acp_luckywheelitems;
CREATE TABLE l2acp_luckywheelitems (
  itemid int NOT NULL,
  itemcount int DEFAULT '1',
  enchant int DEFAULT '0',
  chance float DEFAULT '0'
);

-- ----------------------------
-- Records of l2acp_luckywheelitems
-- ----------------------------
INSERT INTO l2acp_luckywheelitems VALUES ('6375', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6376', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6377', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6378', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6379', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6380', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6381', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6382', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6383', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('6384', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('7575', '1', '0', '8.33');
INSERT INTO l2acp_luckywheelitems VALUES ('7576', '1', '0', '8.33');

-- ----------------------------
-- Table structure for l2acp_onlineanalytics
-- ----------------------------
DROP TABLE IF EXISTS l2acp_onlineanalytics;
CREATE TABLE l2acp_onlineanalytics (
  playercount int NOT NULL,
  recordedtime bigint NOT NULL
);