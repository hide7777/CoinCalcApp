�������
�{�T���v���͕K���uRed Hat JBoss Enterprise Application Platform 7.4�v�œ������ĉ������BJBOSS EAP 8.0�ȍ~�ł͓����܂���B
�C���X�g�[�����@�́A�ȉ����Q�Ƃ��ĉ������B

https://docs.redhat.com/ja/documentation/red_hat_jboss_enterprise_application_platform/6.4/html/getting_started_guide/sect-download_and_install_jboss_eap_using_the_zip

��ORACLE�̃o�[�W����
ORACLE��19c�ȏ�𐄏����܂��B

��JUnit�p�̐ݒ�
application-test-context.xml�ɐڑ�����ORACLE�̐ڑ����ݒ�����Ă��������B

��JBOSS��JNDI��ݒ�
JBOSS�œ������ꍇ�́A�ȉ��̂悤��ORACLE�p��JNDI�ݒ肪�K�v�ł��B

�@ORACLE��JDBC�h���C�o�iojdbc8.jar�j���_�E�����[�h���A/tmp�ɔz�u�����Ƃ��܂��B
�Ajboss-cli.sh -c --controller=localhost:9990 �����s���AJBOSS CLI�ɐڑ����܂��B
�BJBOSS CLI�ňȉ������s���AJDBC�h���C�o��z�u���܂��B
�@module add --name=com.oracle --resources=/tmp/ojdbc8.jar --dependencies=javax.api,javax.transaction.api
�@��L�����s����ƁA�h���C�o�[�͎��̃f�B���N�g���ɔz�u����܂��B
  $JBOSS_HOME/modules/com/oracle/main
�CJBOSS CLI�ňȉ������s���AJDBC�h���C�o�[��錾���܂��B
�@/subsystem=datasources/jdbc-driver=oracle:add(driver-name=oracle,driver-module-name=com.oracle,driver-xa-datasource-class-name=oracle.jdbc.xa.client.OracleXADataSource)

�DJBOSS CLI�ňȉ������s���A�f�[�^�\�[�X���쐬���܂��B
�@data-source add --name=OracleDS --jndi-name=java:jboss/datasources/OracleDS --driver-name=oracle --connection-url=jdbc:oracle:thin:@192.168.1.21:1521/FREEPDB1 --user-name=test_u1 --password=test_u1 --jta=true --use-ccm=true --use-java-context=true --enabled=true --max-pool-size=10 --min-pool-size=5 --flush-strategy="FailingConnectionOnly"

