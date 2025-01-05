// 전역으로 사용하기위해 Namespace 삭제
using Dapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using SqlKata;
using SqlKata.Compilers;

using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;

public static class DBContextExtention
{
    static MySqlCompiler _compiler = new MySqlCompiler();

    public async static Task<List<T>> GetListAsync<T>(this DbContext dbContext, Action<Query> queryAction)
    {
        Query query = new();
        queryAction.Invoke(query);

        if (query.Clauses?.Any(c => c.Component == "from") == false)
        {
            string? tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
            if (tableName != null)
                query.From(tableName);
        }
        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        return (await dbConn.QueryAsync<T>(sqlResult.Sql, sqlResult.NamedBindings, tran)).ToList();
    }
}

