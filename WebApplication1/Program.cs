using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using WebApplication1.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//EF 로 DB 컨텍스트 추가
builder.Services.AddDbContext<TestDbContext>(option =>
{
    option.UseMySql(builder.Configuration.GetConnectionString("testdb"), new MySqlServerVersion("8.0"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
