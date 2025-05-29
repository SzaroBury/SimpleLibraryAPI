using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Application.Services.Abstraction;
using SimpleLibrary.Application.Commands.Books;

namespace SimpleLibrary.Application.Services;

public class BookService: IBookService
{
    private readonly IUnitOfWork unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await unitOfWork.GetRepository<Book>().GetAllAsync();
    }
    public Task<Book> GetBookByIdAsync(string id)
    {
        var bookGuid = ValidateGuid(id);
        return GetBookByIdAsync(bookGuid);
    }
    public async Task<Book> GetBookByIdAsync(Guid id)
    {
        var result = await unitOfWork.GetRepository<Book>().GetByIdAsync(id) 
            ?? throw new KeyNotFoundException($"A book with the specified id ({id}) was not found in the system.");

        return result;
    }
    public async Task<Book> CreateBookAsync(PostBookCommand book)
    {
        var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(book.CategoryId)
            ?? throw new KeyNotFoundException($"A category with the specified ID ({book.CategoryId}) was not found in the system.");

        var author = await unitOfWork.GetRepository<Author>().GetByIdAsync(book.AuthorId)
            ?? throw new KeyNotFoundException($"An author with the specified ID ({book.AuthorId}) was not found in the system.");

        var tagsInString = book.Tags.ToList().Count > 1 
            ? string.Join(',', book.Tags.Select(t => t.ToLower())) 
            : book.Tags.First() ?? "";

        bool isThereSimilarBook = unitOfWork.GetRepository<Book>().GetQueryable()
            .Any(b => b.Title.ToLower() == book.Title.ToLower() 
                && b.AuthorId == author.Id
            );
        if(isThereSimilarBook)
        {
            throw new InvalidOperationException("There is already a similar book in the system.");
        }

        Book newBook = new()
        {
            Title = book.Title,
            Description = book.Description,
            ReleaseDate = book.ReleaseDate,
            Language = book.Language,
            Author = author,
            AuthorId = author.Id,
            Category = category,
            CategoryId = book.CategoryId,
            Tags = tagsInString,
        };

        await unitOfWork.GetRepository<Book>().AddAsync(newBook);
        return newBook;
    }
    public async Task<Book> UpdateBookAsync(PatchBookCommand book)
    {
        Book existingBook = await GetBookByIdAsync(book.Id);

        if(book.Title is not null)
        {
            if(string.IsNullOrEmpty(book.Title))
            {
                throw new ArgumentException("Title can not be empty.");
            }
            existingBook.Title = book.Title;
        }

        if(book.Description is not null)
        {
            existingBook.Description = book.Description;
        }

        if(book.ReleaseDate.HasValue)
        {
            existingBook.ReleaseDate = book.ReleaseDate.Value;
        }

        if(book.Language.HasValue)
        {
            existingBook.Language = book.Language.Value;
        }

        if(book.Tags is not null)
        {
            if(book.Tags.Any(tag => tag.Contains(',')))
            {
                throw new FormatException("Invalid tags format. Please do not use commas in tags.");
            }
            var tagsInString = book.Tags.ToList().Count > 1 
                ? string.Join(',', book.Tags) 
                : book.Tags.First() ?? "";
            existingBook.Tags = tagsInString;
        }

        if(book.AuthorId.HasValue)
        {
            var author = await unitOfWork.GetRepository<Author>().GetByIdAsync(book.AuthorId.Value)
                ?? throw new KeyNotFoundException($"An author with the specified ID ({book.AuthorId}) was not found in the system.");
                
            existingBook.Author = author;
            existingBook.AuthorId = author.Id;
        }

        if(book.CategoryId.HasValue)
        {
            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(book.CategoryId.Value)
                ?? throw new KeyNotFoundException($"A category with the specified ID ({book.CategoryId}) was not found in the system.");
                
            existingBook.Category = category;
            existingBook.CategoryId = category.Id;
        }

        bool isThereSimilarBook = unitOfWork.GetRepository<Book>().GetQueryable()
            .Any(b => b.Id != existingBook.Id 
                && b.Title.ToLower() == existingBook.Title.ToLower()
                && b.AuthorId == existingBook.AuthorId
            );

        if(isThereSimilarBook)
        {
            throw new InvalidOperationException("There is already a similar book in the system.");
        }

        unitOfWork.GetRepository<Book>().Update(existingBook);
        await unitOfWork.SaveChangesAsync();

        return existingBook;
    }
    public async Task DeleteBookAsync(string id)
    {     
        Book book = await GetBookByIdAsync(id);

        var copies = unitOfWork.GetRepository<Copy>().GetQueryable()
            .Where(c => c.BookId == book.Id)
            .Select(c => c.Id)
            .ToList();

        foreach(var c in copies)
        {
            bool areThereActiveBorrowings = unitOfWork.GetRepository<Borrowing>()
                .GetQueryable()
                .Any(bor => bor.CopyId == c 
                    && !bor.ActualReturnDate.HasValue
                );
            if(areThereActiveBorrowings)
            {
                throw new InvalidOperationException("The book can not be deleted. There are still active borrowings in the system.");
            }
            await unitOfWork.GetRepository<Copy>().DeleteAsync(c);
        }

        await unitOfWork.GetRepository<Book>().DeleteAsync(book.Id);
    }

