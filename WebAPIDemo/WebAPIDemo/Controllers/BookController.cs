using Microsoft.AspNetCore.Mvc;
using WebAPIDemo.Models;
using WebAPIDemo.Providers;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookProvider _bookProvider;

        public BooksController(IBookProvider bookProvider, ILogger<BooksController> logger)
        {
            _bookProvider = bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
        }

        /// <summary>
        /// Gets all books
        /// </summary>
        /// <returns>List of all books</returns>
        /// <response code="200">Returns all books</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _bookProvider.GetAllBooksAsync();
            return Ok(books);
        }

        /// <summary>
        /// Gets a specific book by ID
        /// </summary>
        /// <param name="id">The book ID</param>
        /// <returns>The requested book</returns>
        /// <response code="200">Returns the requested book</response>
        /// <response code="404">If the book is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookProvider.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found");
            }

            return Ok(book);
        }

        /// <summary>
        /// Gets books by author
        /// </summary>
        /// <param name="author">The author name to search for</param>
        /// <returns>List of books by the author</returns>
        /// <response code="200">Returns books by the author</response>
        /// <response code="400">If the author parameter is empty</response>
        /// <response code="404">If there are no books with given author</response>
        [HttpGet("author/{author}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return BadRequest("Author parameter cannot be empty");
            }

            var books = await _bookProvider.GetBooksByAuthorAsync(author);
            if (books.Count() == 0) return NotFound();
            return Ok(books);
        }

        /// <summary>
        /// Gets books by genre
        /// </summary>
        /// <param name="genre">The genre to search for</param>
        /// <returns>List of books in the genre</returns>
        /// <response code="200">Returns books in the genre</response>
        /// <response code="400">If the genre parameter is empty</response>
        /// <response code="404">If there are no books with given genre</response>
        [HttpGet("genre/{genre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
            {
                return BadRequest("Genre parameter cannot be empty");
            }

            var books = await _bookProvider.GetBooksByGenreAsync(genre);
            if (books.Count() == 0) return NotFound();
            return Ok(books);
        }

        /// <summary>
        /// Gets books by title
        /// </summary>
        /// <param name="title">The title to search for</param>
        /// <returns>List of books with matching title</returns>
        /// <response code="200">Returns books with matching title</response>
        /// <response code="400">If the title parameter is empty</response>
        /// <response code="404">If there are no books with given title</response>
        [HttpGet("title/{title}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title parameter cannot be empty");
            }

            var books = await _bookProvider.GetBooksByTitleAsync(title);
            if (books.Count() == 0) return NotFound();
            return Ok(books);
        }

        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="book">The book to create (ID will be ignored/generated)</param>
        /// <returns>The created book</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the book data is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Ensure ID is not set (will be generated by provider)
                book.Id = 0;

                var createdBook = await _bookProvider.AddBookAsync(book);

                return CreatedAtAction(
                    nameof(GetBook),
                    new { id = createdBook.Id },
                    createdBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="id">The book ID to update</param>
        /// <param name="book">The updated book data</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was updated successfully</response>
        /// <response code="400">If the book data is invalid or ID mismatch</response>
        /// <response code="404">If the book was not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest("ID in route does not match ID in book data");
            }

            try
            {
                var updatedBook = await _bookProvider.UpdateBookAsync(book);
                if (updatedBook == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a specific book
        /// </summary>
        /// <param name="id">The book ID to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was deleted successfully</response>
        /// <response code="404">If the book was not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookProvider.DeleteBookAsync(id);
            if (!result)
            {
                return NotFound($"Book with ID {id} not found");
            }

            return NoContent();
        }
    }
}