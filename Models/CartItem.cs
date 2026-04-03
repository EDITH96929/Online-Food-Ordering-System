namespace FoodOrderingSystem.Models
{
    public class CartItem
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }
        public decimal Total
        {
            get { return Price * Quantity; }
        }
    }
}