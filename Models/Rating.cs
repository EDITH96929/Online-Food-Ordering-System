using System;

namespace FoodOrderingSystem.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public DateTime RatedOn { get; set; }
        public string OrderCode { get; set; }
        public string RatingType { get; set; }
        public int? DeliveryBoyId { get; set; }
    }
}