using RMC_Donation.CustomAttributes;
using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RMC_Donation.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        public ActionResult ManageItems()
        {
            using (var dbContext = new rmcdonateItemsEntity())
            {
                var items = dbContext.items.OrderByDescending(item => item.createdat).ToList();
                var userDb = new rmcdonateEntities();

                var users = userDb.users.Where(user => user.status != 0).ToList();

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

        public ActionResult ManageUsers()
        {
            using (var dbContext = new rmcdonateEntities())
            {
                var users = dbContext.users.OrderByDescending(user => user.createdat).ToList();
                return View(users);
            }
        }

        [HttpPost]
        public ActionResult UpdateItemStatus(int id, bool isChecked)
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
            return RedirectToAction("ManageItems");
        }

        [HttpPost]
        public ActionResult UpdateUserStatus(int id, bool isChecked)
        {
            using (var db = new rmcdonateEntities())
            {
                var user = db.users.Find(id);

                if (user != null)
                {
                    user.status = isChecked ? 1 : 0;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ManageUsers");
        }
    }
}