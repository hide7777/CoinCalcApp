/* Drop Tables */
DROP TABLE test_u1.SAMP_CUSTOMER_SQLSERVER;

/* Create Tables */
-- SAMP_CUSTOMER
CREATE TABLE test_u1.SAMP_CUSTOMER_SQLSERVER
(
	-- 顧客番号
	CUST_NO int NOT NULL,
	-- 残高
	CASH  numeric(5,3) NOT NULL,
	-- 氏名
	CUST_NAME varchar(30) NOT NULL,
	-- 電話番号
	CUST_TEL decimal(5,4),
	-- 入会日時
	JOIN_DATE date,
	-- 入会タイムスタンプ
	JOIN_TIMESTAMP datetime,

	PRIMARY KEY (CUST_NO, CASH )
);

insert into SAMP_CUSTOMER_SQLSERVER values (1000,1,'山田太郎','03-1268-1111',NULL,NULL);


