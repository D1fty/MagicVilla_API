using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// SQL
const string DefaultSQSLConnection = "DefaultSQLConnection";
string conStr = builder.Configuration.GetConnectionString(DefaultSQSLConnection) 
    ?? throw new Exception("Default SQL Connection String is not defined");

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(conStr);
});

// DI
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

// Mapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// Controllers
builder.Services
    .AddControllers(
        option => option.ReturnHttpNotAcceptable = true
    )
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen();

// Build
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
