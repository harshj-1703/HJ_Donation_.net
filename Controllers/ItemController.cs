using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net;

namespace RMC_Donation.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        rmcdonateItemsEntity itemsEntity = new rmcdonateItemsEntity();
        // GET: Item
        public ActionResult Additems()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Additems(item items, HttpPostedFileBase imageurl1, HttpPostedFileBase imageurl2, HttpPostedFileBase imageurl3, HttpPostedFileBase imageurl4)
        {
            items.createdat = DateTime.Now;
            items.updatedat = DateTime.Now;
            items.status = 1;
            items.user_id = (int)Session["user_id"];

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            if (imageurl1 != null && imageurl1.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(imageurl1.FileName);
                string fileExtension = Path.GetExtension(originalFileName);


                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View();
                }

                string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;

                string targetDirectory1 = Server.MapPath("~/Uploads/ItemPhotos/");
                string targetDirectory = "/Uploads/ItemPhotos/";

                if (!Directory.Exists(targetDirectory1))
                {
                    Directory.CreateDirectory(targetDirectory1);
                }

                string filePath = targetDirectory + uniqueFileName;
                imageurl1.SaveAs(targetDirectory1 + uniqueFileName);
                items.imageurl1 = filePath;
            }

            if (imageurl2 != null && imageurl2.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(imageurl2.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View();
                }

                string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;

                string targetDirectory1 = Server.MapPath("~/Uploads/ItemPhotos/");
                string targetDirectory = "/Uploads/ItemPhotos/";

                if (!Directory.Exists(targetDirectory1))
                {
                    Directory.CreateDirectory(targetDirectory1);
                }

                string filePath = targetDirectory + uniqueFileName;
                imageurl2.SaveAs(targetDirectory1 + uniqueFileName);
                items.imageurl2 = filePath;
            }

            if (imageurl3 != null && imageurl3.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(imageurl3.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View();
                }

                string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;

                string targetDirectory1 = Server.MapPath("~/Uploads/ItemPhotos/");
                string targetDirectory = "/Uploads/ItemPhotos/";

                if (!Directory.Exists(targetDirectory1))
                {
                    Directory.CreateDirectory(targetDirectory1);
                }

                string filePath = targetDirectory + uniqueFileName;
                imageurl3.SaveAs(targetDirectory1 + uniqueFileName);
                items.imageurl3 = filePath;
            }

            if (imageurl4 != null && imageurl4.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(imageurl4.FileName);
                string fileExtension = Path.GetExtension(originalFileName);

                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View();
                }

                string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;

                string targetDirectory1 = Server.MapPath("~/Uploads/ItemPhotos/");
                string targetDirectory = "/Uploads/ItemPhotos/";

                if (!Directory.Exists(targetDirectory1))
                {
                    Directory.CreateDirectory(targetDirectory1);
                }

                string filePath = targetDirectory + uniqueFileName;
                imageurl4.SaveAs(targetDirectory1 + uniqueFileName);
                items.imageurl4 = filePath;
            }

            itemsEntity.items.Add(items);
            itemsEntity.SaveChanges();
            return View();
        }

        public ActionResult ItemDetails(int itemId)
        {
            using (var itemDb = new rmcdonateItemsEntity())
            using (var userDb = new rmcdonateEntities())
            {
                var itemWithUserDetails = itemDb.items
                    .Where(item => item.id == itemId)
                    .Join(
                        userDb.users,
                        item => item.user_id,
                        user => user.id,
                        (item, user) => new ItemsWithUserViewModel()
                        {
                            Item = item,
                            User = user
                        })
                    .SingleOrDefault();

                if (itemWithUserDetails == null)
                {
                    return HttpNotFound();
                }

                return View(itemWithUserDetails);
            }
        }
    }
}