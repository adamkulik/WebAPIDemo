using WebAPIDemo.Models;

namespace WebAPIDemo.Providers
{
    public class SampleBookProvider : BookProvider
    {
        public SampleBookProvider() : base(GenerateSampleBooks())
        {

        }


        private static List<Book> GenerateSampleBooks()
        {
            return new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Genre = "Classic"
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Genre = "Fiction"
                },
                new Book
                {
                    Id = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    Genre = "Dystopian"
                },
                new Book
                {
                    Id = 4,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    Genre = "Romance"
                },
                new Book
                {
                    Id = 5,
                    Title = "The Hobbit",
                    Author = "J.R.R. Tolkien",
                    Genre = "Fantasy"
                },
                new Book
                {
                    Id = 6,
                    Title = "The Shining",
                    Author = "Stephen King",
                    Genre = "Horror"
                },
                new Book
                {
                    Id = 7,
                    Title = "Dune",
                    Author = "Frank Herbert",
                    Genre = "Science Fiction"
                },
                new Book
                {
                    Id = 8,
                    Title = "Murder on the Orient Express",
                    Author = "Agatha Christie",
                    Genre = "Mystery"
                }
            };
        }
}
