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
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BooksController _booksController;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _mockMapper = new Mock<IMapper>();
            _booksController = new BooksController(_mockBookService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnOkResult_WithBooks()
        {
            // Given
            var books = new List<Book> { new Book { Id = Guid.NewGuid(), Title = "Test Book", Author = "Author", Category = "Category", IsAvailable = true } };
            _mockBookService.Setup(service => service.GetAllBooksAsync()).ReturnsAsync(books);

            var bookDtos = books.Select(book => new BookDto { Id = book.Id, Title = book.Title, Author = book.Author, Category = book.Category });
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(books)).Returns(bookDtos);

            // When
            var result = await _booksController.GetAllBooks();

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Single(returnBooks);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnOkResult_WithBook()
        {
            // Given
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Author", Category = "Category", IsAvailable = true };
            _mockBookService.Setup(service => service.GetBookByIdAsync(bookId)).ReturnsAsync(book);

            var bookDto = new BookDto { Id = book.Id, Title = book.Title, Author = book.Author, Category = book.Category };
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(bookDto);

            // When
            var result = await _booksController.GetBookById(bookId);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBook = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(bookId, returnBook.Id);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Given
            var nonExistentBookId = Guid.NewGuid();
            _mockBookService.Setup(service => service.GetBookByIdAsync(nonExistentBookId))
                            .ReturnsAsync((Book)null);

            // When
            var result = await _booksController.GetBookById(nonExistentBookId);

            // Then
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddBook_ShouldReturnCreatedAtActionResult()
        {
            // Given
            var createBookDto = new CreateBookDto { Title = "Test Book", Author = "Author", Category = "Category" };
            var book = new Book { Id = Guid.NewGuid(), Title = createBookDto.Title, Author = createBookDto.Author, Category = createBookDto.Category, IsAvailable = true };

            _mockMapper.Setup(mapper => mapper.Map<Book>(createBookDto)).Returns(book);
            _mockBookService.Setup(service => service.AddBookAsync(book)).Returns(Task.CompletedTask);

            // When
            var result = await _booksController.AddBook(createBookDto);

            // Then
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnBook = Assert.IsType<Book>(createdAtActionResult.Value);
            Assert.Equal(book.Title, returnBook.Title);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnNoContentResult()
        {
            // Given
            var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Updated Title", Author = "Updated Author", Category = "Updated Category" };
            var book = new Book { Id = bookDto.Id, Title = bookDto.Title, Author = bookDto.Author, Category = bookDto.Category, IsAvailable = true };

            _mockMapper.Setup(mapper => mapper.Map<Book>(bookDto)).Returns(book);
            _mockBookService.Setup(service => service.UpdateBookAsync(book)).Returns(Task.CompletedTask);

            // When
            var result = await _booksController.UpdateBook(bookDto.Id, bookDto);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnBadRequest_WhenIdsDoNotMatch()
        {
            // Given
            var bookId = Guid.NewGuid();
            var bookDto = new BookDto { Id = Guid.NewGuid(), Title = "Title", Author = "Author", Category = "Category" };

            // When
            var result = await _booksController.UpdateBook(bookId, bookDto);

            // Then
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Given
            var bookId = Guid.NewGuid();
            var bookDto = new BookDto
            {
                Id = bookId,
                Title = "Updated Title",
                Author = "Updated Author",
                Category = "Fiction"
            };

            _mockMapper.Setup(mapper => mapper.Map<Book>(bookDto))
                .Returns(new Book { Id = bookId, Title = bookDto.Title, Author = bookDto.Author, Category = bookDto.Category });

            _mockBookService.Setup(service => service.UpdateBookAsync(It.IsAny<Book>()))
                    .Throws(new InvalidOperationException("Book not found"));

            // When
            var result = await _booksController.UpdateBook(bookId, bookDto);

            // Then
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNoContentResult()
        {
            // Given
            var bookId = Guid.NewGuid();
            _mockBookService.Setup(service => service.DeleteBookAsync(bookId)).Returns(Task.CompletedTask);

            // When
            var result = await _booksController.DeleteBook(bookId);

            // Then
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Given
            var nonExistentBookId = Guid.NewGuid();
            _mockBookService.Setup(service => service.DeleteBookAsync(nonExistentBookId))
                            .Throws(new InvalidOperationException("Book not found"));

            // When
            var result = await _booksController.DeleteBook(nonExistentBookId);

            // Then
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchBooks_ShouldReturnOkResult_WithFilteredBooks()
        {
            // Given
            var books = new List<Book> 
            {
                new Book { Id = Guid.NewGuid(), Title = "Search Test Book", Author = "Author1", Category = "Category1" },
                new Book { Id = Guid.NewGuid(), Title = "Another Book", Author = "Author2", Category = "Category2" }
            };

            _mockBookService.Setup(service => service.SearchBooksAsync("Search", null, null)).ReturnsAsync(books);

            var bookDtos = books.Select(book => new BookDto { Id = book.Id, Title = book.Title, Author = book.Author, Category = book.Category });
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(books)).Returns(bookDtos);

            // When
            var result = await _booksController.SearchBooks("Search", null, null);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Equal(2, returnBooks.Count());
        }

        [Fact]
        public async Task SearchBooks_ShouldReturnEmptyList_WhenNoBooksMatch()
        {
            // Given
            _mockBookService.Setup(service => service.SearchBooksAsync("Nonexistent", null, null)).ReturnsAsync(new List<Book>());

            // When
            var result = await _booksController.SearchBooks("Nonexistent", null, null);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Empty(returnBooks);
        }

        [Fact]
        public async Task SearchBooks_ShouldFilterByMultipleCriteria()
        {
            // Given
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Filtered Book", Author = "Specific Author", Category = "Specific Category" }
            };

            _mockBookService.Setup(service => service.SearchBooksAsync("Filtered", "Specific Author", "Specific Category"))
                            .ReturnsAsync(books);

            var bookDtos = books.Select(book => new BookDto { Id = book.Id, Title = book.Title, Author = book.Author, Category = book.Category });
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(books)).Returns(bookDtos);

            // When
            var result = await _booksController.SearchBooks("Filtered", "Specific Author", "Specific Category");

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Single(returnBooks);
            Assert.Equal("Filtered Book", returnBooks.First().Title);
        }

        [Fact]
        public async Task GetBooksBorrowedByMember_ShouldReturnOkResult_WithBorrowedBooks()
        {
            // Given
            var memberId = Guid.NewGuid();
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Borrowed Book 1", Author = "Author 1", Category = "Category 1" },
                new Book { Id = Guid.NewGuid(), Title = "Borrowed Book 2", Author = "Author 2", Category = "Category 2" }
            };

            _mockBookService.Setup(service => service.GetBooksBorrowedByMemberAsync(memberId))
                            .ReturnsAsync(books);

            var bookDtos = books.Select(book => new BookDto { Id = book.Id, Title = book.Title, Author = book.Author, Category = book.Category });
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<BookDto>>(books)).Returns(bookDtos);

            // When
            var result = await _booksController.GetBooksBorrowedByMember(memberId);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Equal(2, returnBooks.Count());
        }

        [Fact]
        public async Task GetBooksBorrowedByMember_ShouldReturnEmptyList_WhenNoBooksBorrowed()
        {
            // Given
            var memberId = Guid.NewGuid();
            _mockBookService.Setup(service => service.GetBooksBorrowedByMemberAsync(memberId))
                            .ReturnsAsync(new List<Book>());

            // When
            var result = await _booksController.GetBooksBorrowedByMember(memberId);

            // Then
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Empty(returnBooks);
        }

        [Fact]
        public async Task GetBooksBorrowedByMember_ShouldHandleExceptionGracefully()
        {
            // Given
            var memberId = Guid.NewGuid();
            _mockBookService.Setup(service => service.GetBooksBorrowedByMemberAsync(memberId))
                            .ThrowsAsync(new Exception("Unexpected error"));

            // When
            var result = await _booksController.GetBooksBorrowedByMember(memberId);

            // Then
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = (ObjectResult)result.Result;
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}