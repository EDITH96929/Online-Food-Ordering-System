using System;

namespace FoodOrderingSystem.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string OTP { get; set; }
        public string OrderCode { get; set; }
        public int? DeliveryTime { get; set; }
        public int? DeliveryBoyId { get; set; }
        public string DeliveryBoyName { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
    }
}