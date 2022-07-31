namespace library.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public decimal Score { get; set; } = decimal.Zero;

        public int BookId { get; set; }
        public Book Book { get; set; } = new Book();

    }
}
