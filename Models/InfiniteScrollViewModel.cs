using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMC_Donation.Models
{
    public class InfiniteScrollViewModel
    {
        public List<ItemsWithUserViewModel> ItemsWithUserDetails { get; set; }
        public bool HasMoreItems { get; set; }
    }
}