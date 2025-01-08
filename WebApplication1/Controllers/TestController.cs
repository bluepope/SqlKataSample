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
            //EF �� ��� �����ϰ� �ܼ������� �������� ����
            //return _dbContext.User.Where(u => u.id == 1).ToList();

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

        [HttpGet("list")]
        public async Task<IEnumerable<UserModel>> GetUserListAsync()
        {
            //Ȯ��޼��带 �̿��� ����
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
                insertData[nameof(UserModel.name)] = "���׽�Ʈ";
            });
        }

        [HttpGet("update")]
        public async Task UpdateTestAsync()
        {
            await _dbContext.UpdateAsync<UserModel>(updateData =>
            {
                updateData[nameof(UserModel.name)] = "���׽�Ʈ22";
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
