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
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using System.Data.Entity.Validation;

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
            var user = entity.users.FirstOrDefault(x => x.email == credentials.email);

            if (user != null && BCrypt.Net.BCrypt.Verify(credentials.password, user.password))
            {
                if (user.status == 1)
                {
                    FormsAuthentication.SetAuthCookie(user.fullname, false);
                    var userProfile = new HttpCookie("userProfile", user.profilephoto);
                    Response.Cookies.Add(userProfile);
                    Session["user_id"] = user.id;
                    Session["user_role"] = "User";
                    user.lastlogin = DateTime.Now;
                    entity.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else if (user.status == 2)
                {
                    FormsAuthentication.SetAuthCookie(user.fullname, false);
                    var userProfile = new HttpCookie("userProfile", user.profilephoto);
                    Response.Cookies.Add(userProfile);
                    Session["user_id"] = user.id;
                    Session["user_role"] = "Admin";
                    user.lastlogin = DateTime.Now;
                    entity.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "You were removed by Admin");
                    return View();
                }
            }

            ModelState.AddModelError("", "Username or password is wrong");
            return View();
        }

        [HttpPost]
        public ActionResult Signup(user userinfo, HttpPostedFileBase profilePhotoFile, string confirmPassword)
        {
            bool userExists = entity.users.Any(x => x.email == userinfo.email || x.mobile_no == userinfo.mobile_no);
            if (!userExists)
            {
                if (userinfo.password.Length < 8 || userinfo.password.Length > 15)
                {
                    ModelState.AddModelError("", "Password must be between 8 and 15 characters.");
                    return View();
                }
                if (userinfo.password != confirmPassword)
                {
                    ModelState.AddModelError("", "Password And Confirm Password Must Be Same.");
                    return View();
                }

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
                userinfo.password = BCrypt.Net.BCrypt.HashPassword(userinfo.password);
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
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [Authorize]
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

        [Authorize]
        //[HttpGet]
        public ActionResult EditUserProfile()
        {
            int userId = (int)Session["user_id"];
            using (var db = new rmcdonateEntities())
            {
                var user = db.users.Find(userId);
                if (user == null || Session["user_id"] == null)
                {
                    return RedirectToAction("Login");
                }

                if (user.id != (int)Session["user_id"])
                {
                    return RedirectToAction("Login");
                }
                return View(user);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUserProfile(user users, HttpPostedFileBase profileImage)
        {
            using (var db = new rmcdonateEntities())
            {
                var oldUser = db.users.Find(users.id);
                if (oldUser == null || Session["user_id"] == null || oldUser.id != (int)Session["user_id"])
                {
                    return RedirectToAction("Index", "Home");
                }

                users.profilephoto = oldUser.profilephoto;

                ModelState.Remove("email");
                ModelState.Remove("password");

                if (!ModelState.IsValid)
                {
                    List<string> modelErrors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            modelErrors.Add(error.ErrorMessage);
                        }
                    }
                    ViewBag.ModelErrors = modelErrors;
                    return View(users);
                }

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (profileImage != null && profileImage.ContentLength > 0)
                {
                    string originalFileName = Path.GetFileName(profileImage.FileName);
                    string fileExtension = Path.GetExtension(originalFileName);
                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        ModelState.AddModelError("", "Invalid file format. Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View(users);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;
                    string targetDirectory = "/Uploads/ProfilePhotos/";
                    string filePath = targetDirectory + uniqueFileName;
                    string targetDirectory1 = Server.MapPath("~/Uploads/ProfilePhotos/");
                    profileImage.SaveAs(targetDirectory1 + uniqueFileName);
                    oldUser.profilephoto = filePath;

                    var userProfile = new HttpCookie("userProfile", oldUser.profilephoto);
                    Response.Cookies.Add(userProfile);
                }

                oldUser.fullname = users.fullname;
                oldUser.profession = users.profession;
                oldUser.dob = users.dob;
                oldUser.address = users.address;
                oldUser.mobile_no = users.mobile_no;
                oldUser.updatedat = DateTime.Now;

                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(users.fullname, false);
                TempData["SuccessMessage"] = "Profile Edited successfully!";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteProfilePhoto(int userId)
        {
            try
            {
                var db = new rmcdonateEntities();

                if (userId <= 0)
                {
                    return Json(new { success = false, error = "Invalid user ID" });
                }

                var user = db.users.Find(userId);
                if (user != null)
                {
                    user.profilephoto = "/Uploads/ProfilePhotos/NullImages/status1.png";
                    db.SaveChanges();

                    var userProfile = new HttpCookie("userProfile", user.profilephoto);
                    Response.Cookies.Add(userProfile);

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "User not found" });
                }
            }
            catch (Exception ex)
            {
                var responseObject = new
                {
                    success = false,
                    error = ex.Message
                };

                return Json(responseObject);
            }
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("Login");
            }

            if(!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }

            int userId = (int)Session["user_id"];
            var user = entity.users.FirstOrDefault(x => x.id == userId);

            if (user != null && BCrypt.Net.BCrypt.Verify(changePasswordViewModel.CurrentPassword, user.password))
            {
                if (BCrypt.Net.BCrypt.Verify(changePasswordViewModel.NewPassword, user.password))
                {
                    ModelState.AddModelError("", "New password is same as old password!");
                    return View(changePasswordViewModel);
                }
                user.password = BCrypt.Net.BCrypt.HashPassword(changePasswordViewModel.NewPassword);
                entity.SaveChanges();
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Old Password is Wrong!");
                return View(changePasswordViewModel);
            }

            return View();
        }
    }
}