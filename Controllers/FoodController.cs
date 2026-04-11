using System.Web.Mvc;
using FoodOrderingSystem.DAL;

namespace FoodOrderingSystem.Controllers
{
    public class FoodController : Controller
    {
        FoodDAL foodDAL = new FoodDAL();
        RatingDAL ratingDAL = new RatingDAL();

        public ActionResult Menu()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["UserRole"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            if (Session["UserRole"].ToString() == "Delivery")
                return RedirectToAction("MyDeliveries", "Delivery");

            var foodItems = foodDAL.GetAllFoodItems();
            return View(foodItems);
        }

        public ActionResult Detail(int foodId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var food = foodDAL.GetFoodById(foodId);
            if (food == null)
                return RedirectToAction("Menu");

            var ratings = ratingDAL.GetFoodRatings(foodId);
            ViewBag.Ratings = ratings;
            return View(food);
        }
    }
}