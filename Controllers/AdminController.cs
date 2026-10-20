using System;
using System.Linq;
using System.Web.Mvc;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    [Authorize(Roles = "Admin")] // Locks this ENTIRE controller to Admins only
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            // Calculate statistics for the dashboard widgets
            ViewBag.TotalFacilities = db.Facilities.Count();
            ViewBag.TotalUsers = db.Users.Count();
            
            // Count bookings that are currently active (Reserved or Checked-In)
            int activeBookings = db.Bookings.Count(b => b.Status == "Checked-In" || b.Status == "Reserved");
            ViewBag.ActiveBookings = activeBookings;

            return View();
        }

        // GET: Admin/ManageUsers
        public ActionResult ManageUsers()
        {
            // Gets a list of all users to show in a table
            var users = db.Users.ToList();
            return View(users);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(string id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                // Önce bu kullanıcıya ait rezervasyonları silmeliyiz (Veritabanı ilişkisi hatası almamak için)
                var userBookings = db.Bookings.Where(b => b.UserId == id).ToList();
                if (userBookings.Any())
                {
                    db.Bookings.RemoveRange(userBookings);
                }

                // Sonra kullanıcıyı siliyoruz
                db.Users.Remove(user);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Kullanıcı ve ona ait tüm rezervasyonlar başarıyla silindi.";
            }
            return RedirectToAction("ManageUsers");
        }

        // GET: Admin/SendNotification
        public ActionResult SendNotification()
        {
            return View();
        }

        // POST: Admin/SendNotification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendNotification([Bind(Include = "Message")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.DateSent = DateTime.Now;
                // UserId is left null so it broadcasts to everyone
                
                db.Notifications.Add(notification);
                db.SaveChanges();
                
                TempData["SuccessMessage"] = "Notification sent successfully!";
                return RedirectToAction("Dashboard");
            }
            return View(notification);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}