using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMC_Donation.Models;
using System.Linq;
using System.Configuration;
using System.Net;
using System.IO;

namespace RMC_Donation.Controllers
{
    public class ItemsController : Controller
    {
        public ActionResult Additems()
        {
            return View();
        }
    }
}