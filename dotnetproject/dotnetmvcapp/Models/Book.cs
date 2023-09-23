using System;
using System.Collections.Generic;

namespace BookStoreApp.Models;

public class Book
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Author { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }
}
