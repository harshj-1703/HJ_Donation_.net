using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RMC_Donation.Models;
using System.Configuration;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using System.Data.Entity.Validation;
using System.Net.Mail;
using System.Data.SqlClient;

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

        public ActionResult ForgotPassword()
        { 
            return View(); 
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("email", "Email is required.");
            }
            else if (!IsValidEmail(email))
            {
                ModelState.AddModelError("email", "Invalid email format.");
            }
            else if (!IsEmailInDatabase(email))
            {
                ModelState.AddModelError("email", "Email does not exist/registered.");
            }

            if (ModelState.IsValid)
            {
                string to = email;
                string from = "harshj0107@gmail.com";
                string newPassword = GenerateRandomPassword(15);
                MailMessage message = new MailMessage(from, to);

                string mailbody = "Your New Password is : "+newPassword;
                message.Subject = "Forgot Password Reset";
                message.Body = mailbody;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;

                System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("harshj0107@gmail.com", "olex pmno fojy hvbc");

                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;

                try
                {
                    client.Send(message);
                    //update password
                    var rmcUsers = new rmcdonateEntities();
                    var user = rmcUsers.users.FirstOrDefault(u => u.email == email);
                    user.password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    rmcUsers.SaveChanges();
                    //message send
                    TempData["SuccessMessage"] = "Reseted Password To Your Email Sent Successfully!";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return View();
        }

        private string GenerateRandomPassword(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
            Random random = new Random();
            var password = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsEmailInDatabase(string email)
        {
            var dbContext = new rmcdonateEntities();
            var user = dbContext.users.SingleOrDefault(u => u.email == email);

            return user != null;
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

        /*[HttpPost]
        public ActionResult Signup(user userinfo, HttpPostedFileBase profilePhotoFile, string confirmPassword)
        {
            bool userExists = entity.users.Any(x => x.email == userinfo.email || x.mobile_no == userinfo.mobile_no);
            if (!userExists && IsValidEmail(userinfo.email))
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
                TempData["SuccessMessage"] = "User Created Verification Email Sent Successfully!";
                return RedirectToAction("Login");
            }
            else if(!userExists && !IsValidEmail(userinfo.email))
            {
                ModelState.AddModelError("", "Email is not valid");
                return View();
            }
            ModelState.AddModelError("", "User Exists");
            return View();
        }*/

        [HttpPost]
        public ActionResult Signup(user userinfo, HttpPostedFileBase profilePhotoFile, string confirmPassword)
        {
            bool userExists = entity.users.Any(x => x.email == userinfo.email || x.mobile_no == userinfo.mobile_no);
            if (!userExists && IsValidEmail(userinfo.email))
            {
                if (userinfo.password.Length < 8 || userinfo.password.Length > 15)
                {
                    ModelState.AddModelError("", "Password must be between 8 and 15 characters.");
                    return View();
                }
                if (userinfo.password != confirmPassword)
                {
                    ModelState.AddModelError("", "Password and Confirm Password must be the same.");
                    return View();
                }

                // Generate a verification token
                string verificationToken = GenerateVerificationToken();

                // Store the verification token in the user's model
                userinfo.VerificationToken = verificationToken;

                if (SendVerificationEmail(userinfo.email, verificationToken))
                {
                    if (profilePhotoFile != null && profilePhotoFile.ContentLength > 0)
                    {
                        userinfo.profilephoto = SaveProfilePhoto(profilePhotoFile);
                    }
                    else
                    {
                        userinfo.profilephoto = "/Uploads/ProfilePhotos/NullImages/status1.png";
                    }

                    userinfo.createdat = DateTime.Now;
                    userinfo.updatedat = DateTime.Now;
                    userinfo.lastlogin = DateTime.Now;

                    // Set the status to "0" (unverified) initially
                    userinfo.status = 0;
                    userinfo.password = BCrypt.Net.BCrypt.HashPassword(userinfo.password);

                    entity.users.Add(userinfo);
                    entity.SaveChanges();
                    using (var context = new rmcdonateEntities())
                    {
                        context.Database.ExecuteSqlCommand(
                            "UPDATE users SET VerificationToken = @VerificationToken WHERE email = @UserEmail",
                            new SqlParameter("VerificationToken", verificationToken),
                            new SqlParameter("UserEmail", userinfo.email)
                        );
                    }

                    TempData["SuccessMessage"] = "User Created. Verification Email Sent Successfully!";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to send the verification email.");
                }
            }
            else if (!userExists && !IsValidEmail(userinfo.email))
            {
                ModelState.AddModelError("", "Email is not valid");
            }
            else
            {
                ModelState.AddModelError("", "User already exists");
            }
            return View();
        }

        // Helper function to generate a verification token
        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        // Helper function to send the verification email
        private bool SendVerificationEmail(string userEmail, string verificationToken)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient client = new SmtpClient();

                client.Host = "smtp.gmail.com"; // Replace with your SMTP server
                client.Port = 587; // Replace with your SMTP server port
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("harshj0107@gmail.com", "olex pmno fojy hvbc");
                client.EnableSsl = true;

                message.From = new MailAddress("harshj0107@gmail.com"); // Replace with your email
                message.To.Add(userEmail);
                message.Subject = "Email Verification";

                // Construct the verification link (modify the URL as needed)
                string verificationLink = $"{Request.Url.Scheme}://{Request.Url.Authority}/Users/Verify?token={verificationToken}";

                message.Body = $"Click this link to verify your email: {verificationLink}";
                message.IsBodyHtml = true;

                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Helper function to save the profile photo
        private string SaveProfilePhoto(HttpPostedFileBase profilePhotoFile)
        {
            string originalFileName = Path.GetFileName(profilePhotoFile.FileName);
            string fileExtension = Path.GetExtension(originalFileName);

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
            {
                return null; // You can choose an appropriate fallback image
            }

            string uniqueFileName = Guid.NewGuid().ToString("N") + fileExtension;
            string targetDirectory1 = Server.MapPath("~/Uploads/ProfilePhotos/");
            string targetDirectory = "/Uploads/ProfilePhotos/";

            if (!Directory.Exists(targetDirectory1))
            {
                Directory.CreateDirectory(targetDirectory1);
            }

            string filePath = targetDirectory + uniqueFileName;
            profilePhotoFile.SaveAs(targetDirectory1 + uniqueFileName);

            return filePath;
        }

        // Your Verify method to handle email verification
        [HttpGet]
        public ActionResult Verify(string token)
        {
            // Construct a SQL query to find the user by VerificationToken
            string findUserQuery = "SELECT * FROM users WHERE VerificationToken = @Token";
            var user = entity.Database.SqlQuery<user>(findUserQuery, new SqlParameter("Token", token)).FirstOrDefault();

            if (user != null)
            {
                // Update the user's properties using a SQL command
                string updateQuery = "UPDATE users SET VerificationToken = NULL, status = 1 WHERE id = @UserId";
                entity.Database.ExecuteSqlCommand(updateQuery, new SqlParameter("UserId", user.id));

                TempData["SuccessMessage"] = "Email Verified Successfully!";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid verification token.";
                return RedirectToAction("Login");
            }
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
        }
    }
}