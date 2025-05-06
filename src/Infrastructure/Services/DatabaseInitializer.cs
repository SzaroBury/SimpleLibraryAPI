using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Infrastructure.Services;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly LibraryEFContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(LibraryEFContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Applying pending EF Core migrations...");
        await _context.Database.MigrateAsync();
        _logger.LogInformation("Migrations applied successfully.");
    }
}