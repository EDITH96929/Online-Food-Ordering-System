using System.Web.Mvc;
using FoodOrderingSystem.DAL;
using FoodOrderingSystem.Models;

namespace FoodOrderingSystem.Controllers
{
    public class AccountController : Controller
    {
        UserDAL userDAL = new UserDAL();
        DeliveryDAL deliveryDAL = new DeliveryDAL();

        public ActionResult Login()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Menu", "Food");
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            // Check regular users first
            User user = userDAL.LoginUser(email, password);
            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["UserName"] = user.Name;
                Session["UserRole"] = user.Role;
                Session["Phone"] = user.Phone;

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Menu", "Food");
            }

            // Check delivery boy login
            DeliveryBoy boy = deliveryDAL.LoginDeliveryBoy(email, password);
            if (boy != null)
            {
                Session["UserId"] = boy.DeliveryBoyId;
                Session["UserName"] = boy.Name;
                Session["UserRole"] = "Delivery";
                return RedirectToAction("MyDeliveries", "Delivery");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public ActionResult Register()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Menu", "Food");
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (userDAL.EmailExists(user.Email))
            {
                ViewBag.Error = "Email already registered. Please login.";
                return View();
            }
            bool success = userDAL.RegisterUser(user);
            if (success)
            {
                ViewBag.Success = "Registration successful! Please login.";
                return View("Login");
            }
            ViewBag.Error = "Registration failed. Please try again.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}