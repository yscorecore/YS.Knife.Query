using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using YS.Knife.Query.Demo;
using YS.Knife.Query.Demo.Impl;
using YS.Knife.Query.Demo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddDbContext<EFContext>((op) =>
{
    op.UseSqlite("Data Source=demo.db");
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

var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
using (var context = scope.ServiceProvider.GetService<EFContext>())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    context.Materials.Add(new Material
    {
        Id = Guid.NewGuid(),
        Name = "食材1",
        CreatedAt = DateTime.Now
    });
    context.Materials.Add(new Material
    {
        Id = Guid.NewGuid(),
        Name = "食材2",
        CreatedAt = DateTime.Now
    });
    context.SaveChanges();
}

app.Run();
