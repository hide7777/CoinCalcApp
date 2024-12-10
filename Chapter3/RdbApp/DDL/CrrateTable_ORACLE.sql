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
	-- �ڋq�ԍ�
	CUST_NO int NOT NULL,
	-- �c��
	CASH  DECIMAL(5,3) NOT NULL,
	-- ����
	CUST_NAME varchar(30) NOT NULL,
	-- �d�b�ԍ�
	CUST_TEL varchar(20) NOT NULL,
	-- �������
	JOIN_DATE date,
	-- ����^�C���X�^���v
	JOIN_TIMESTAMP timestamp,
	-- 
	CONSTRAINT PK_SAMP_CUSTOMER PRIMARY KEY (CUST_NO, CASH )
);

COMMENT ON TABLE  test_u1.SAMP_CUSTOMER_ORACLE                IS '�T���v���ڋq�e�[�u��';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_NO        IS '�ڋq�ԍ�';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CASH           IS '�c��';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_NAME      IS '����';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.CUST_TEL       IS '�d�b�ԍ�';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.JOIN_DATE      IS '�������';
COMMENT ON COLUMN test_u1.SAMP_CUSTOMER_ORACLE.JOIN_TIMESTAMP IS '����^�C���X�^���v';

insert into SAMP_CUSTOMER_ORACLE values (1000,1,'�R�c���Y','03-1268-1111',NULL,NULL);


