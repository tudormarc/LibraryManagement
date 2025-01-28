using LibraryManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class BookSqlRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        public BookSqlRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? category)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => EF.Functions.Like(b.Title, $"%{title}%"));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => EF.Functions.Like(b.Category, $"%{category}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksBorrowedByMemberAsync(Guid memberId)
        {
            return await _context.Books
                .Where(b => _context.Transactions.Any(t => t.BookId == b.Id && t.MemberId == memberId && t.ReturnedDate == null))
                .ToListAsync();
        }
    }
}
