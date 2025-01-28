namespace LibraryManagement.Dtos
{
    public class CreateBookDto
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string Category { get; set; }
    }
}

