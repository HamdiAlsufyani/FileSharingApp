using FileSharingPractice.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice.Models
{
    public class LoginViewModels
    {
        [EmailAddress(ErrorMessageResourceName="Email",ErrorMessageResourceType =typeof(SharedResource))]
        [Required(ErrorMessageResourceName ="Required",ErrorMessageResourceType =typeof(SharedResource))]
        [Display(Name ="EmailLabel",ResourceType=typeof(SharedResource))]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name ="PasswordLabel",ResourceType=typeof(SharedResource))]
        public string Password { get; set; }
    }
    public class RegistrationViewModels
    {
      
        [EmailAddress(ErrorMessageResourceName="Email",ErrorMessageResourceType =typeof(SharedResource))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name="EmailLabel", ResourceType =typeof(SharedResource))]
        public string Email { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name = "PasswordLabel", ResourceType = typeof(SharedResource))]
        public string Password { get; set; }
        [Compare("Password")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name ="ConfirmPasswordLabel",ResourceType =typeof(SharedResource))]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessageResourceName ="Required",ErrorMessageResourceType =typeof(SharedResource))]
        [Display(Name = "FirstNameLabel", ResourceType = typeof(SharedResource))]
        public string FirstName { get;  set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name = "LastNameLabel", ResourceType = typeof(SharedResource))]
        public string LastName { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
        [Display(Name = "PasswordLabel", ResourceType = typeof(SharedResource))]
        public string CurentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string  ConfirmPassword { get; set; }
    }

    public class AddPasswordViewModel
    {
        [Required]
       [DataType(DataType.Password)]
        public string Password { get; set; }
    
    }
    public class ConfirmEmailViewModel
    {
        [Required]
        public string Token  { get; set; }
        [Required]
        public string UserId  { get; set; }
    }
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email  { get; set; }
    }
    public class veriefyPasswordViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
