using Entities.Interfaces;

namespace ApiServer.Services
{
    public class BookService
    {
        private readonly IBookRepository bookRepository;
        public BookService(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        //In the future, if the logic in the BookControler becomes too complicated, it will be moved here
    }
}
