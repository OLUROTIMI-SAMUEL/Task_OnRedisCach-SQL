using Microsoft.EntityFrameworkCore;
using Task_1.Db_Folder;
using Task_1.Models;
using Task_1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDistributedRedisCache(options =>

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "RedisSample_";
});


builder.Services.AddDbContext<CustomerInfo_DbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));

builder.Services.AddScoped<IRedisCaching, RedisCaching>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
