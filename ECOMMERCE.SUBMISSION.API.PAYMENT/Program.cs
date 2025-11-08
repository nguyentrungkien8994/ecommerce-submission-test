using ECOMMERCE.SUBMISSION.EVENT.BUS.CORE;
using ECOMMERCE.SUBMISSION.EVENT.BUS.KAFKA;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.InjectBaseService();
builder.InjectDbContext();
builder.AddAuthentication();
builder.InjectKafa();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
