using Xunit;
using Moq;
using LibraryManagement.Entities;
using LibraryManagement.Repositories;
using LibraryManagement.Services;

namespace LibraryManagement.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMemberRepository> _mockMemberRepository;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMemberRepository = new Mock<IMemberRepository>();

            _transactionService = new TransactionService(
                _mockTransactionRepository.Object,
                _mockBookRepository.Object,
                _mockMemberRepository.Object);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldBorrowBookSuccessfully()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var book = new Book { Id = bookId, IsAvailable = true, Author = "test", Title = "test", Category = "test" };
            var member = new Member { Id = memberId, Name = "name", BorrowedBooksCount = 0 };

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);

            // When
            await _transactionService.BorrowBookAsync(bookId, memberId);

            // Then
            Assert.False(book.IsAvailable);
            Assert.Equal(1, member.BorrowedBooksCount);
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.Is<Transaction>(t => t.BookId == bookId && t.MemberId == memberId)), Times.Once);
            _mockBookRepository.Verify(repo => repo.UpdateAsync(book), Times.Once);
            _mockMemberRepository.Verify(repo => repo.UpdateAsync(member), Times.Once);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldThrowException_WhenMemberHasReachedBorrowLimit()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var book = new Book { Id = bookId, IsAvailable = true, Author = "test", Title = "test", Category = "test" };
            var member = new Member { Id = memberId, Name = "name", BorrowedBooksCount = 5 }; // Borrow limit reached

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);

            // When & Then
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.BorrowBookAsync(bookId, memberId));
            Assert.Equal("Cannot borrow the book.", exception.Message);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null);
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(new Member { Id = memberId, Name = "name" });

            // When & Then
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.BorrowBookAsync(bookId, memberId));
            Assert.Equal("Cannot borrow the book.", exception.Message);
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldReturnBookSuccessfully()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var transaction = new Transaction { BookId = bookId, MemberId = memberId, ReturnedDate = null };
            var book = new Book { Id = bookId, IsAvailable = false, Author = "test", Title = "test", Category = "test" };
            var member = new Member { Id = memberId, Name = "name", BorrowedBooksCount = 1 };

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Transaction> { transaction });
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(book);
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(member);

            // When
            await _transactionService.ReturnBookAsync(bookId, memberId);

            // Then
            Assert.True(book.IsAvailable);
            Assert.Equal(0, member.BorrowedBooksCount);
            Assert.NotNull(transaction.ReturnedDate);
            _mockTransactionRepository.Verify(repo => repo.UpdateAsync(transaction), Times.Once);
            _mockBookRepository.Verify(repo => repo.UpdateAsync(book), Times.Once);
            _mockMemberRepository.Verify(repo => repo.UpdateAsync(member), Times.Once);
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldThrowException_WhenTransactionDoesNotExist()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Transaction>());

            // When & Then
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.ReturnBookAsync(bookId, memberId));
            Assert.Equal("No active transaction found for the book and member.", exception.Message);
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Transaction>());
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync((Book)null);
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(new Member { Id = memberId, Name = "name" });

            // When & Then
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.ReturnBookAsync(bookId, memberId));
            Assert.Equal("No active transaction found for the book and member.", exception.Message);
        }

        [Fact]
        public async Task ReturnBookAsync_ShouldThrowException_WhenTransactionDoesNotExistForMember()
        {
            // Given
            var bookId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            var transaction = new Transaction { BookId = bookId, MemberId = Guid.NewGuid(), ReturnedDate = null }; // Different member

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Transaction> { transaction });
            _mockBookRepository.Setup(repo => repo.GetByIdAsync(bookId)).ReturnsAsync(new Book { Id = bookId, Author = "test", Title = "test", Category = "test" });
            _mockMemberRepository.Setup(repo => repo.GetByIdAsync(memberId)).ReturnsAsync(new Member { Id = memberId, Name = "name" });

            // When & Then
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.ReturnBookAsync(bookId, memberId));
            Assert.Equal("No active transaction found for the book and member.", exception.Message);
        }

        [Fact]
        public async Task GetOverdueTransactionsAsync_ShouldReturnOverdueTransactions()
        {
            // Given
            var overdueTransaction = new Transaction { DueDate = DateTime.UtcNow.AddDays(-1), ReturnedDate = null };
            var validTransaction = new Transaction { DueDate = DateTime.UtcNow.AddDays(1), ReturnedDate = null };

            _mockTransactionRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Transaction> { overdueTransaction, validTransaction });

            // When
            var result = await _transactionService.GetOverdueTransactionsAsync();

            // Then
            Assert.Single(result);
            Assert.Contains(overdueTransaction, result);
        }
    }
} 