namespace FoodOrderingSystem.Models
{
    public class FoodItem
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}