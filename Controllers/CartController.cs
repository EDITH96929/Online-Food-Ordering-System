using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FoodOrderingSystem.DAL;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Controllers
{
    public class CartController : Controller
    {
        FoodDAL foodDAL = new FoodDAL();
        OrderDAL orderDAL = new OrderDAL();
        RatingDAL ratingDAL = new RatingDAL();

        private List<CartItem> GetCart()
        {
            if (Session["Cart"] == null)
                Session["Cart"] = new List<CartItem>();
            return (List<CartItem>)Session["Cart"];
        }

        private bool IsUser()
        {
            return Session["UserId"] != null && Session["UserRole"].ToString() == "User";
        }

        public ActionResult Index()
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");
            return View(GetCart());
        }

        public ActionResult AddToCart(int foodId)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");

            FoodItem food = foodDAL.GetFoodById(foodId);
            if (food == null)
                return RedirectToAction("Menu", "Food");

            List<CartItem> cart = GetCart();
            CartItem existing = cart.FirstOrDefault(c => c.FoodId == foodId);

            if (existing != null)
                existing.Quantity++;
            else
                cart.Add(new CartItem
                {
                    FoodId = food.FoodId,
                    FoodName = food.FoodName,
                    Price = food.Price,
                    Quantity = 1,
                    ImagePath = food.ImagePath
                });

            Session["Cart"] = cart;
            TempData["Toast"] = "Item added to cart!";
            return RedirectToAction("Menu", "Food");
        }

        public ActionResult RemoveFromCart(int foodId)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(c => c.FoodId == foodId);
            if (item != null)
                cart.Remove(item);

            Session["Cart"] = cart;
            TempData["Toast"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateQuantity(int foodId, int quantity)
        {
            List<CartItem> cart = GetCart();
            CartItem item = cart.FirstOrDefault(c => c.FoodId == foodId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            Session["Cart"] = new List<CartItem>();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult PlaceOrder(string customerPhone, string deliveryAddress)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");

            List<CartItem> cart = GetCart();
            if (cart.Count == 0)
                return RedirectToAction("Index");

            int userId = (int)Session["UserId"];
            decimal total = cart.Sum(c => c.Total);

            Order order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                CustomerPhone = customerPhone,
                DeliveryAddress = deliveryAddress
            };
            int orderId = orderDAL.PlaceOrder(order);

            foreach (CartItem item in cart)
            {
                orderDAL.AddOrderDetail(new OrderDetail
                {
                    OrderId = orderId,
                    FoodId = item.FoodId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            Session["Cart"] = new List<CartItem>();
            TempData["Toast"] = "Order placed successfully!";
            return RedirectToAction("OrderHistory");
        }

        public ActionResult OrderHistory()
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var orders = orderDAL.GetOrdersByUser(userId);
            return View(orders);
        }

        public ActionResult OrderDetail(int? orderId)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");
            if (orderId == null)
                return RedirectToAction("OrderHistory");

            var order = orderDAL.GetOrderById(orderId.Value);
            var details = orderDAL.GetOrderDetails(orderId.Value);

            ViewBag.Details = details;
            ViewBag.HasRatedFood = ratingDAL.HasRated(orderId.Value, "Food");
            ViewBag.HasRatedDelivery = ratingDAL.HasRated(orderId.Value, "Delivery");
            return View(order);
        }

        public ActionResult MyReviews()
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var reviews = ratingDAL.GetRatingsByUser(userId);
            return View(reviews);
        }

        [HttpPost]
        public ActionResult RateOrder(int orderId, int stars, string comment,
                                       string ratingType, int? deliveryBoyId)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Account");

            ratingDAL.AddRating(new Rating
            {
                OrderId = orderId,
                UserId = (int)Session["UserId"],
                Stars = stars,
                Comment = comment,
                RatingType = ratingType,
                DeliveryBoyId = deliveryBoyId
            });

            TempData["Toast"] = ratingType == "Food"
                ? "Food rating submitted! Thank you."
                : "Delivery rating submitted! Thank you.";

            return RedirectToAction("OrderDetail", new { orderId = orderId });
        }
    }
}