using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Services;
using SimpleLibrary.Infrastructure;
using SimpleLibrary.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Adding services to the container.
builder.Services.AddDbContext<LibraryEFContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppModel")));

//builder.Services.AddIdentity<User, Role>(options =>
//{
//    options.User.RequireUniqueEmail = true;
//}).AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowingService, BorrowingService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddScoped<IReaderService, ReaderService>();

builder.Services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Language)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuring the HTTP request pipeline.
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
