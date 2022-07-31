using System.ComponentModel.DataAnnotations.Schema;

namespace library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Cover { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;

        public ICollection<Rating> Ratings { get; } = new List<Rating>();
        public ICollection<Review> Reviews { get; } = new List<Review>();

    }
}
