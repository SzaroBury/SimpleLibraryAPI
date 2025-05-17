namespace SimpleLibrary.API.Requests.Authors;

public record PostAuthorRequest
(
    string FirstName, 
    string LastName,
    string? Description, 
    string? BornDate, 
    IEnumerable<string>? Tags
);