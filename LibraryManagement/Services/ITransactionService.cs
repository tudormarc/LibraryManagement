using LibraryManagement.Entities;

namespace LibraryManagement.Services
{
    public interface ITransactionService
    {
        Task BorrowBookAsync(Guid bookId, Guid memberId);
        Task ReturnBookAsync(Guid bookId, Guid memberId);
        Task<IEnumerable<Transaction>> GetOverdueTransactionsAsync();
    }
}
