/* Drop Tables */
DROP TABLE test_u1.SAMP_CUSTOMER_SQLSERVER;

/* Create Tables */
-- SAMP_CUSTOMER
CREATE TABLE test_u1.SAMP_CUSTOMER_SQLSERVER
(
	-- �ڋq�ԍ�
	CUST_NO int NOT NULL,
	-- �c��
	CASH  numeric(5,3) NOT NULL,
	-- ����
	CUST_NAME varchar(30) NOT NULL,
	-- �d�b�ԍ�
	CUST_TEL decimal(5,4),
	-- �������
	JOIN_DATE date,
	-- ����^�C���X�^���v
	JOIN_TIMESTAMP datetime,

	PRIMARY KEY (CUST_NO, CASH )
);

insert into SAMP_CUSTOMER_SQLSERVER values (1000,1,'�R�c���Y','03-1268-1111',NULL,NULL);


