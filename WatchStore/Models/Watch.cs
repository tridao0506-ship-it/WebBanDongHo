using System;

namespace WatchStore.Models
{
    /// <summary>
    /// Represents a watch product.
    /// </summary>
    public class Watch
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
