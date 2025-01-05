using Dapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using MySqlConnector;

using SqlKata;
using SqlKata.Compilers;

using System.Text.Json;

using WebApplication1.Repository;
using WebApplication1.Repository.Entities;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> _logger, TestDbContext _dbContext) : ControllerBase
    {
        [HttpGet("ef")]
        public async Task<IEnumerable<UserModel>> GetUserList1Async()
        {
            return _dbContext.User.Where(u => u.id == 1).ToList();
        }

        [HttpGet("sqlkata1")]
        public async Task<IEnumerable<UserModel>> GetUserList2Async()
        {
            Query query = new();
            query.From("user");
            query.Where("id", 1);

            SqlResult sqlResult = new MySqlCompiler().Compile(query);

            _logger.LogInformation(sqlResult.Sql);
            _logger.LogInformation(JsonSerializer.Serialize(sqlResult.NamedBindings));

            //현재 DB 커넥션 가져오기
            var dbConn = _dbContext.Database.GetDbConnection();

            //현재 설정된 트랜잭션 가져오기
            var tran = _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            return await dbConn.QueryAsync<UserModel>(sqlResult.Sql, sqlResult.NamedBindings, tran);
        }

        [HttpGet("sqlkata2")]
        public async Task<IEnumerable<UserModel>> GetUserList3Async()
        {
            //확장메서드를 이용한 단축
            return await _dbContext.GetListAsync<UserModel>(query =>
            {
                query.Where(nameof(UserModel.id), 1);
            });
        }
    }
}
