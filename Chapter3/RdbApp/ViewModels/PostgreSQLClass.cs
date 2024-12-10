using System;
using System.Data;
using Npgsql;

namespace RdbApp.ViewModels
{
    public class PostgreSQLClass
    {
        public PostgreSQLClass()
        {
        }

        public void select()
        {
            try
            {
                DataTable dt = new DataTable();
                string connString = "Server=192.168.X.XX;Port=5444;Database=edb;Username=xxxx;Password=yyyy;SearchPath=zzzz";
                using (var con = new NpgsqlConnection(connString))
                {
                    using (NpgsqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "SELECT * FROM samp_customer_postgresql";
                        dt.Load(cmd.ExecuteReader());
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        Console.WriteLine("PostgreSQL:" + dt.Columns[0].ColumnName + "：" + row[0] + "," + dt.Columns[2].ColumnName + ":" + row[2]);
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
