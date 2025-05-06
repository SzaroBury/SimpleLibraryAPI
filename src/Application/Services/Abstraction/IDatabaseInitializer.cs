namespace SimpleLibrary.Application.Services.Abstraction;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}