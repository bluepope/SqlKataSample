using MySqlConnector;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Samples
{
    public class DataTableSample(string connStr)
    {
        public DataTable GetData()
        {
            string query = @"
SELECT *
FROM user
";

            DataSet ds = new();
            using (MySqlConnection conn = new(connStr))
            {
                conn.Open();

                using MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;

                DataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(ds);
            }

            Console.WriteLine($"RowCount - {ds.Tables[0].Rows.Count}");
            Console.WriteLine($"RowData - {ds.Tables[0].Rows[0]["ID"]} {ds.Tables[0].Rows[0]["Name"]}");

            return ds.Tables[0];
        }
    }
}
