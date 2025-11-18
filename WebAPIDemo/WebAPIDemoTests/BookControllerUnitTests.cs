using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using WebAPIDemo.Controllers;
using WebAPIDemo.Models;
using WebAPIDemo.Providers;

namespace WebAPIDemoTests
{
    [TestFixture]
    public class BooksControllerUnitTests
    {
        private Mock<IBookProvider> _mockBookProvider;
        private Mock<ILogger<BooksController>> _mockLogger;
        private BooksController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBookProvider = new Mock<IBookProvider>();
            _mockLogger = new Mock<ILogger<BooksController>>();
            _controller = new BooksController(_mockBookProvider.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetBooks_ReturnsAllBooks()
        {
            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", Genre = "Genre 1" },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", Genre = "Genre 2" }
            };
            _mockBookProvider.Setup(x => x.GetAllBooksAsync()).ReturnsAsync(expectedBooks);

            var result = await _controller.GetBooks();

            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedBooks);
        }

        [Test]
        public async Task GetBook_WithValidId_ReturnsBook()
        {
            var expectedBook = new Book { Id = 1, Title = "Test Book", Author = "Test Author", Genre = "Test Genre" };
            _mockBookProvider.Setup(x => x.GetBookByIdAsync(1)).ReturnsAsync(expectedBook);


            var result = await _controller.GetBook(1);


            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedBook);
        }

        [Test]
        public async Task GetBook_WithInvalidId_ReturnsNotFound()
        {

            _mockBookProvider.Setup(x => x.GetBookByIdAsync(999)).ReturnsAsync((Book)null);


            var result = await _controller.GetBook(999);


            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Value.Should().Be("Book with ID 999 not found");
        }

        [Test]
        public async Task GetBooksByAuthor_WithValidAuthor_ReturnsBooks()
        {

            var expectedBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Stephen King", Genre = "Horror" }
            };
            _mockBookProvider.Setup(x => x.GetBooksByAuthorAsync("Stephen King")).ReturnsAsync(expectedBooks);


            var result = await _controller.GetBooksByAuthor("Stephen King");


            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedBooks);
        }

        [Test]
        public async Task GetBooksByAuthor_WithEmptyAuthor_ReturnsBadRequest()
        {

            var result = await _controller.GetBooksByAuthor("");


            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("Author parameter cannot be empty");
        }

        [Test]
        public async Task CreateBook_WithValidBook_ReturnsCreatedResult()
        {

            var newBook = new Book { Title = "New Book", Author = "New Author", Genre = "New Genre" };
            var createdBook = new Book { Id = 6, Title = "New Book", Author = "New Author", Genre = "New Genre" };

            _mockBookProvider.Setup(x => x.AddBookAsync(newBook)).ReturnsAsync(createdBook);

            var result = await _controller.CreateBook(newBook);


            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(BooksController.GetBook));
            createdResult.RouteValues["id"].Should().Be(6);
            createdResult.Value.Should().BeEquivalentTo(createdBook);
        }

        [Test]
        public async Task CreateBook_WithInvalidModel_ReturnsBadRequest()
        {

            var invalidBook = new Book { Title = "", Author = "Author", Genre = "Genre" };
            _controller.ModelState.AddModelError("Title", "Title is required");

            var result = await _controller.CreateBook(invalidBook);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CreateBook_WithDuplicateBook_ReturnsBadRequest()
        {
            var duplicateBook = new Book { Title = "Existing Book", Author = "Existing Author", Genre = "Genre" };
            _mockBookProvider.Setup(x => x.AddBookAsync(duplicateBook))
                .ThrowsAsync(new InvalidOperationException("A book with the same title and author already exists"));

            var result = await _controller.CreateBook(duplicateBook);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task UpdateBook_WithValidBook_ReturnsNoContent()
        {

            var existingBook = new Book { Id = 1, Title = "Updated Book", Author = "Updated Author", Genre = "Updated Genre" };
            _mockBookProvider.Setup(x => x.UpdateBookAsync(existingBook)).ReturnsAsync(existingBook);

            var result = await _controller.UpdateBook(1, existingBook);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task UpdateBook_WithIdMismatch_ReturnsBadRequest()
        {

            var book = new Book { Id = 1, Title = "Book", Author = "Author", Genre = "Genre" };

            var result = await _controller.UpdateBook(2, book);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be("ID in route does not match ID in book data");
        }

        [Test]
        public async Task UpdateBook_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var book = new Book { Id = 999, Title = "Non-existent", Author = "Author", Genre = "Genre" };
            _mockBookProvider.Setup(x => x.UpdateBookAsync(book)).ReturnsAsync((Book)null);

            // Act
            var result = await _controller.UpdateBook(999, book);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task DeleteBook_WithValidId_ReturnsNoContent()
        {
            _mockBookProvider.Setup(x => x.DeleteBookAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteBook(1);

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task DeleteBook_WithInvalidId_ReturnsNotFound()
        {

            _mockBookProvider.Setup(x => x.DeleteBookAsync(999)).ReturnsAsync(false);

            var result = await _controller.DeleteBook(999);

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}