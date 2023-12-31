﻿using RMC_Donation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Web.WebPages;

namespace RMC_Donation.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        rmcdonateItemsEntity itemsEntity = new rmcdonateItemsEntity();
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

            if (imageurl1 == null)
            {
                //ModelState.AddModelError("", "One image is required");
                return View();
            }

            if (imageurl1 != null && imageurl1.ContentLength > 0)
            {
                string originalFileName = Path.GetFileName(imageurl1.FileName);
                string fileExtension = Path.GetExtension(originalFileName);


                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View();
                }

                int maxFileSize = 4 * 1024 * 1024; // 4 MB
                if (imageurl1.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
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

                int maxFileSize = 4 * 1024 * 1024; // 4 MB
                if (imageurl2.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
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

                int maxFileSize = 4 * 1024 * 1024; // 4 MB
                if (imageurl3.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
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

                int maxFileSize = 4 * 1024 * 1024; // 4 MB
                if (imageurl4.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
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
            TempData["SuccessMessage"] = "Item Added successfully!";
            return RedirectToAction("MyItems");
        }

        public ActionResult ItemDetails(int itemId)
        {
            var itemDb = new rmcdonateItemsEntity();
            var itemDetails = itemDb.items.SingleOrDefault(item => item.id == itemId);

            if (itemDetails == null)
            {
                return HttpNotFound();
            }

            return View(itemDetails);
        }

        public ActionResult MyItems()
        {
            var itemDb = new rmcdonateItemsEntity();
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var user_id = (int)Session["user_id"];
            var itemDetails = itemDb.items
            .Where(item => item.user_id == user_id)
            .Where(item => item.status != 0)
            .OrderByDescending(item => item.createdat)
            .ToList();

            return View(itemDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemDeleteByUser(int itemId)
        {
            using (var db = new rmcdonateItemsEntity())
            {
                var item = db.items.Find(itemId);
                if (item == null)
                {
                    return RedirectToAction("MyItems");
                }

                if (Session["user_id"] == null)
                {
                    return RedirectToAction("Login", "Users");
                }

                if (item.user_id != (int)Session["user_id"])
                {
                    return RedirectToAction("Index", "Home");
                }
                try
                {
                    item.status = 0;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            // Log or print the validation error details
                            Console.WriteLine($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }
                }
            }
            return RedirectToAction("MyItems");
        }


        [HttpGet]
        public ActionResult ItemEditByUser(int itemId)
        {
            using (var db = new rmcdonateItemsEntity())
            {
                var item = db.items.Find(itemId);
                if (item == null)
                {
                    return RedirectToAction("MyItems");
                }

                if (Session["user_id"] == null)
                {
                    return RedirectToAction("Login", "Users");
                }

                if (item.user_id != (int)Session["user_id"])
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(item);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemEditByUser(item items, HttpPostedFileBase imageurl1, HttpPostedFileBase imageurl2, HttpPostedFileBase imageurl3, HttpPostedFileBase imageurl4)
        {
            using (var db = new rmcdonateItemsEntity())
            {
                var existingItem = db.items.Find(items.id);
                if (existingItem == null || existingItem.user_id != (int)Session["user_id"])
                {
                    return RedirectToAction("Index", "Home");
                }

                string existingImage1 = existingItem.imageurl1;
                string existingImage2 = existingItem.imageurl2;
                string existingImage3 = existingItem.imageurl3;
                string existingImage4 = existingItem.imageurl4;

                items.imageurl1 = existingImage1;
                items.imageurl2 = existingImage2;
                items.imageurl3 = existingImage3;
                items.imageurl4 = existingImage4;

                ModelState.Remove("imageurl1");

                // Check model state for validation errors
                if (!ModelState.IsValid)
                {
                    return View(items);
                }

                existingItem.updatedat = DateTime.Now;
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (imageurl1 != null && imageurl1.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(imageurl1.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);
                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(items);
                    }
                    int maxFileSize = 4 * 1024 * 1024; // 4 MB
                    if (imageurl1.ContentLength > maxFileSize)
                    {
                        ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
                        return View(items);
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
                    existingItem.imageurl1 = filePath;
                }

                if (imageurl2 != null && imageurl2.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(imageurl2.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(items);
                    }
                    int maxFileSize = 4 * 1024 * 1024; // 4 MB
                    if (imageurl2.ContentLength > maxFileSize)
                    {
                        ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
                        return View(items);
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
                    existingItem.imageurl2 = filePath;
                }

                if (imageurl3 != null && imageurl3.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(imageurl3.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(items);
                    }
                    int maxFileSize = 4 * 1024 * 1024; // 4 MB
                    if (imageurl3.ContentLength > maxFileSize)
                    {
                        ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
                        return View(items);
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
                    existingItem.imageurl3 = filePath;
                }

                if (imageurl4 != null && imageurl4.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(imageurl4.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(items);
                    }
                    int maxFileSize = 4 * 1024 * 1024; // 4 MB
                    if (imageurl4.ContentLength > maxFileSize)
                    {
                        ModelState.AddModelError("", "The profile photo size cannot exceed 4 MB.");
                        return View(items);
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
                    existingItem.imageurl4 = filePath;
                }

                existingItem.name = items.name;
                existingItem.details = items.details;
                existingItem.catagory = items.catagory;

                db.SaveChanges();
                TempData["SuccessMessage"] = "Item Edited successfully!";
                return RedirectToAction("MyItems");
            }
        }

        [HttpPost]
        public ActionResult DeleteImage(int id, int imageNumber)
        {
            try
            {
                var db = new rmcdonateItemsEntity();

                // Perform validation before making changes
                if (imageNumber < 1 || imageNumber > 4)
                {
                    return Json(new { success = false, error = "Invalid imageNumber" });
                }

                // No validation issues, proceed with the update
                string columnName = $"imageurl{imageNumber}";
                string sql = $"UPDATE items SET {columnName} = NULL WHERE id = {id}";
                db.Database.ExecuteSqlCommand(sql);

                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                // Create an empty list to store validation errors
                var validationErrorsList = new List<object>();

                Console.WriteLine("Exception: " + ex.Message);

                // Check for EntityValidationErrors
                if (ex is System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}");
                            Console.WriteLine($"Property: {validationError.PropertyName}");
                            Console.WriteLine($"Error: {validationError.ErrorMessage}");

                            // Add validation error details to the list
                            validationErrorsList.Add(new
                            {
                                Entity = validationErrors.Entry.Entity.GetType().Name,
                                Property = validationError.PropertyName,
                                ErrorMessage = validationError.ErrorMessage
                            });
                        }
                    }
                }

                // Create the JSON response object
                var responseObject = new
                {
                    success = false,
                    error = ex.Message,
                    validationErrors = validationErrorsList
                };

                return Json(responseObject);
            }
        }
    }
}