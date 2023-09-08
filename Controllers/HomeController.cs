using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMC_Donation.CustomAttributes;

namespace RMC_Donation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(int page = 1, int pageSize = 5)
        {
            using (var dbContext = new rmcdonateItemsEntity())
            {
                var items = dbContext.items
                    .Where(item => item.status != 0)
                    .OrderByDescending(item => item.createdat)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var userDb = new rmcdonateEntities();

                var users = userDb.users
                    .Where(user => user.status != 0)
                    .ToList();

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

                int totalCount = dbContext.items.Count(item => item.status != 0);
                ViewBag.TotalCount = totalCount;

                ViewBag.page = page;
                ViewBag.pageSize = pageSize;
                ViewBag.TotalCount = totalCount;

                return View(itemsWithUserDetails);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //[CustomAuthorize("Admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}