using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMC_Donation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        [AllowAnonymous]
        public ActionResult Index()
        {
            using (var dbContext = new rmcdonateItemsEntity())
            {
                //var items = dbContext.items.Where(item => item.status != 0).ToList();
                var items = dbContext.items.ToList();
                var userDb = new rmcdonateEntities();

                // Retrieve users from the second context
                var users = userDb.users.Where(user => user.status != 0).ToList();

                // Join items and users in memory using LINQ to Objects
                var itemsWithUserDetails = items
                    .Join(
                        users,
                        item => item.user_id,
                        user => user.id,
                        (item, user) => new ItemsWithUserViewModel()
                        {
                            Item = item,
                            User = user
                        })
                    .ToList();

                return View(itemsWithUserDetails);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, bool isChecked)
        {
            using (var db = new rmcdonateItemsEntity())
            {
                var item = db.items.Find(id);

                if (item != null)
                {
                    item.status = isChecked ? 1 : 0;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

    }
}