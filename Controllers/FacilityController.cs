using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    [Authorize] // Requires users to be logged in to see this page
    public class FacilityController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Facility (Shows the search page to both Users and Admins)
        [AllowAnonymous] // Anyone can see the list of facilities even before logging in (optional, but good for UX)
        public ActionResult Index()
        {
            var facilities = db.Facilities.ToList();
            return View(facilities);
        }

        // GET: Facility/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Facility facility = db.Facilities.Find(id);
            if (facility == null) return HttpNotFound();
            
            return View(facility);
        }

        // GET: Facility/Create (ADMIN ONLY)
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Facility/Create (ADMIN ONLY)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,Type,Capacity")] Facility facility)
        {
            if (ModelState.IsValid)
            {
                db.Facilities.Add(facility);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facility);
        }

        // GET: Facility/Edit/5 (ADMIN ONLY)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Facility facility = db.Facilities.Find(id);
            if (facility == null) return HttpNotFound();
            
            return View(facility);
        }

        // POST: Facility/Edit/5 (ADMIN ONLY)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,Type,Capacity")] Facility facility)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facility).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facility);
        }

        // GET: Facility/Delete/5 (ADMIN ONLY)
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Facility facility = db.Facilities.Find(id);
            if (facility == null) return HttpNotFound();
            
            return View(facility);
        }

        // POST: Facility/Delete/5 (ADMIN ONLY)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Facility facility = db.Facilities.Find(id);
            db.Facilities.Remove(facility);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Facility/Notifications
        [Authorize]
        public ActionResult Notifications()
        {
            // Sisteme kayıtlı olan giriş yapmış kullanıcının ID'sini alıyoruz
            var userId = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);

            // Herkese gönderilen (UserId == null) veya sadece bu kullanıcıya gönderilen bildirimleri getir
            var notifications = db.Notifications
                .Where(n => n.UserId == null || n.UserId == userId)
                .OrderByDescending(n => n.DateSent)
                .ToList();

            return View(notifications);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}