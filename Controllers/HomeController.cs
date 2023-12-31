﻿using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMC_Donation.CustomAttributes;
using System.Data.Entity;

namespace RMC_Donation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(string searchQuery, int page = 1, int pageSize = 5)
        {
            var sessionUserId = Session["user_id"] as int?;
            var notificationEntity = new rmcDonateNotificationEntity();

            using (var dbContext = new rmcdonateItemsEntity())
            {
                IQueryable<item> itemsQuery = dbContext.items
                        .Where(item => item.status != 0);

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    itemsQuery = itemsQuery.Where(item =>
                        item.name.Contains(searchQuery) ||
                        item.catagory.Contains(searchQuery));
                }

                var userDb = new rmcdonateEntities();

                if (sessionUserId != null)
                {
                    itemsQuery = itemsQuery.Where(item => item.user_id != sessionUserId.Value);
                    var users1 = userDb.users
                    .Where(user => user.id == sessionUserId).FirstOrDefaultAsync();
                    ViewBag.UserName = users1.Result.fullname;
                    ViewBag.Mobileno = users1.Result.mobile_no;
                    ViewBag.Email = users1.Result.email;
                    DateTime dateOfBirth = users1.Result.dob;
                    ViewBag.DateOfBirth = dateOfBirth.ToString("MM-dd-yyyy");
                    ViewBag.Profession = users1.Result.profession;
                    ViewBag.Address = users1.Result.address;

                    var userItems = dbContext.items.Where(item => item.user_id == sessionUserId.Value).Where(item => item.status != 0).
                        OrderByDescending(item => item.createdat).Take(3).ToList();

                    var userNotifications = notificationEntity.notifications.Where(n => n.user_id == sessionUserId.Value).Where(n => n.status != 0).
                        OrderByDescending(n => n.createdat).Take(10).ToList();

                    ViewBag.userItems = userItems;
                    ViewBag.userNotifications = userNotifications;
                }

                var items = itemsQuery
                    .OrderByDescending(item => item.createdat)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

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

                int totalCount = itemsQuery.Count();

                ViewBag.page = page;
                ViewBag.pageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.SearchQuery = searchQuery;

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