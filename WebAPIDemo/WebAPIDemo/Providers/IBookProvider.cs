using WebAPIDemo.Models;

namespace WebAPIDemo.Providers
{
    public interface IBookProvider
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetBooksByGenreAsync(string genre);
        Task<Book> AddBookAsync(Book book);
        Task<Book?> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);

    }
}
