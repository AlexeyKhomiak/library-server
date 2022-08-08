namespace library.DTO
{
    public class BookDetailDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Cover { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public decimal Rating { get; set; } = decimal.Zero;
        public decimal ReviewsNumber { get; set; } = decimal.Zero;
        public string Genre { get; set; } = string.Empty;
        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}
