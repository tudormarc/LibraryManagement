using LibraryManagement.Entities;

namespace LibraryManagement.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid id);
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(Guid id);
        Task<IEnumerable<Book>> SearchBooksAsync(string? title, string? author, string? category);
        Task<IEnumerable<Book>> GetBooksBorrowedByMemberAsync(Guid memberId);
    }
}

