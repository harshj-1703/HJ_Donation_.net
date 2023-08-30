using System;
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
                    return RedirectToAction("Index", "Home");
                }
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

                    string targetDirectory = Server.MapPath("~/Uploads/ProfilePhotos");

                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string filePath = Path.Combine(targetDirectory, uniqueFileName);
                    profilePhotoFile.SaveAs(filePath);
                    userinfo.profilephoto = filePath;

                }
                else
                {
                    userinfo.profilephoto = Server.MapPath("~/Uploads/ProfilePhotos/NullImages/status1.png");
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
    }
}