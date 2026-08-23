using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using YS.Knife.Query.Demo;
using YS.Knife.Query.Demo.Impl;
using YS.Knife.Query.Demo.Models;
using YS.Knife.Query.Functions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddDbContext<EFContext>((op) =>
{
    op.UseSqlite("Data Source=demo.db").EnableSensitiveDataLogging(true);
    op.LogTo(Console.WriteLine);
});


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

YS.Knife.Query.Functions.StaticFunctions.Add("random", () => EF.Functions.Random());
YS.Knife.Query.Functions.InstanceFunctions.Add<string, bool>("like", s => EF.Functions.Like(s, It.Arg<string>()));

var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
using (var context = scope.ServiceProvider.GetService<EFContext>())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    var datas = Enumerable.Range(1, 200).Select((i) => new Material
    {
        Id = Guid.NewGuid(),
        Name = $"食材{i}",
        CreatedAt = DateTime.Now,
        CreatedBy = "user001",
        Unit = (UnitType)Random.Shared.Next(0, 4),
        Price = Convert.ToDecimal(Random.Shared.Next(10, 100) / 100.0),
        Specs = Enumerable.Range(0, Random.Shared.Next(1, 3)).Select((j) => new MateialSpec
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            CreatedBy = "user001",
            FullName = $"食材{i}规格{j}",
            Factor = Random.Shared.Next(1, 10),
            Unit = (UnitType)Random.Shared.Next(0, 4)
        }).ToList()
    });
    context.Materials.AddRange(datas);
    context.SaveChanges();
}

app.Run();
