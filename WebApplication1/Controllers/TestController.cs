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
        [HttpGet("raw")]
        public async Task<IEnumerable<UserModel>> GetUserListRawAsync()
        {
            //EF 의 경우 강력하고 단순하지만 유연하지 못함
            //return _dbContext.User.Where(u => u.id == 1).ToList();

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

        [HttpGet("list")]
        public async Task<IEnumerable<UserModel>> GetUserListAsync()
        {
            //확장메서드를 이용한 단축
            return await _dbContext.GetListAsync<UserModel>(query =>
            {
                query.Where(nameof(UserModel.id), 1);
            });
        }

        [HttpGet("insert")]
        public async Task InsertTestAsync()
        {
            await _dbContext.InsertAsync<UserModel>(insertData =>
            {
                insertData[nameof(UserModel.id)] = 2;
                insertData[nameof(UserModel.name)] = "웹테스트";
            });
        }

        [HttpGet("update")]
        public async Task UpdateTestAsync()
        {
            await _dbContext.UpdateAsync<UserModel>(updateData =>
            {
                updateData[nameof(UserModel.name)] = "웹테스트22";
            },
            query =>
            {
                query.Where(nameof(UserModel.id), 2);
            });
        }

        [HttpGet("delete")]
        public async Task DeleteTestAsync()
        {
            await _dbContext.DeleteAsync<UserModel>(query =>
            {
                query.Where(nameof(UserModel.id), 2);
            });
        }
    }
}
