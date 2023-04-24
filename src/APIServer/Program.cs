using Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using RepositoryEF;
using RepositoryEF.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LibraryEFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppModel")));

//builder.Services.AddIdentity<User, Role>(options =>
//{
//    options.User.RequireUniqueEmail = true;
//}).AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddScoped<IAuthorRepository,   AuthorRepositoryEF>();
builder.Services.AddScoped<IBookRepository,     BookRepositoryEF>();
builder.Services.AddScoped<IBorrowingRepository,BorrowingRepositoryEF>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryEF>();
builder.Services.AddScoped<ICopyRepository,     CopyRepositoryEF>();
builder.Services.AddScoped<IReaderRepository,   ReaderRepositoryEF>();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Language)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
