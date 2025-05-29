using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Application.Common;
using SimpleLibrary.Infrastructure;
using SimpleLibrary.Infrastructure.Repositories;
using SimpleLibrary.Infrastructure.Services;
using SimpleLibrary.API.Middleware;
using SimpleLibrary.API.Validators;

using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

Console.WriteLine($"Environment: {environment}");

// Adding services to the container.
builder.Services.AddDbContext<LibraryEFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddScoped<IReaderService, ReaderService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddValidatorsFromAssemblyContaining<AddUserRequestValidator>();

var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var accessTokenSecret = jwtSettings?.AccessTokenSecret ?? throw new Exception("AccessTokenSecret is mssing. It is required to run.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenSecret))
    };

    // Pobieranie tokenu z ciasteczka
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AccessToken"))
            {
                context.Token = context.Request.Cookies["AccessToken"];
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Language)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Automatyczne uruchamianie migracji
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync();
}


// Configuring the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
