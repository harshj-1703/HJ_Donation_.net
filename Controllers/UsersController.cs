﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RMC_Donation.Models;
using System.Linq;
using System.Configuration;
using System.Net;
using System.IO;

namespace RMC_Donation.Controllers
{
    public class UsersController : Controller
    {
        rmcdonateEntities entity = new rmcdonateEntities();
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel credentials)
        {
            bool userExists = entity.users.Any(x => x.email == credentials.email && x.password == credentials.password);
            user u = entity.users.FirstOrDefault(x => x.email == credentials.email && x.password == credentials.password);
            if (userExists)
            {
                if(u.status == 0)
                {
                    ModelState.AddModelError("", "You were removed by Admin");
                    return View();
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(u.fullname, false);
                    var userProfile = new HttpCookie("userProfile", u.profilephoto);
                    Response.Cookies.Add(userProfile);
                    Session["user_id"] = u.id;
                    return RedirectToAction("Index", "Home"); 
                }
                /*else if (u.status == 1)
                {
                    FormsAuthentication.SetAuthCookie(u.fullname, false);
                    // Set the user role to "Admin"
                    var authTicket = new FormsAuthenticationTicket(1, u.fullname, DateTime.Now, DateTime.Now.AddMinutes(30), false, "Admin");
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Home");
                }
                else if (u.status == 2)
                {
                    FormsAuthentication.SetAuthCookie(u.fullname, false);
                    // Set the user role to "User"
                    var authTicket = new FormsAuthenticationTicket(1, u.fullname, DateTime.Now, DateTime.Now.AddMinutes(30), false, "User");
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);
                    return RedirectToAction("Index", "Home");
                }*/
            }

            ModelState.AddModelError("", "Username or password is wrong");
            return View();
        }

        [HttpPost]
        public ActionResult Signup(user userinfo, HttpPostedFileBase profilePhotoFile)
        {
            bool userExists = entity.users.Any(x => x.email == userinfo.email || x.mobile_no == userinfo.mobile_no);
            if (!userExists)
            {
                if (profilePhotoFile != null && profilePhotoFile.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(profilePhotoFile.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);

                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View();
                    }

                    string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;

                    string targetDirectory1 = Server.MapPath("~/Uploads/ProfilePhotos/");
                    string targetDirectory = "/Uploads/ProfilePhotos/";

                    if (!Directory.Exists(targetDirectory1))
                    {
                        Directory.CreateDirectory(targetDirectory1);
                    }

                    //string filePath = Path.Combine(targetDirectory, uniqueFileName);
                    string filePath = targetDirectory + uniqueFileName;
                    profilePhotoFile.SaveAs(targetDirectory1 + uniqueFileName);
                    userinfo.profilephoto = filePath;
                }
                else
                {
                    string targetDirectory1 = Server.MapPath("~/Uploads/ProfilePhotos/NullImages/");
                    if (!Directory.Exists(targetDirectory1))
                    {
                        Directory.CreateDirectory(targetDirectory1);
                    }
                    userinfo.profilephoto = "/Uploads/ProfilePhotos/NullImages/status1.png";
                }
                userinfo.createdat = DateTime.Now;
                userinfo.updatedat = DateTime.Now;
                userinfo.lastlogin = DateTime.Now;
                userinfo.status = 1;
                entity.users.Add(userinfo);
                entity.SaveChanges();
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "User Exists");
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserDetails(int userId)
        {
            var userDb = new rmcdonateEntities();
            var userDetails = userDb.users.SingleOrDefault(user => user.id == userId);

            if (userDetails == null)
            {
                return HttpNotFound();
            }

            return View(userDetails);
        }

    }
}