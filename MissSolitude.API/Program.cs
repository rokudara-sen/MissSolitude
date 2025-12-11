using Microsoft.EntityFrameworkCore;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.UseCases.User;
using MissSolitude.Infrastructure;
using MissSolitude.Infrastructure.Auth;
using MissSolitude.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------- MY CODE ------------------------------

builder.Services.Configure<TokenOptions>(builder.Configuration.GetSection("Tokens"));
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddInfrastructure();
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<ReadUserUseCase>();
builder.Services.AddScoped<UpdateUserUseCase>();
builder.Services.AddScoped<DeleteUserUseCase>();
builder.Services.AddScoped<LogInUserUseCase>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Ui", policy =>
        policy.WithOrigins("http://localhost:8081", "https://localhost:8081").AllowAnyHeader().AllowAnyMethod());
});

// ------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// --------------------- MY CODE ------------------------------

app.UseCors("Ui");
app.MapControllers();

// ------------------------------------------------------------

app.Run();

