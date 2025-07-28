using FluentValidation;
using FluentValidation.AspNetCore;
using KonsolcumApi.API.Configurations.ColumnWriters;
using KonsolcumApi.API.Extensions;
using KonsolcumApi.API.Filters;
using KonsolcumApi.Application;
using KonsolcumApi.Application.Validators.Categories;
using KonsolcumApi.Application.Validators.Products;
using KonsolcumApi.Infrastructure;
using KonsolcumApi.Infrastructure.Filters;
using KonsolcumApi.Persistence.Contexts.EfContext;
using KonsolcumApi.Persistence.Services;
using KonsolcumApi.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region
//servisler
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddSignalRServices();
#endregion

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

#region
//serilog
SqlColumn sqlColumn = new SqlColumn();
sqlColumn.ColumnName = "UserName";
sqlColumn.DataType = System.Data.SqlDbType.NVarChar;
sqlColumn.PropertyName = "UserName";
sqlColumn.DataLength = 50;
sqlColumn.AllowNull = true;        
ColumnOptions columnOpt = new ColumnOptions();
columnOpt.Store.Remove(StandardColumn.Properties);
columnOpt.Store.Add(StandardColumn.LogEvent);
columnOpt.AdditionalColumns = new Collection<SqlColumn> { sqlColumn };

Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.MSSqlServer(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
     sinkOptions: new MSSqlServerSinkOptions
     {
         AutoCreateSqlTable = true,
         TableName = "logs",
     },
     appConfiguration: null,
     columnOptions: columnOpt
    )
    .Enrich.FromLogContext()
    .Enrich.With<CustomUserNameColumn>()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Host.UseSerilog(log);

#endregion

#region
//valid
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryCommandRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductCommandRequestValidator>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>();
})
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);
#endregion
builder.Services.AddHttpContextAccessor(); // Clienttan gelen request neticesinde oluþturulan httpcontext nesnesine katmanlardaki
//classlar üzerinden eriþebilmemizi saðlayan bir servistir.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin", options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateAudience = true, // Token’ýn hedef aldýðý uygulamayý (yani kimin için üretildiðini) kontrol eder
        ValidateIssuer = true, // Token’ýn kim tarafýndan üretildiðini kontrol eder.
        ValidateLifetime = true, // Token’ýn kim tarafýndan üretildiðini kontrol eder.
        ValidateIssuerSigningKey = true, // Token’ýn dijital imzasýnýn doðruluðunu kontrol eder.

        ValidAudience = builder.Configuration["Token:Audience"],
        ValidIssuer = builder.Configuration["Token:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,
    
        NameClaimType = ClaimTypes.Name,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseRouting();
app.UseCors();
app.UseStaticFiles(); //wwwroot dosya için
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (EfDbContext, next) =>
{
    var username = EfDbContext.User?.Identity?.IsAuthenticated != null || true ? EfDbContext.User.Identity.Name : null;
    LogContext.PushProperty("UserName", username);
    await next();
});

app.MapControllers();
app.MapHubs();

app.Run();
