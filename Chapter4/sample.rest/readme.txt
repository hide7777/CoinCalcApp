��WAR�̓����
Eclipse�Ŗ{Prj���R���p�C�����Ă��������B
Maven�v���W�F�N�g�Ȃ̂ŁAMaven Install��war���쐬�ł��܂��B

WAR�𓮂����ꍇ�ɂ́A�K���uRed Hat JBoss Enterprise Application Platform 7.4�v�œ������ĉ������BJBOSS EAP 8.0�ȍ~�ł͓����܂���B
JBOSS EAP�̃C���X�g�[�����@�́A�ȉ����Q�Ƃ��ĉ������B

https://docs.redhat.com/ja/documentation/red_hat_jboss_enterprise_application_platform/7.0/html/installation_guide/index

��ORACLE�̃o�[�W����
�ڑ�����ORACLE��19c�ȏ�𐄏����܂��B

��JUnit�p�̐ݒ�
application-test-context.xml�ɐڑ�����ORACLE�̐ڑ����ݒ�����Ă��������B

��JBOSS��JNDI��ݒ�
JBOSS EAP��WAR�𓮂����ꍇ�́A�ȉ��̎菇��ORACLE�p��JNDI��ݒ肵�܂��B

�@ORACLE��JDBC�h���C�o�iojdbc8.jar�j���ȉ�����_�E�����[�h���ALinux��/tmp�ɔz�u�����Ƃ��܂��B
�@https://www.oracle.com/jp/database/technologies/appdev/jdbc-downloads.html
�Ajboss-cli.sh -c --controller=localhost:9990 �����s���AJBOSS CLI�ɐڑ����܂��B
�BJBOSS CLI�ňȉ������s���AJDBC�h���C�o��z�u���܂��B
�@module add --name=com.oracle --resources=/tmp/ojdbc8.jar --dependencies=javax.api,javax.transaction.api
�@��L�����s����ƁA�h���C�o�[�͎��̃f�B���N�g���ɔz�u����܂��B
  $JBOSS_HOME/modules/com/oracle/main
�CJBOSS CLI�ňȉ������s���AJDBC�h���C�o�[��錾���܂��B
�@/subsystem=datasources/jdbc-driver=oracle:add(driver-name=oracle,driver-module-name=com.oracle,driver-xa-datasource-class-name=oracle.jdbc.xa.client.OracleXADataSource)
�DJBOSS CLI�ňȉ������s���A�f�[�^�\�[�X���쐬���܂��B
�@data-source add --name=OracleDS --jndi-name=java:jboss/datasources/OracleDS --driver-name=oracle --connection-url=jdbc:oracle:thin:@192.168.1.21:1521/FREEPDB1 --user-name=test_u1 --password=test_u1 --jta=true --use-ccm=true --use-java-context=true --enabled=true --max-pool-size=10 --min-pool-size=5 --flush-strategy="FailingConnectionOnly"

