using SimpleLibrary.Domain.Models;
using SimpleLibrary.Domain.Enumerations;
using SimpleLibrary.Domain.DTO;
using SimpleLibrary.Application.Repositories;
using SimpleLibrary.Application.Services.Abstraction;

namespace SimpleLibrary.Application.Services;

public class BookService: IBookService
{
    private readonly IBookRepository bookRepository;
    private readonly ICopyRepository copyRepository;
    private readonly IBorrowingRepository borrowingRepository;
    private readonly IAuthorRepository authorRepository;
    private readonly ICategoryRepository categoryRepository;

    public BookService(IBookRepository bookRepository, 
                        ICopyRepository copyRepository, 
                        IBorrowingRepository borrowingRepository, 
                        IAuthorRepository authorRepository, 
                        ICategoryRepository categoryRepository)
    {
        this.bookRepository = bookRepository;
        this.copyRepository = copyRepository;
        this.borrowingRepository = borrowingRepository;
        this.authorRepository = authorRepository;
        this.categoryRepository = categoryRepository;
    }

    public List<Book> SearchBooks(string? searchTerm = null, 
                                    bool? isAvailable = null, 
                                    string? olderThan = null, 
                                    string? newerThan = null, 
                                    int? author = null, 
                                    int? category = null,
                                    int page = 1,
                                    int pageSize = 25
    )
    {
        if(page <= 0 || pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        var booksQuery = bookRepository.GetBooks();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            booksQuery = booksQuery.Where(b =>
                b.Title.ToLower().Contains(searchTerm.ToLower())
                || b.Description.ToLower().Contains(searchTerm.ToLower())
                || b.Tags.ToLower().Contains(searchTerm.ToLower())
                || b.Language.ToString()!.ToLower().Contains(searchTerm.ToLower())
                || b.Author!.FirstName.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Author.LastName.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Author.Description.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Author.Tags.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Category!.Name.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Category.Description.ToString().ToLower().Contains(searchTerm.ToLower())
                || b.Category.Tags.ToString().ToLower().Contains(searchTerm.ToLower())
            );
        }

        if (isAvailable.HasValue)
        {
            var copies = copyRepository.GetAllCopies();
            var borrowings = borrowingRepository.GetAllBorrowings();
            if (isAvailable.Value)
            {
                booksQuery = booksQuery.Where(
                    b => copies.Exists(
                        c => c.BookId == b.Id && !borrowings.Exists(
                            br => br.CopyId == c.Id && br.ActualReturnDate == null
                        )
                    )
                );  
            }
            else
            {
                booksQuery = booksQuery.Where(
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
                throw new FormatException();
            }
            booksQuery = booksQuery.Where(b => b.ReleaseDate <= olderThanDate);
        }

        if (!string.IsNullOrEmpty(newerThan))
        {
            if(!DateTime.TryParse(newerThan, out DateTime newerThanDate))
            {
                throw new FormatException();
            }
            booksQuery = booksQuery.Where(b => b.ReleaseDate >= newerThanDate);
        }

        if (author.HasValue)
        {
            if(authorRepository.GetAuthor(author.Value) == null ) 
            { 
                throw new ArgumentOutOfRangeException(); 
            }
            booksQuery = booksQuery.Where(b => b.AuthorId == author);
        }

        if (category.HasValue)
        {
            if (categoryRepository.GetCategory(category.Value) == null) 
            { 
                throw new ArgumentOutOfRangeException(); 
            }
            booksQuery = booksQuery.Where(b => b.CategoryId == category);
        }

        var count = booksQuery.Count();
        if (count > 0 
            && page > Math.Ceiling( (decimal)count / pageSize ))
        {
            throw new InvalidOperationException();
        }

        booksQuery = booksQuery.Skip((page - 1) * pageSize);
        booksQuery = booksQuery.Count() > pageSize ? booksQuery.Take(pageSize) : booksQuery;

        return booksQuery.ToList();
    }

    public Book GetBookById(int id)
    {
        if(id < 0)
        {
            throw new ArgumentException("Book id cannot be lesser than 0.");
        }

        Book? book = bookRepository.GetBook(id);

        if (book == null)
        {
            throw new KeyNotFoundException();
        }

        return book;
    }

    public Book CreateBook(BookPostDTO book)
    {
        if(string.IsNullOrEmpty(book.title))
        {
            throw new ArgumentException("Title can not be empty.");
        }
        if(!DateTime.TryParse(book.releaseDate, out DateTime newReleaseDate))
        {
            throw new FormatException("Invalid date format");
        }
        if(!Enum.TryParse(book.language, true, out Language newLanguage))
        {
            throw new FormatException("Invalid language format. Pick one of the following values: English, Polish, German, French, Spanish, Other.");
        }
        if(authorRepository.GetAuthor(book.authorId) == null) 
        {
            throw new ArgumentException("Author with the given id is not present in the system.");
        }
        if(categoryRepository.GetCategory(book.categoryId) == null) 
        {
            throw new ArgumentException("Category with the given id is not present in the system.");
        }

        if(bookRepository.GetBooks().Any(b => b.Title == book.title && b.AuthorId == book.authorId))
        {
            throw new InvalidOperationException("There is a similar book in the system.");
        }

        Book newBook = new Book
        {
            Title = book.title,
            Description = book.desc,
            ReleaseDate = newReleaseDate,
            Language = newLanguage,
            Tags = book.tags,
            AuthorId = book.authorId,
            CategoryId = book.categoryId
        };

        bookRepository.CreateBook(newBook);
        return newBook;
    }

    public Book UpdateBook(BookPutDTO book)
    {
        if(book.id <= 0)
        {
            throw new ArgumentException("Invalid id.");
        }
        
        Book? existingBook = GetBookById(book.id);
        
        if(existingBook == null)
        {
            throw new KeyNotFoundException();
        }
        if(book.title == "")
        {
            throw new ArgumentException("Title can not be empty.");
        }
        if(book.releaseDate != null && !DateTime.TryParse(book.releaseDate, out var newReleaseDate))
        {
            throw new FormatException("Invalid date format.");
        }
        if(book.language != null && !Enum.TryParse<Language>(book.language, true, out var newLanguage))
        {
            throw new FormatException("Invalid language format. Pick one of the following values: English, Polish, German, French, Spanish, Other.");
        }
        if(book.authorId.HasValue && authorRepository.GetAuthor(book.authorId.Value) == null) 
        {
            throw new ArgumentException("Author with the given id is not present in the system.");
        }
        if(book.categoryId.HasValue && categoryRepository.GetCategory(book.categoryId.Value) == null) 
        {
            throw new ArgumentException("Category with the given id is not present in the system.");
        }
 
        existingBook.Title = book.title ?? existingBook.Title;
        existingBook.Description = book.desc ?? existingBook.Description;
        if(book.releaseDate != null) existingBook.ReleaseDate = DateTime.Parse(book.releaseDate);
        if(book.language != null) existingBook.Language = Enum.Parse<Language>(book.language);
        existingBook.Tags = book.tags ?? existingBook.Tags;
        existingBook.AuthorId = book.authorId ?? existingBook.AuthorId;
        existingBook.CategoryId = book.categoryId ?? existingBook.CategoryId;

        if(bookRepository.GetBooks().Any(b => b.Id != existingBook.Id && b.Title == existingBook.Title && b.AuthorId == existingBook.AuthorId))
        {
            throw new InvalidOperationException("There is a similar book in the system.");
        }

        bookRepository.UpdateBook(existingBook);

        return existingBook;
    }

    public void DeleteBook(int id)
    {
        if(id <= 0)
        {
            throw new ArgumentException("Invalid id.");
        }
        
        Book? book = GetBookById(id);
        
        if(book == null)
        {
            throw new KeyNotFoundException();
        }

        var copies = copyRepository.GetCopies().Where(c => c.BookId == id).Select(c => c.Id).ToList();
        foreach(var c in copies)
        {
            if(borrowingRepository.GetBorrowings().Any(bor => bor.CopyId == c && !bor.ActualReturnDate.HasValue))
            {
                throw new InvalidOperationException("The book can not be deleted. There are still active borrowings in the system.");
            }
        }

        bookRepository.DeleteBook(id);
    }

}