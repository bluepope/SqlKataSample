using ConsoleApp1.Dto;
using ConsoleApp1.Models;

using Dapper;
using MySqlConnector;

using SqlKata;
using SqlKata.Compilers;

using System.Text.Json;

namespace ConsoleApp1.Samples
{
    internal class SqlKataSample(string connectionString)
    {
        public async Task<UserModel?> GetDataAsync()
        {
            IEnumerable<UserModel> resultList = [];

            Query query = new();
            query.From("user");
            query.Where("id", 1);

            //query.From(typeof(UserModel).GetCustomAttribute<TableAttribute>().Name);
            //query.Where(nameof(UserModel.id), 1);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("SELECT Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                resultList = await conn.QueryAsync<UserModel>(sqlResult.Sql, sqlResult.NamedBindings);
            }

            UserModel? resultData = resultList.FirstOrDefault();

            Console.WriteLine($"RowCount - {resultList.Count()}");
            Console.WriteLine($"RowData - {resultData?.id} {resultData?.name}");

            return resultData;
        }

        public async Task<UserNickNameResponse?> JoinSelectAsync()
        {
            IEnumerable<UserNickNameResponse> resultList = [];

            Query query = new();
            query.Select("user.id");
            query.Select("user.name");
            query.Select("nickname.nickname");

            query.From("user");
            query.Join("nickname", j => j.On("user.id", "nickname.user_id"));

            query.Where("id", 1);
            
            //query.From(typeof(UserModel).GetCustomAttribute<TableAttribute>().Name);
            //query.Where(nameof(UserModel.id), 1);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("SELECT Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                resultList = await conn.QueryAsync<UserNickNameResponse>(sqlResult.Sql, sqlResult.NamedBindings);
            }

            UserNickNameResponse? resultData = resultList.FirstOrDefault();

            Console.WriteLine($"RowCount - {resultList.Count()}");
            Console.WriteLine($"RowData - {resultData?.Id} {resultData?.Name} {resultData?.NickName}"); //Dapper Mapping은 대소문자를 가리지 않음

            return resultData;
        }


        public async Task<UserNickNameResponse?> SubQuerySelectAsync()
        {
            IEnumerable<UserNickNameResponse> resultList = [];

            Query query = new();
            query.Select("id");
            query.Select("name");
            query.From("user");

            //서브쿼리 생성
            Query subQuery = new();
            subQuery.Select("user_id");
            subQuery.From("nickname");

            //서브쿼리 대입
            query.WhereIn("id", subQuery);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("SELECT Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                resultList = await conn.QueryAsync<UserNickNameResponse>(sqlResult.Sql, sqlResult.NamedBindings);
            }

            UserNickNameResponse? resultData = resultList.FirstOrDefault();

            Console.WriteLine($"RowCount - {resultList.Count()}");
            Console.WriteLine($"RowData - {resultData?.Id} {resultData?.Name} {resultData?.NickName}"); //Dapper Mapping은 대소문자를 가리지 않음

            return resultData;
        }


        public async Task InsertAsync(UserModel input)
        {
            Query query = new();
            query.AsInsert(input);
            query.From("user");
            
            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("INSERT Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                await conn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings);
            }
        }

        public async Task UpdateAsync(UserModel input)
        {
            Query query = new();
            query.AsUpdate(new[] { "name" }, new[] { input.name });
            query.From("user");
            query.Where("id", input.id);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("UPDATE Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                await conn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings);
            }
        }

        public async Task DeleteAsync(UserModel input)
        {
            Query query = new();
            query.AsDelete();
            query.From("user");
            query.Where("id", input.id);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            Console.WriteLine("DELETE Query");
            Console.WriteLine(sqlResult.Sql);
            Console.WriteLine(JsonSerializer.Serialize(sqlResult.NamedBindings));

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                await conn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings);
            }
        }
    }
}
