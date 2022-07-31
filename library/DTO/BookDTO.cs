﻿namespace library.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public decimal Raiting { get; set; } = decimal.Zero;
        public decimal ReviewsNumber { get; set; } = decimal.Zero;
    }
}