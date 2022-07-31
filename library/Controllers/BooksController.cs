using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using library.DTO;
using library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<BooksController> _logger;
        private readonly IMapper _mapper;

        public BooksController(LibraryContext context, IConfiguration config, ILogger<BooksController> logger, IMapper mapper)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks(string order)
        {
            switch (order.ToLower())
            {
                case "author":
                    return await _context.Books
                         .Include(rat => rat.Ratings)
                         .Include(rew => rew.Reviews)
                         .Select(b => new BookDTO()
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Author = b.Author,
                             Raiting = b.Ratings.Select(s => s.Score).DefaultIfEmpty().Average(),
                             ReviewsNumber = b.Reviews.Count
                         })
                         .OrderBy(x => x.Author)
                         .ToListAsync();
                case "title":
                    return await _context.Books
                         .Include(rat => rat.Ratings)
                         .Include(rew => rew.Reviews)
                         .Select(b => new BookDTO()
                         {
                             Id = b.Id,
                             Title = b.Title,
                             Author = b.Author,
                             Raiting = b.Ratings.Select(s => s.Score).DefaultIfEmpty().Average(),
                             ReviewsNumber = b.Reviews.Count
                         })
                         .OrderBy(x => x.Title)
                         .ToListAsync();
                default:
                    return BadRequest();

            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailDTO>> GetBook(int id)
        {

            var book = await _context.Books
                .Include(rat => rat.Ratings)
                .Include(rew => rew.Reviews)
                .Select(b => new BookDetailDTO()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Cover = b.Cover,
                    Content = b.Content,
                    Raiting = b.Ratings.Select(s => s.Score).DefaultIfEmpty().Average(),
                    Reviews = b.Reviews.Select(r => new ReviewDTO()
                    {
                        Id = r.Id,
                        Message = r.Message,
                        Reviewer = r.Reviewer
                    })
                    .ToList()
                })
                .SingleOrDefaultAsync(b => b.Id == id);

            return book;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id, string secret)
        {
            var confSecret = _config.GetValue<string>("secret");

            try
            {
                if (secret.ToLower().Equals(confSecret))
                {
                    Book book = await _context.Books.FindAsync(id);
                    if (book == null)
                    {
                        _logger.LogInformation($"Book with Id = {id} not found");
                        return BadRequest($"Book with Id = {id} not found");
                    }

                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Book with Id = {id} has been deleted");
                    return Content($"Book with Id = {id} has been deleted");
                }
                else
                {
                    _logger.LogInformation($"Wrong secret key");
                    return BadRequest($"Wrong secret key");
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Problem();
            }

        }

        [HttpPost("save")]
        public async Task<ActionResult<BookIdDTO>> PostBook([FromBody] BookPostDTO book)
        {
            try
            {
                if (book is null)
                {
                    _logger.LogError("Book object sent from client is null.");
                    return BadRequest("Book object is null");
                }
                
                Book bookEntity = _mapper.Map<Book>(book);
                int id = bookEntity.Id;
                if (_context.Books.Any(e => e.Id == id))
                {
                    _context.Books.Update(bookEntity);
                }
                else
                {
                    _context.Books.Add(bookEntity);
                }

                _context.SaveChanges();

                Book bookSaved = await _context.Books.FindAsync(bookEntity.Id);
                if (bookSaved == null)
                {
                    return Problem();
                }
                BookIdDTO bookId = _mapper.Map<BookIdDTO>(bookSaved);
                return bookId;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Problem();
            }

        }

        [HttpPut("{id}/review")]
        public async Task<ActionResult<ReviewIdDTO>> PutReview(int id, [FromBody] ReviewPutDTO review)
        {
            try
            {
                Review reviewEntity = _mapper.Map<Review>(review);
                Book book = await _context.Books.FindAsync(id);

                if (_context.Books.Any(e => e.Id == id))
                {
                    reviewEntity.BookId = book.Id;
                    _context.Entry(reviewEntity).State = EntityState.Modified;
                    _context.Reviews.Add(reviewEntity);
                    await _context.SaveChangesAsync();

                    Review reviewSaved = await _context.Reviews.FindAsync(reviewEntity.Id);
                    if (reviewSaved == null)
                    {
                        return Problem();
                    }
                    ReviewIdDTO reviewId = _mapper.Map<ReviewIdDTO>(reviewSaved);

                    return reviewId;
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Problem();
            }
            
        }

        [HttpPut("{id}/rate")]
        public async Task<ActionResult> PutRating(int id, [FromBody] RatingPutDTO rating)
        {
            try
            {
                Rating ratingEntity = _mapper.Map<Rating>(rating);
                Book book = await _context.Books.FindAsync(id);

                if (_context.Books.Any(e => e.Id == id))
                {
                    ratingEntity.BookId = book.Id;
                    _context.Entry(ratingEntity).State = EntityState.Modified;
                    _context.Ratings.Add(ratingEntity);
                    await _context.SaveChangesAsync();
                    
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Problem();
            }

        }

        [HttpGet("throw")]
        [Route("/error")]
        public IActionResult HandleError() => Problem();
    }

}
