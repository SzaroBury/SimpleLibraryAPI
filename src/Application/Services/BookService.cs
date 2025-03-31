using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Domain.Repositories;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class BookService: IBookService
{
    private readonly IRepository<Book> bookRepository;
    private readonly IAuthorService authorService;
    private readonly IRepository<Copy> copyRepository;
    private readonly IRepository<Borrowing> borrowingRepository;
    private readonly IRepository<Category> categoryRepository;

    public BookService(IRepository<Book> bookRepository, 
                        IAuthorService authorService, 
                        IRepository<Copy> copyRepository, 
                        IRepository<Borrowing> borrowingRepository, 
                        IRepository<Category> categoryRepository)
    {
        this.bookRepository = bookRepository;
        this.authorService = authorService;
        this.copyRepository = copyRepository;
        this.borrowingRepository = borrowingRepository;
        this.categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await bookRepository.GetAllAsync();
    }
    public Task<Book> GetBookByIdAsync(string id)
    {
        var bookGuid = ValidateGuid(id);
        return GetBookByIdAsync(bookGuid);
    }
    public async Task<Book> GetBookByIdAsync(Guid id)
    {
        var result = await bookRepository.GetByIdAsync(id) 
            ?? throw new KeyNotFoundException($"A book with the specified id ({id}) was not found in the system.");

        return result;
    }
    public async Task<Book> CreateBookAsync(BookPostDTO book)
    {
        if(string.IsNullOrEmpty(book.Title))
        {
            throw new ArgumentException("Title cannot be empty.");
        }
        if(!DateTime.TryParse(book.ReleaseDate, out DateTime releaseDate))
        {
            throw new FormatException("Invalid date format. Please use the following format: YYYY-MM-DD");
        }
        if(!Enum.TryParse(book.Language, true, out Language language))
        {
            throw new FormatException("Invalid language format. Pick one of the following values: English, Polish, German, French, Spanish, Other.");
        }

        if(!Guid.TryParse(book.CategoryId, out var categoryGuid))
        {
            throw new FormatException("Invalid category's id format.");
        }
        var category = await categoryRepository.GetByIdAsync(categoryGuid) 
            ?? throw new ArgumentException("Category with the given id is not present in the system.");

        var author = await authorService.GetAuthorByIdAsync(book.AuthorId);

        if(book.Tags.Any(tag => tag.Contains(',')))
        {
            throw new FormatException("Invalid tags format. Please do not use commas in tags.");
        }
        var tagsInString = book.Tags.ToList().Count > 1 
            ? string.Join(',', book.Tags.Select(t => t.ToLower())) 
            : book.Tags.First() ?? "";

        bool isThereSimilarBook = bookRepository.GetQueryable()
            .Any(b => b.Title.ToLower() == book.Title.ToLower() 
                && b.AuthorId == author.Id
            );
        if(isThereSimilarBook)
        {
            throw new InvalidOperationException("There is a similar book in the system.");
        }

        Book newBook = new()
        {
            Title = book.Title,
            Description = book.Description,
            ReleaseDate = releaseDate,
            Language = language,
            Author = author,
            AuthorId = author.Id,
            Category = category,
            CategoryId = categoryGuid,
            Tags = tagsInString,
        };

        await bookRepository.AddAsync(newBook);
        return newBook;
    }
    public async Task<Book> UpdateBookAsync(BookPutDTO book)
    {
        Book existingBook = await GetBookByIdAsync(book.Id);

        if(book.Title is not null && existingBook.Title != book.Title)
        {
            if(string.IsNullOrEmpty(""))
            {
                throw new ArgumentException("Title can not be empty.");
            }
            existingBook.Title = book.Title;
        }

        if(book.ReleaseDate is not null)
        {
            if(!DateTime.TryParse(book.ReleaseDate, out var releaseDate))
            {
                throw new FormatException("Invalid date format. Please use the following format: YYYY-MM-DD");
            }
            existingBook.ReleaseDate = releaseDate;
        }

        if(book.Language is not null)
        {
            if(!Enum.TryParse<Language>(book.Language, true, out var language))
            {
                throw new FormatException("Invalid language format. Pick one of the following values: English, Polish, German, French, Spanish, Other.");
            }
            existingBook.Language = language;
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

        if(book.AuthorId is not null)
        {
            var author = await authorService.GetAuthorByIdAsync(book.AuthorId);
                
            existingBook.Author = author;
            existingBook.AuthorId = author.Id;
        }

        if(book.CategoryId is not null)
        {
            if(!Guid.TryParse(book.CategoryId, out var categoryGuid))
            {
                throw new FormatException("Invalid category's id format.");
            }

            var category = await categoryRepository.GetByIdAsync(categoryGuid)
                ?? throw new ArgumentException("Category with the given id is not present in the system.");
                
            existingBook.Category = category;
            existingBook.CategoryId = categoryGuid;
        }

        bool isThereSimilarBook = bookRepository.GetQueryable()
            .Any(b => b.Id != existingBook.Id 
                && b.Title.ToLower() == existingBook.Title.ToLower()
                && b.AuthorId == existingBook.AuthorId
            );

        if(isThereSimilarBook)
        {
            throw new InvalidOperationException("There is already a similar book in the system.");
        }

        await bookRepository.UpdateAsync(existingBook);

        return existingBook;
    }
    public async Task DeleteBookAsync(string id)
    {     
        Book book = await GetBookByIdAsync(id);

        var copies = copyRepository.GetQueryable()
            .Where(c => c.BookId == book.Id)
            .Select(c => c.Id)
            .ToList();

        foreach(var c in copies)
        {
            bool areThereActiveBorrowings = borrowingRepository
                .GetQueryable()
                .Any(bor => bor.CopyId == c 
                    && !bor.ActualReturnDate.HasValue
                );
            if(areThereActiveBorrowings)
            {
                throw new InvalidOperationException("The book cannot be deleted. There are still active borrowings in the system.");
            }
            await copyRepository.DeleteAsync(c);
        }

        await bookRepository.DeleteAsync(book.Id);
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

        var searchBooksQuery = bookRepository.GetQueryable();
        var searchBooksResult = searchBooksQuery.ToList();
        
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
            searchBooksResult = searchBooksQuery.ToList();
        }


        if (isAvailable.HasValue)
        {
            var copies = copyRepository.GetAllAsync().GetAwaiter().GetResult().ToList(); //??
            var borrowings = borrowingRepository.GetAllAsync().GetAwaiter().GetResult().ToList(); //??
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
            searchBooksResult = searchBooksQuery.ToList();
        }

        if (!string.IsNullOrEmpty(olderThan))
        {
            if (!DateTime.TryParse(olderThan, out DateTime olderThanDate))
            {
                throw new FormatException("Invalid date format of olderThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchBooksQuery = searchBooksQuery.Where(b => b.ReleaseDate <= olderThanDate);
            searchBooksResult = searchBooksQuery.ToList();
        }

        if (!string.IsNullOrEmpty(newerThan))
        {
            if(!DateTime.TryParse(newerThan, out DateTime newerThanDate))
            {
                throw new FormatException("Invalid date format of newerThan parameter. Please use the following format: YYYY-MM-DD");
            }
            searchBooksQuery = searchBooksQuery.Where(b => b.ReleaseDate >= newerThanDate);
            searchBooksResult = searchBooksQuery.ToList();
        }

        if (!string.IsNullOrEmpty(authorId))
        {
            var author = await authorService.GetAuthorByIdAsync(authorId);
            searchBooksQuery = searchBooksQuery.Where(b => b.AuthorId == author.Id);
            searchBooksResult = searchBooksQuery.ToList();
        }

        if (!string.IsNullOrEmpty(categoryId))
        {
            if(!Guid.TryParse(categoryId, out var categoryGuid))
            {
                throw new FormatException("Invalid category's id format.");
            }
            
            var category = await categoryRepository.GetByIdAsync(categoryGuid)
                ?? throw new KeyNotFoundException($"A category with the specified id ({categoryId}) was not found in the system.");

            searchBooksQuery = searchBooksQuery.Where(b => b.CategoryId == categoryGuid);
            searchBooksResult = searchBooksQuery.ToList();
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

    private static Guid ValidateGuid(string id)
    {
        if(!Guid.TryParse(id, out var bookGuid))
        {
            throw new FormatException("Invalid ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
        }
        return bookGuid;
    }
}