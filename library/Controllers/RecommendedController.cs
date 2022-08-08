using library.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendedController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BooksController> _logger;

        public RecommendedController(LibraryContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookGenreDTO>>> GetRecommendedBooks(string? genre)
        {
            if (genre == null)
            {
                return await _context.Books
                     .Include(rat => rat.Ratings)
                     .Include(rew => rew.Reviews)
                     .Select(b => new BookGenreDTO()
                     {
                         Id = b.Id,
                         Title = b.Title,
                         Author = b.Author,
                         Cover = b.Cover,
                         Genre = b.Genre,
                         Rating = b.Ratings.Select(s => s.Score).DefaultIfEmpty().Average(),
                         ReviewsNumber = b.Reviews.Count
                     })
                     .Where(b => b.ReviewsNumber > 10)
                     .OrderByDescending(x => x.Rating)
                     .Take(10)
                     .ToListAsync();
            }
            else
            {
                List<string> allGenre = _context.Books.Select(x => x.Genre.ToLower()).Distinct().ToList();

                if (allGenre.Contains(genre.ToLower()))
                {
                    return await _context.Books
                         .Include(rat => rat.Ratings)
                         .Include(rew => rew.Reviews)
                         .Select(b => new BookGenreDTO()
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Author = b.Author,
                             Genre = b.Genre,
                             Rating = b.Ratings.Select(s => s.Score).DefaultIfEmpty().Average(),
                             ReviewsNumber = b.Reviews.Count
                         })
                         .Where(b => b.ReviewsNumber > 10 && b.Genre.ToLower() == genre.ToLower())
                         .OrderByDescending(x => x.Rating)
                         .Take(10)
                         .ToListAsync();
                }
                else
                {
                    _logger.LogInformation("Genre name does not exist");
                    return BadRequest();
                }
            }

        }

    }
}
