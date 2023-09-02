//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMC_Donation.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class item
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string name { get; set; }
        [Required(ErrorMessage = "Catagory is required")]
        [MaxLength(255, ErrorMessage = "Catagory cannot exceed 255 characters")]
        public string catagory { get; set; }
        [Required(ErrorMessage = "Details is required")]
        [MaxLength(255, ErrorMessage = "Details cannot exceed 255 characters")]
        public string details { get; set; }
        public int user_id { get; set; }

        public string imageurl1 { get; set; }
        public string imageurl2 { get; set; }
        public string imageurl3 { get; set; }
        public string imageurl4 { get; set; }
        public Nullable<System.DateTime> createdat { get; set; }
        public Nullable<System.DateTime> updatedat { get; set; }
        public Nullable<int> status { get; set; }
    }
}