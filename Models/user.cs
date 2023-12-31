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

    public partial class user
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string fullname { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime dob { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
        public string address { get; set; }

        [Required(ErrorMessage = "Professor is required")]
        [MaxLength(255, ErrorMessage = "Profession cannot exceed 255 characters")]
        public string profession { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

        //[MinLength(8, ErrorMessage = "Password must be between 8 to 15 characters")]
        //[MaxLength(15, ErrorMessage = "Password must be between 8 to 15 characters")]
        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }

        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Mobile Number must be exactly 10 digits and contain only digits.")]
        public string mobile_no { get; set; }
        public string profilephoto { get; set; }
        public Nullable<System.DateTime> createdat { get; set; }
        public Nullable<System.DateTime> updatedat { get; set; }
        public Nullable<System.DateTime> lastlogin { get; set; }
        public Nullable<int> status { get; set; }
        public string VerificationToken { get; set; }
    }
}
