using Xunit;
using Moq;
using System.Threading.Tasks;
using LibraryManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Services;
using AutoMapper;
using LibraryManagement.Entities;
using LibraryManagement.Dtos;

namespace LibraryManagement.Tests.Controllers 
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TransactionsController _transactionsController;

        public TransactionsControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockMapper = new Mock<IMapper>();
            _transactionsController = new TransactionsController(_mockTransactionService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task BorrowBook_ShouldReturnNoContent_WhenBorrowedBooksCountIs5()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            var member = new Member
            {
                Id = createTransactionDto.MemberId,
                Name = "Name",
                BorrowedBooksCount = 5
            };

            _mockTransactionService.Setup(service => service.BorrowBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Returns(Task.CompletedTask);

            // When
            var result = await _transactionsController.BorrowBook(createTransactionDto);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorrowBook_ShouldReturnBadRequest_WhenBorrowedBooksCountExceeds5()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            var member = new Member
            {
                Id = createTransactionDto.MemberId,
                Name = "Name",
                BorrowedBooksCount = 6
            };

            _mockTransactionService.Setup(service => service.BorrowBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Throws(new InvalidOperationException("Cannot borrow more than 5 books"));

            // When
            var result = await _transactionsController.BorrowBook(createTransactionDto);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task BorrowBook_ShouldReturnNoContent_WhenSuccessful()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            _mockTransactionService.Setup(service => service.BorrowBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Returns(Task.CompletedTask);

            // When
            var result = await _transactionsController.BorrowBook(createTransactionDto);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task BorrowBook_ShouldReturnBadRequest_WhenOperationFails()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            _mockTransactionService.Setup(service => service.BorrowBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Throws(new InvalidOperationException("Cannot borrow the book"));

            // When
            var result = await _transactionsController.BorrowBook(createTransactionDto);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ReturnBook_ShouldReturnNoContent_WhenSuccessful()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            _mockTransactionService.Setup(service => service.ReturnBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Returns(Task.CompletedTask);

            // When
            var result = await _transactionsController.ReturnBook(createTransactionDto);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReturnBook_ShouldReturnBadRequest_WhenOperationFails()
        {
            // Given
            var createTransactionDto = new CreateTransactionDto
            {
                BookId = Guid.NewGuid(),
                MemberId = Guid.NewGuid()
            };

            _mockTransactionService.Setup(service => service.ReturnBookAsync(createTransactionDto.BookId, createTransactionDto.MemberId))
                .Throws(new InvalidOperationException("Cannot return the book"));

            // When
            var result = await _transactionsController.ReturnBook(createTransactionDto);

            // Then
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetOverdueTransactions_ShouldReturnOkResult_WithTransactions()
        {
            // Given
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    BookId = Guid.NewGuid(),
                    MemberId = Guid.NewGuid(),
                    BorrowedDate = DateTime.Now.AddDays(-20),
                    DueDate = DateTime.Now.AddDays(-6),
                    ReturnedDate = null
                }
            };

            _mockTransactionService.Setup(service => service.GetOverdueTransactionsAsync()).ReturnsAsync(transactions);

            var transactionDtos = transactions.Select(transaction => new TransactionDto
            {
                Id = transaction.Id,
                BookId = transaction.BookId,
                MemberId = transaction.MemberId,
                BorrowedDate = transaction.BorrowedDate,
                DueDate = transaction.DueDate,
                ReturnedDate = transaction.ReturnedDate
            });

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TransactionDto>>(transactions)).Returns(transactionDtos);

            // When
            var result = await _transactionsController.GetOverdueTransactions();

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnTransactions = Assert.IsAssignableFrom<IEnumerable<TransactionDto>>(okResult.Value);
            Assert.Single(returnTransactions);
        }

        [Fact]
        public async Task GetOverdueTransactions_ShouldReturnEmptyList_WhenNoOverdueTransactions()
        {
            // Given
            _mockTransactionService.Setup(service => service.GetOverdueTransactionsAsync())
                .ReturnsAsync(new List<Transaction>());

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TransactionDto>>(It.IsAny<IEnumerable<Transaction>>()))
                .Returns(new List<TransactionDto>());

            // When
            var result = await _transactionsController.GetOverdueTransactions();

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnTransactions = Assert.IsAssignableFrom<IEnumerable<TransactionDto>>(okResult.Value);
            Assert.Empty(returnTransactions);
        }
    }
}