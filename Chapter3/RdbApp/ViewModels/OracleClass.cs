using System;
using System.Data;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;

namespace RdbApp.ViewModels
{
    public class OracleClass
    {
        public OracleClass()
        {
        }

        public void execute()
        {
            try
            {
                //TransactionScopeの利用
                using (TransactionScope ts = new TransactionScope())
                {
                    string connString = "Data Source=192.168.1.21:1521/FREEPDB1;User ID=kakei;Password=kakei";
                    string select_string = "select * from SAMP_CUSTOMER_ORACLE order by cust_no";
                    string insert_string = "insert into SAMP_CUSTOMER_ORACLE values (2000,10,'テスト','03-1268-1111',TO_DATE('2021/01/02','yyyy/mm/dd hh24:mi:ss'),TO_TIMESTAMP('2022/10/30 11:11:00')";
                    string update_string = "update SAMP_CUSTOMER_ORACLE set CUST_NAME='テスト一郎' Where CUST_NO=2000 AND CASH=10";
                    string delete_string = "delete SAMP_CUSTOMER_ORACLE where cust_no=2000 and cash=10";
                    using (OracleConnection con = new OracleConnection(connString))
                    {
                        con.Open();
                        execute(con, insert_string);
                        execute(con, update_string);
                        execute(con, delete_string);
                        select(con, select_string);
                    }

                    //トランザクション完了
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void select(OracleConnection con, string commandText)
        {
            DataTable dt = new DataTable();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = commandText;
                dt.Load(cmd.ExecuteReader());
            }
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine("Oracle:" + dt.Columns[0].ColumnName + "：" + row[0] + "," + dt.Columns[2].ColumnName + ":" + row[2]);
            }
        }

        public void execute(OracleConnection con, string commandText)
        {
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.ExecuteNonQuery();
            }
        }

    }
}
