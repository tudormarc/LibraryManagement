using LibraryManagement.Entities;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, string? category)
        {
            return await _bookRepository.SearchAsync(title, author, category);
        }

        public async Task<IEnumerable<Book>> GetBooksBorrowedByMemberAsync(Guid memberId)
        {
            return await _bookRepository.GetBooksBorrowedByMemberAsync(memberId);
        }
    }
}