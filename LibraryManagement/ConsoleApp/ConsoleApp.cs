namespace LibraryManagement.ConsoleApp
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
                Console.WriteLine("6. Exit");
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
                var overdueTransactions = await response.Content.ReadFromJsonAsync<List<dynamic>>();
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
    }
}