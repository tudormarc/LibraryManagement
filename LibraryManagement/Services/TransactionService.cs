using LibraryManagement.Entities;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;

        public TransactionService(ITransactionRepository transactionRepository, IBookRepository bookRepository, IMemberRepository memberRepository)
        {
            _transactionRepository = transactionRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        public async Task BorrowBookAsync(Guid bookId, Guid memberId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            var member = await _memberRepository.GetByIdAsync(memberId);

            if (book == null || member == null || !book.IsAvailable || member.BorrowedBooksCount >= 5)
            {
                throw new InvalidOperationException("Cannot borrow the book.");
            }

            book.IsAvailable = false;
            member.BorrowedBooksCount++;

            var transaction = new Transaction
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            await _transactionRepository.AddAsync(transaction);
            await _bookRepository.UpdateAsync(book);
            await _memberRepository.UpdateAsync(member);
        }

        public async Task ReturnBookAsync(Guid bookId, Guid memberId)
        {
            var transaction = (await _transactionRepository.GetAllAsync()).FirstOrDefault(t => t.BookId == bookId && t.MemberId == memberId && t.ReturnedDate == null);

            if (transaction == null)
            {
                throw new InvalidOperationException("No active transaction found for the book and member.");
            }

            var book = await _bookRepository.GetByIdAsync(bookId);
            var member = await _memberRepository.GetByIdAsync(memberId);

            if (book != null && member != null)
            {
                book.IsAvailable = true;
                member.BorrowedBooksCount--;
                transaction.ReturnedDate = DateTime.UtcNow;

                await _transactionRepository.UpdateAsync(transaction);
                await _bookRepository.UpdateAsync(book);
                await _memberRepository.UpdateAsync(member);
            }
        }

        public async Task<IEnumerable<Transaction>> GetOverdueTransactionsAsync()
        {
            var now = DateTime.UtcNow;
            return (await _transactionRepository.GetAllAsync()).Where(t => t.DueDate < now && t.ReturnedDate == null);
        }
    }
}