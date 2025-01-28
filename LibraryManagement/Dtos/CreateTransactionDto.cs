namespace LibraryManagement.Dtos
{
    public class CreateTransactionDto
    {
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
    }
}