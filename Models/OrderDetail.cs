namespace FoodOrderingSystem.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string FoodName { get; set; }
        public decimal ItemTotal
        {
            get { return Price * Quantity; }
        }
    }
}