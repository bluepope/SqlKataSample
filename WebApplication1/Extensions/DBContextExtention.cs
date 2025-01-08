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

    /// <summary>
    /// Query 객체에 From 이 없다면 T 에서 TableName 을 가져와 From 에 추가한다
    /// </summary>
    static void SetQueryFrom<T>(Query query)
    {
        if (query.Clauses?.Any(c => c.Component == "from") == false)
        {
            string? tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name;
            if (tableName != null)
                query.From(tableName);
        }
    }

    /// <summary>
    /// 리스트 가져오기
    /// </summary>
    public async static Task<List<T>> GetListAsync<T>(this DbContext dbContext, Action<Query> queryAction)
    {
        Query query = new();
        queryAction.Invoke(query);

        SetQueryFrom<T>(query);

        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        return (await dbConn.QueryAsync<T>(sqlResult.Sql, sqlResult.NamedBindings, tran)).ToList();
    }

    /// <summary>
    /// 단일 건 가져오기
    /// </summary>
    public async static Task<T?> GetAsync<T>(this DbContext dbContext, Action<Query> queryAction)
    {
        Query query = new();
        queryAction.Invoke(query);

        SetQueryFrom<T>(query);
        //강제 Limit 추가
        query.Limit(1);

        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        //QueryAsync를 하는 이유는 MySql 에서 QueryFirstOrDefault 가 비효율적으로 동작하기때문
        return (await dbConn.QueryAsync<T>(sqlResult.Sql, sqlResult.NamedBindings, tran)).FirstOrDefault();
    }

    /// <summary>
    /// Insert 쿼리
    /// </summary>
    public async static Task<int> InsertAsync<T>(this DbContext dbContext, Action<Dictionary<string, object?>> insertData)
    {
        Query query = new();

        Dictionary<string, object?> insertDataDic = new();
        insertData.Invoke(insertDataDic);

        //Insert 임을 명시
        query.AsInsert(insertDataDic);
        SetQueryFrom<T>(query);

        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        return await dbConn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings, tran);
    }

    /// <summary>
    /// Update 쿼리
    /// </summary>
    public async static Task<int> UpdateAsync<T>(this DbContext dbContext, Action<Dictionary<string, object?>> updateData, Action<Query> queryAction)
    {
        Query query = new();
        queryAction.Invoke(query);

        Dictionary<string, object?> updateDataDic = new();
        updateData.Invoke(updateDataDic);

        //Update 임을 명시
        query.AsUpdate(updateDataDic);
        SetQueryFrom<T>(query);

        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        return await dbConn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings, tran);
    }

    /// <summary>
    /// Delete 쿼리
    /// </summary>
    public async static Task<int> DeleteAsync<T>(this DbContext dbContext, Action<Query> queryAction)
    {
        Query query = new();
        queryAction.Invoke(query);

        //Delete 임을 명시
        query.AsDelete();
        SetQueryFrom<T>(query);

        SqlResult sqlResult = _compiler.Compile(query);

        //_logger.LogInformation(sqlResult.Sql);
        //_logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

        //현재 DB 커넥션 가져오기
        var dbConn = dbContext.Database.GetDbConnection();

        //현재 설정된 트랜잭션 가져오기
        var tran = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        return await dbConn.ExecuteAsync(sqlResult.Sql, sqlResult.NamedBindings, tran);
    }
}

