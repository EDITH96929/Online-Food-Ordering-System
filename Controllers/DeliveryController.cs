using FoodOrderingSystem.DAL;
using System;
using System.Web.Mvc;

namespace FoodOrderingSystem.Controllers
{
    public class DeliveryController : Controller
    {
        OrderDAL orderDAL = new OrderDAL();
        RatingDAL ratingDAL = new RatingDAL();

        private bool IsDelivery()
        {
            return Session["UserRole"] != null &&
                   Session["UserRole"].ToString() == "Delivery";
        }

        public ActionResult MyDeliveries()
        {
            if (!IsDelivery())
                return RedirectToAction("Login", "Account");

            int deliveryBoyId = Convert.ToInt32(Session["UserId"]);
            var orders = orderDAL.GetOrdersByDeliveryBoy(deliveryBoyId);
            return View(orders);
        }

        public ActionResult DeliveryHistory()
        {
            if (!IsDelivery())
                return RedirectToAction("Login", "Account");

            int deliveryBoyId = Convert.ToInt32(Session["UserId"]);
            var orders = orderDAL.GetDeliveryHistory(deliveryBoyId);
            return View(orders);
        }

        public ActionResult VerifyOTP(int? orderId)
        {
            if (!IsDelivery())
                return RedirectToAction("Login", "Account");

            if (orderId == null)
                return RedirectToAction("MyDeliveries");

            var order = orderDAL.GetOrderById(orderId.Value);
            return View(order);
        }

        public ActionResult MyReviews()
        {
            if (!IsDelivery())
                return RedirectToAction("Login", "Account");

            int deliveryBoyId = Convert.ToInt32(Session["UserId"]);
            var reviews = ratingDAL.GetDeliveryRatings(deliveryBoyId);
            double avgRating = ratingDAL.GetDeliveryAverageRating(deliveryBoyId);

            ViewBag.AverageRating = avgRating;
            return View(reviews);
        }

        [HttpPost]
        public ActionResult VerifyOTP(int orderId, string otp)
        {
            if (!IsDelivery())
                return RedirectToAction("Login", "Account");

            string result = orderDAL.VerifyOTP(orderId, otp);

            if (result == "success")
            {
                TempData["Toast"] = "OTP verified! Order marked as Delivered.";
                TempData["Success"] = "true";
            }
            else if (result == "wrong")
            {
                TempData["Toast"] = "Wrong OTP! Please try again.";
                TempData["Error"] = "Incorrect OTP entered.";
            }
            else if (result == "locked")
            {
                TempData["Toast"] = "Too many wrong attempts! Contact admin.";
                TempData["Error"] = "Maximum OTP attempts reached.";
            }

            return RedirectToAction("VerifyOTP", new { orderId = orderId });
        }
    }
}