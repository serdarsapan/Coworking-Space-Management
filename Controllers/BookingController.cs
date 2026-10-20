using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    [Authorize] // All booking actions require the user to be logged in
    public class BookingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Booking (My Booking History)
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var myBookings = db.Bookings
                .Include(b => b.Facility)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartTime)
                .ToList();

            return View(myBookings);
        }

        // GET: Booking/Create/5 (The ID is the FacilityId)
        public ActionResult Create(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var facility = db.Facilities.Find(id);
            if (facility == null) return HttpNotFound();

            ViewBag.Facility = facility;
            
            var booking = new Booking
            {
                FacilityId = facility.Id,
                StartTime = DateTime.Now.AddHours(1), // Default suggestion: 1 hour from now
                EndTime = DateTime.Now.AddHours(2)
            };

            return View(booking);
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FacilityId,StartTime,EndTime")] Booking booking)
        {
            // YENİ EKLENEN: Geçmiş tarihe rezervasyon engeli
            if (booking.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "Geçmiş bir tarihe veya saate rezervasyon yapamazsınız.");
            }

            // Custom Validation: End time must be after start time
            if (booking.StartTime >= booking.EndTime)
            {
                ModelState.AddModelError("EndTime", "Bitiş saati, başlangıç saatinden sonra olmalıdır.");
            }


            // Custom Validation: Check for double-booking
            bool isDoubleBooked = db.Bookings.Any(b => 
                b.FacilityId == booking.FacilityId &&
                b.Status != "Cancelled" &&
                b.StartTime < booking.EndTime && 
                b.EndTime > booking.StartTime);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "This facility is already booked during this time slot.");
            }

            if (ModelState.IsValid)
            {
                booking.UserId = User.Identity.GetUserId();
                booking.Status = "Reserved";
                
                db.Bookings.Add(booking);
                db.SaveChanges();
                
                return RedirectToAction("Index"); // Send them to their booking history
            }

            // If validation fails, reload the facility info for the view
            ViewBag.Facility = db.Facilities.Find(booking.FacilityId);
            return View(booking);
        }

        // POST: Booking/CheckIn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckIn(int id)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking != null && booking.Status == "Reserved")
            {
                booking.Status = "Checked-In";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Booking/CheckOut/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(int id)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking != null && booking.Status == "Checked-In")
            {
                booking.Status = "Checked-Out";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (booking != null && booking.Status == "Reserved")
            {
                booking.Status = "Cancelled";
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Booking/Calendar (Loads the HTML page for the calendar)
        public ActionResult Calendar()
        {
            return View();
        }

        // GET: Booking/GetEvents
        public JsonResult GetEvents()
        {
            // 1. Önce verileri veritabanından SQL ile çekip belleğe (RAM) alıyoruz
            var bookings = db.Bookings
                .Include(b => b.Facility) // Facility.Name'e erişebilmek için tabloyu dahil ediyoruz
                .Where(b => b.Status != "Cancelled")
                .ToList();

            // 2. Belleğe alınan veriler üzerinde C# metodlarını (ToString) güvenle çalıştırıyoruz
            var events = bookings.Select(b => new
            {
                id = b.Id,
                title = b.Facility.Name + " (" + b.Status + ")",
                start = b.StartTime.ToString("s"), // "s" ISO 8601 formatı
                end = b.EndTime.ToString("s"),
                color = b.Status == "Reserved" ? "#0d6efd" : (b.Status == "Checked-In" ? "#198754" : "#6c757d")
            })
            .ToList();

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}