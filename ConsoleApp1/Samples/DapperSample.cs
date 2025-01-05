using MySqlConnector;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Models;
using Dapper;

namespace ConsoleApp1.Samples
{
    internal class DapperSample(string connStr)
    {
        public async Task<UserModel?> GetDataAsync()
        {
            string query = @"
SELECT *
FROM user
";
            IEnumerable<UserModel> resultList = [];

            using (MySqlConnection conn = new(connStr))
            {
                conn.Open();

                resultList = await conn.QueryAsync<UserModel>(query);
            }

            UserModel? resultData = resultList.FirstOrDefault();

            Console.WriteLine($"RowCount - {resultList.Count()}");
            Console.WriteLine($"RowData - {resultData?.id} {resultData?.name}");

            return resultData;
        }
    }
}
