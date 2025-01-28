namespace LibraryManagement.Dtos
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int BorrowedBooksCount { get; set; }
    }
}