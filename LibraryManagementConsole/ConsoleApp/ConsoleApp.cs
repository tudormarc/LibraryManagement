using System.Net.Http.Json;
using LibraryManagement.Dtos;

namespace LibraryManagementConsole.Application
{
    public class ConsoleApp
    {
        private readonly HttpClient _httpClient;

        public ConsoleApp(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5124/api/"); // Update to match your API base URL
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.WriteLine("Library Management System");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. Add Member");
                Console.WriteLine("3. Borrow Book");
                Console.WriteLine("4. Return Book");
                Console.WriteLine("5. Display Overdue Books");
                Console.WriteLine("6. Display All Books");
                Console.WriteLine("7. Display All Members");
                Console.WriteLine("8. Search Books");
                Console.WriteLine("9. Get Member's Borrowed Books");
                Console.WriteLine("10. Exit");
                Console.Write("Select an option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddBookAsync();
                        break;
                    case "2":
                        await AddMemberAsync();
                        break;
                    case "3":
                        await BorrowBookAsync();
                        break;
                    case "4":
                        await ReturnBookAsync();
                        break;
                    case "5":
                        await DisplayOverdueBooksAsync();
                        break;
                    case "6":
                        await DisplayAllBooksAsync();
                        break;
                    case "7":
                        await DisplayAllMembersAsync();
                        break;
                    case "8":
                        await SearchBooksAsync();
                        break;
                    case "9":
                        await GetMemberBorrowedBooksAsync();
                        break;
                    case "10":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private async Task AddBookAsync()
        {
            Console.Write("Enter book title: ");
            var title = Console.ReadLine();
            Console.Write("Enter book author: ");
            var author = Console.ReadLine();
            Console.Write("Enter book category: ");
            var category = Console.ReadLine();

            var book = new { Title = title, Author = author, Category = category };
            var response = await _httpClient.PostAsJsonAsync("books", book);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Book added successfully.");
            else
                Console.WriteLine($"Error: {response.ReasonPhrase}");
        }

        private async Task AddMemberAsync()
        {
            Console.Write("Enter member name: ");
            var name = Console.ReadLine();

            var member = new { Name = name };
            var response = await _httpClient.PostAsJsonAsync("members", member);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Member added successfully.");
            else
                Console.WriteLine($"Error: {response.ReasonPhrase}");
        }

        private async Task BorrowBookAsync()
        {
            Console.Write("Enter book ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId))
            {
                Console.WriteLine("Invalid book ID.");
                return;
            }

            Console.Write("Enter member ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId))
            {
                Console.WriteLine("Invalid member ID.");
                return;
            }

            var transaction = new { BookId = bookId, MemberId = memberId };
            var response = await _httpClient.PostAsJsonAsync("transactions/borrow", transaction);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Book borrowed successfully.");
            else
                Console.WriteLine($"Error: {response.ReasonPhrase}");
        }

        private async Task ReturnBookAsync()
        {
            Console.Write("Enter book ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var bookId))
            {
                Console.WriteLine("Invalid book ID.");
                return;
            }

            Console.Write("Enter member ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId))
            {
                Console.WriteLine("Invalid member ID.");
                return;
            }

            var transaction = new { BookId = bookId, MemberId = memberId };
            var response = await _httpClient.PostAsJsonAsync("transactions/return", transaction);

            if (response.IsSuccessStatusCode)
                Console.WriteLine("Book returned successfully.");
            else
                Console.WriteLine($"Error: {response.ReasonPhrase}");
        }

        private async Task DisplayOverdueBooksAsync()
        {
            var response = await _httpClient.GetAsync("transactions/overdue");

            if (response.IsSuccessStatusCode)
            {
                var overdueTransactions = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
                Console.WriteLine("Overdue Books:");

                foreach (var transaction in overdueTransactions)
                {
                    Console.WriteLine($"Transaction ID: {transaction.Id}, Book ID: {transaction.BookId}, Member ID: {transaction.MemberId}, Due Date: {transaction.DueDate}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }

        private async Task DisplayAllBooksAsync()
        {
            var response = await _httpClient.GetAsync("books");

            if (response.IsSuccessStatusCode)
            {
                var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
                Console.WriteLine("Books:");

                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, Category: {book.Category}, Available: {book.IsAvailable}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }

        private async Task DisplayAllMembersAsync()
        {
            var response = await _httpClient.GetAsync("members");

            if (response.IsSuccessStatusCode)
            {
                var members = await response.Content.ReadFromJsonAsync<List<MemberDto>>();
                Console.WriteLine("Members:");

                foreach (var member in members)
                {
                    Console.WriteLine($"ID: {member.Id}, Name: {member.Name}, Borrowed Books: {member.BorrowedBooksCount}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }

        private async Task SearchBooksAsync()
        {
            Console.Write("Enter title to search (leave blank to skip): ");
            var title = Console.ReadLine();

            Console.Write("Enter author to search (leave blank to skip): ");
            var author = Console.ReadLine();

            Console.Write("Enter category to search (leave blank to skip): ");
            var category = Console.ReadLine();

            var query = $"books/search?title={title}&author={author}&category={category}";
            var response = await _httpClient.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
                Console.WriteLine("Search Results:");

                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, Category: {book.Category}, Available: {book.IsAvailable}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }

        private async Task GetMemberBorrowedBooksAsync()
        {
            Console.Write("Enter member ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var memberId))
            {
                Console.WriteLine("Invalid member ID.");
                return;
            }

            var response = await _httpClient.GetAsync($"books/member/{memberId}/borrowed");

            if (response.IsSuccessStatusCode)
            {
                var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
                Console.WriteLine("Borrowed Books:");

                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, Category: {book.Category}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
    }
}