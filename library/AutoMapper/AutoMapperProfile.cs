using AutoMapper;
using library.DTO;
using library.Models;

namespace library.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BookPostDTO, Book>();
            CreateMap<Book, BookIdDTO>();
            CreateMap<ReviewPutDTO, Review>();
            CreateMap<Review, ReviewIdDTO>();
            CreateMap<RatingPutDTO, Rating>();
        }

    }
}
