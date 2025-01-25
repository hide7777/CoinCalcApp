using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RdbApp.ViewModels
{
    public class SQLServerClass
    {
        public SQLServerClass()
        {
        }

        public void select()
        {
            try
            {
                DataTable dt = new DataTable();
                string connString = "Data Source=192.168.1.84,1433;Initial Catalog=master;User ID=sa;Password=moritaka;Integrated Security=false;Encrypt=false";
                using (var con = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        //※SQLServerは、データベース接続時にカレントスキーマを指定できないため、テーブル名の前にスキーマ名を付与する
                        cmd.CommandText = "SELECT * FROM test_u1.SAMP_CUSTOMER_SQLSERVER";
                        dt.Load(cmd.ExecuteReader());
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        Console.WriteLine("SQLServer:" + dt.Columns[0].ColumnName + "：" + row[0] + "," + dt.Columns[2].ColumnName + ":" + row[2]);
                    }
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
    }
}
