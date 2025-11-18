using WebAPIDemo.Models;

namespace WebAPIDemo.Providers
{
    public abstract class BookProvider : IBookProvider
    {
        protected List<Book> _books;
        protected int _nextId;

        public BookProvider(List<Book> books)
        {
            _books = books;
        }
        /// <summary>
        /// Returns all books in the model
        /// </summary>
        /// <returns>IEnumerable of all books</returns>
        public Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return Task.FromResult(_books.AsEnumerable());
        }
        /// <summary>
        /// Returns a book with a given ID
        /// </summary>
        /// <param name="id">ID of a book</param>
        /// <returns>A book with a given ID, or a null if no such book exists</returns>
        public Task<Book?> GetBookByIdAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(book);
        }
        /// <summary>
        /// Returns all books that contain the author name
        /// </summary>
        /// <param name="author">Author name to search</param>
        /// <returns>Books with author name, or empty list if no such book exists</returns>
        public Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        {
            var results = _books.Where(x =>
                x.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(results);
        }
        /// <summary>
        /// Returns all books that contain the genre name
        /// </summary>
        /// <param name="genre">Genre to search</param>
        /// <returns>Books with genre, or empty list if no such book exists</returns>
        public Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre)
        {
            var results = _books.Where(x =>
                x.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(results);
        }
        /// <summary>
        /// Returns all books that contain the title
        /// </summary>
        /// <param name="title">Title to search</param>
        /// <returns>Books with name, or empty list if no such book exists</returns>
        public Task<IEnumerable<Book>> GetBooksByTitleAsync(string title)
        {
            var results = _books.Where(x =>
                x.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(results);
        }
        /// <summary>
        /// Adds a new book to the collection
        /// </summary>
        /// <param name="book">Book to add (ID will be auto-generated)</param>
        /// <returns>The added book with generated ID</returns>
        public virtual Task<Book> AddBookAsync(Book book)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Author is required");

            // Check if book with same title and author already exists
            var existingBook = _books.FirstOrDefault(b =>
                b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(book.Author, StringComparison.OrdinalIgnoreCase));

            if (existingBook != null)
                throw new InvalidOperationException("A book with the same title and author already exists");

            // Assign new ID and add to collection
            book.Id = _nextId++;
            _books.Add(book);

            return Task.FromResult(book);
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="book">Book with updated information</param>
        /// <returns>The updated book, or null if book was not found</returns>
        public virtual Task<Book?> UpdateBookAsync(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null)
                return Task.FromResult<Book?>(null);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Author is required");

            // Check for duplicates (excluding current book)
            var duplicateBook = _books.FirstOrDefault(b =>
                b.Id != book.Id &&
                b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(book.Author, StringComparison.OrdinalIgnoreCase));

            if (duplicateBook != null)
                throw new InvalidOperationException("Another book with the same title and author already exists");

            // Update properties
            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Genre = book.Genre;

            return Task.FromResult<Book?>(existingBook);
        }

        /// <summary>
        /// Deletes a book by ID
        /// </summary>
        /// <param name="id">ID of the book to delete</param>
        /// <returns>True if book was found and deleted, false otherwise</returns>
        public virtual Task<bool> DeleteBookAsync(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return Task.FromResult(false);

            _books.Remove(book);
            return Task.FromResult(true);
        }



    }
}
