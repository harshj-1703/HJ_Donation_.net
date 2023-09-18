using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMC_Donation.Models
{
    public class RequestItemWithUserAndItemViewModel
    {
        public requestitem RequestItem { get; set; }
        public user User { get; set; }
        public item Item { get; set; }
    }
}