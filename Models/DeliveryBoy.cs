using System;

namespace FoodOrderingSystem.Models
{
    public class DeliveryBoy
    {
        public int DeliveryBoyId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalDeliveries { get; set; }
        public DateTime JoinedOn { get; set; }
        public double AverageRating { get; set; }
    }
}