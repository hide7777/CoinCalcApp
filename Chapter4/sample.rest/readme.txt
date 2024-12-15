■動作環境
本サンプルは必ず「Red Hat JBoss Enterprise Application Platform 7.4」で動かして下さい。JBOSS EAP 8.0以降では動きません。
インストール方法は、以下を参照して下さい。

https://docs.redhat.com/ja/documentation/red_hat_jboss_enterprise_application_platform/6.4/html/getting_started_guide/sect-download_and_install_jboss_eap_using_the_zip

■ORACLEのバージョン
ORACLEは19c以上を推奨します。

■JUnit用の設定
application-test-context.xmlに接続するORACLEの接続先を設定をしてください。

■JBOSSにJNDIを設定
JBOSSで動かす場合は、以下のようなORACLE用のJNDI設定が必要です。

①ORACLEのJDBCドライバ（ojdbc8.jar）をダウンロードし、/tmpに配置したとします。
②jboss-cli.sh -c --controller=localhost:9990 を実行し、JBOSS CLIに接続します。
③JBOSS CLIで以下を実行し、JDBCドライバを配置します。
　module add --name=com.oracle --resources=/tmp/ojdbc8.jar --dependencies=javax.api,javax.transaction.api
　上記を実行すると、ドライバーは次のディレクトリに配置されます。
  $JBOSS_HOME/modules/com/oracle/main
④JBOSS CLIで以下を実行し、JDBCドライバーを宣言します。
　/subsystem=datasources/jdbc-driver=oracle:add(driver-name=oracle,driver-module-name=com.oracle,driver-xa-datasource-class-name=oracle.jdbc.xa.client.OracleXADataSource)

⑤JBOSS CLIで以下を実行し、データソースを作成します。
　data-source add --name=OracleDS --jndi-name=java:jboss/datasources/OracleDS --driver-name=oracle --connection-url=jdbc:oracle:thin:@192.168.1.21:1521/FREEPDB1 --user-name=test_u1 --password=test_u1 --jta=true --use-ccm=true --use-java-context=true --enabled=true --max-pool-size=10 --min-pool-size=5 --flush-strategy="FailingConnectionOnly"

