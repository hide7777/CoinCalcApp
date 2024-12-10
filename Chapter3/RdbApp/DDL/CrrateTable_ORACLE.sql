/* Drop User */
drop user test_u1;

/* Create USer */
create user test_u1 identified by test_u1
	default tablespace users
	temporary tablespace temp
	quota unlimited on users
;
grant connect, resource, dba to test_u1;

/* Drop Tables */
DROP TABLE test_u1.SAMP_CUSTOMER_ORACLE;

/* Create Tables */
-- SAMP_CUSTOMER
CREATE TABLE test_u1.SAMP_CUSTOMER_ORACLE
(
	-- 顧客番号
	CUST_NO int NOT NULL,
	-- 残高
	CASH  DECIMAL(5,3) NOT NULL,
	-- 氏名
	CUST_NAME varchar(30) NOT NULL,
	-- 電話番号
	CUST_TEL varchar(20) NOT NULL,
	-- 入会日時
	JOIN_DATE date,
	-- 入会タイムスタンプ
	JOIN_TIMESTAMP timestamp,
	-- 
	CONSTRAINT PK_SAMP_CUSTOMER PRIMARY KEY (CUST_NO, CASH )
);

COMMENT ON TABLE  test_u1.SAMP_CUSTOMER_ORACLE                IS 'サンプル顧客テーブル';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_NO        IS '顧客番号';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CASH           IS '残高';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_NAME      IS '氏名';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_TEL       IS '電話番号';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.JOIN_DATE      IS '入会日時';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.JOIN_TIMESTAMP IS '入会タイムスタンプ';

insert into SAMP_CUSTOMER_ORACLE values (1000,1,'山田太郎','03-1268-1111',NULL,NULL);


