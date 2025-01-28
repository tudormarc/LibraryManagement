using LibraryManagement.Entities;

namespace LibraryManagement.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(Guid id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? category);
        Task<IEnumerable<Book>> GetBooksBorrowedByMemberAsync(Guid memberId);
    }
}
