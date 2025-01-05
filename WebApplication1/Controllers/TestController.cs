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

            //���� DB Ŀ�ؼ� ��������
            var dbConn = _dbContext.Database.GetDbConnection();

            //���� ������ Ʈ����� ��������
            var tran = _dbContext.Database.CurrentTransaction?.GetDbTransaction();

            return await dbConn.QueryAsync<UserModel>(sqlResult.Sql, sqlResult.NamedBindings, tran);
        }

        [HttpGet("sqlkata2")]
        public async Task<IEnumerable<UserModel>> GetUserList3Async()
        {
            //Ȯ��޼��带 �̿��� ����
            return await _dbContext.GetListAsync<UserModel>(query =>
            {
                query.Where(nameof(UserModel.id), 1);
            });
        }
    }
}
