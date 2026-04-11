using FoodOrderingSystem.DAL;
using FoodOrderingSystem.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FoodOrderingSystem.Controllers
{
    public class AdminController : Controller
    {
        FoodDAL foodDAL = new FoodDAL();
        OrderDAL orderDAL = new OrderDAL();
        DeliveryDAL deliveryDAL = new DeliveryDAL();
        RatingDAL ratingDAL = new RatingDAL();

        private bool IsAdmin()
        {
            return Session["UserRole"] != null &&
                   Session["UserRole"].ToString() == "Admin";
        }

        // Dashboard
        public ActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var orders = orderDAL.GetAllOrders();
            var stats = orderDAL.GetDashboardStats();
            ViewBag.Stats = stats;
            return View(orders);
        }

        // Manage Food
        public ActionResult ManageFood()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var foods = foodDAL.GetAllFoodItemsForAdmin();
            return View(foods);
        }

        public ActionResult AddFood()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult AddFood(FoodItem item)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            foodDAL.AddFoodItem(item);
            TempData["Toast"] = "Food item added successfully!";
            return RedirectToAction("ManageFood");
        }

        public ActionResult EditFood(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var food = foodDAL.GetFoodById(id);
            return View(food);
        }

        [HttpPost]
        public ActionResult EditFood(FoodItem item)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            foodDAL.UpdateFoodItem(item);
            TempData["Toast"] = "Food item updated!";
            return RedirectToAction("ManageFood");
        }

        public ActionResult DeleteFood(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            foodDAL.DeleteFoodItem(id);
            TempData["Toast"] = "Food item hidden from menu!";
            return RedirectToAction("ManageFood");
        }

        // Assign Delivery
        public ActionResult AssignDelivery(int orderId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var order = orderDAL.GetOrderById(orderId);
            var deliveryBoys = deliveryDAL.GetAvailableDeliveryBoys();
            ViewBag.DeliveryBoys = deliveryBoys;
            return View(order);
        }

        [HttpPost]
        public ActionResult AssignDelivery(int orderId, int deliveryBoyId, int deliveryMinutes)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            orderDAL.AssignDelivery(orderId, deliveryBoyId, deliveryMinutes);
            TempData["Toast"] = "Delivery assigned! Customer notified.";
            return RedirectToAction("Dashboard");
        }

        // Order Detail
        public ActionResult OrderDetail(int orderId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var order = orderDAL.GetOrderById(orderId);
            var details = orderDAL.GetOrderDetails(orderId);
            ViewBag.Details = details;
            return View(order);
        }
        // Add this action
        public ActionResult FoodReviews()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var allFoods = foodDAL.GetAllFoodItemsForAdmin();
            var reviewData = new Dictionary<FoodOrderingSystem.Models.FoodItem,
                                            List<FoodOrderingSystem.Models.Rating>>();

            foreach (var food in allFoods)
            {
                var ratings = ratingDAL.GetFoodRatings(food.FoodId);
                if (ratings.Count > 0)
                    reviewData[food] = ratings;
            }

            return View(reviewData);
        }

        // ── Delivery Partners ──────────────────────────────────

        public ActionResult ManageDelivery()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var boys = deliveryDAL.GetAllDeliveryBoys();

            // Attach average rating to each
            foreach (var boy in boys)
                boy.AverageRating = ratingDAL.GetDeliveryAverageRating(boy.DeliveryBoyId);

            return View(boys);
        }

        public ActionResult AddDeliveryBoy()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult AddDeliveryBoy(DeliveryBoy boy)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (deliveryDAL.EmailExists(boy.Email))
            {
                TempData["Toast"] = "Email already registered!";
                return View(boy);
            }

            deliveryDAL.AddDeliveryBoy(boy);
            TempData["Toast"] = "Delivery partner added successfully!";
            return RedirectToAction("ManageDelivery");
        }

        public ActionResult ToggleDeliveryBoy(int id, bool activate)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            deliveryDAL.ToggleAvailability(id, activate);
            TempData["Toast"] = activate
                ? "Delivery partner activated!"
                : "Delivery partner deactivated!";
            return RedirectToAction("ManageDelivery");
        }

        public ActionResult DeliveryBoyDetail(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var boy = deliveryDAL.GetDeliveryBoyById(id);
            var ratings = ratingDAL.GetDeliveryRatings(id);
            boy.AverageRating = ratingDAL.GetDeliveryAverageRating(id);
            ViewBag.Ratings = ratings;
            return View(boy);
        }
    }
}