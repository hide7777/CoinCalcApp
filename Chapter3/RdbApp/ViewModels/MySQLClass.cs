using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace RdbApp.ViewModels
{
    public class MySQLClass
    {
        public MySQLClass()
        {
        }

        public void select()
        {
            try
            {
                DataTable dt = new DataTable();
                string connString = "Server=192.168.X.X;Port=3306;database=test;User ID=xxxx;Password=yyyy;charset=utf8";
                using (var con = new MySqlConnection(connString))
                {
                    using (MySqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "SELECT * FROM SAMP_CUSTOMER_MYSQL";
                        dt.Load(cmd.ExecuteReader());
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        Console.WriteLine("MySQL:" + dt.Columns[0].ColumnName + "：" + row[0] + "," + dt.Columns[2].ColumnName + ":" + row[2]);
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
