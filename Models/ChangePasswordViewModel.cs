using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMC_Donation.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "The current password is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "The new password is required.")]
        [MinLength(8, ErrorMessage = "The new password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm the new password.")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}