public async Task<IEnumerable<Book>> SearchBooksAsync(string? searchTerm = null, 
                                bool? isAvailable = null, 
                                string? olderThan = null, 
                                string? newerThan = null, 
                                string? authorId = null, 
                                string? categoryId = null,
                                int page = 1,
                                int pageSize = 25
    )
    {
        if(page <= 0 || pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException("Page and size of a page must be a positive number.");
        }

        var searchBooksQuery = unitOfWork.GetRepository<Book>().GetQueryable();
        // var searchBooksResult = searchBooksQuery.ToList();
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchBooksQuery = searchBooksQuery.Where(b =>
                b.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Tags.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Language.ToString().ToLower().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Author.FirstName.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Author.LastName.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Author.Description.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Author.Tags.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Category!.Name.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Category.Description.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
                || b.Category.Tags.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
            );
        }


        if (isAvailable.HasValue)
        {
            var copies = unitOfWork.GetRepository<Copy>().GetAllAsync().GetAwaiter().GetResult().ToList(); //??
            var borrowings = unitOfWork.GetRepository<Borrowing>().GetAllAsync().GetAwaiter().GetResult().ToList(); //??
            if (isAvailable.Value)
            {
                searchBooksQuery = searchBooksQuery.Where(
                    b => copies.Exists(
                        c => c.BookId == b.Id && !borrowings.Exists(
                            br => br.CopyId == c.Id && br.ActualReturnDate == null
                        )
                    )
                );  
            }
            else
            {
                searchBooksQuery = searchBooksQuery.Where(
                    b => !copies.Exists(
                        c => b.Id == c.BookId && !borrowings.Exists(
                            br => c.Id == br.CopyId && br.ActualReturnDate == null
                        )
                    )
                );
            }
        }

        if (!string.IsNullOrEmpty(olderThan))
        {
            if (!DateTime.TryParse(olderThan, out DateTime olderThanDate))
            {
                throw new FormatException("Invalid date format of olderThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchBooksQuery = searchBooksQuery.Where(b => b.ReleaseDate <= olderThanDate);
        }

        if (!string.IsNullOrEmpty(newerThan))
        {
            if(!DateTime.TryParse(newerThan, out DateTime newerThanDate))
            {
                throw new FormatException("Invalid date format of newerThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchBooksQuery = searchBooksQuery.Where(b => b.ReleaseDate >= newerThanDate);
        }

        if (!string.IsNullOrEmpty(authorId))
        {
            var authorGuid = ValidateGuid(authorId, "author");
            var author = await unitOfWork.GetRepository<Author>().GetByIdAsync(authorGuid)
                ?? throw new KeyNotFoundException($"An author with the specified ID ({authorId}) was not found in the system.");

            searchBooksQuery = searchBooksQuery.Where(b => b.AuthorId == authorGuid);
        }

        if (!string.IsNullOrEmpty(categoryId))
        {
            var categoryGuid = ValidateGuid(categoryId, "category");
            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(categoryGuid)
                ?? throw new KeyNotFoundException($"A category with the specified id ({categoryId}) was not found in the system.");

            searchBooksQuery = searchBooksQuery.Where(b => b.CategoryId == categoryGuid);
        }

        var count = searchBooksQuery.Count();
        if (count > 0 
            && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException();
        }

        searchBooksQuery = searchBooksQuery.Skip((page - 1) * pageSize);
        searchBooksQuery = count > pageSize ? searchBooksQuery.Take(pageSize) : searchBooksQuery;

        return searchBooksQuery.ToList();
    }

    private static Guid ValidateGuid(string id, string entity = "book")
    {
        if(!Guid.TryParse(id, out var guid))
        {
            throw new FormatException($"Invalid {entity} ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return guid;
    }
}