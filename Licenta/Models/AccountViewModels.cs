using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage ="Introduceti adresa dumneavoastra de email")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Introduceti parola")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Tine-ma minte")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Prenumele este obligatoriu"), Display(Name ="Prenume")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu"), Display(Name = "Nume")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Introduceti un numar de telefon"), Display(Name = "Numar de telefon")]
        [RegularExpression(@"^(\+4|)?(07[0-8]{1}[0-9]{1}|02[0-9]{2}|03[0-9]{2}){1}?(\s|\.|\-)?([0-9]{3}(\s|\.|\-|)){2}$", ErrorMessage = "Formatul numarului de telefon este invalid")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Adresa")]
        public string Address { get; set; }

        [Display(Name = "Oras")]
        public string City { get; set; }

        [Required(ErrorMessage ="Emailul este obligatoriu")]
        [EmailAddress]
        [System.Web.Mvc.Remote("IsEmailAvailable", "Users", ErrorMessage = "Adresa de email introdusa este deja folosita")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Parola este obligatorie")]
        [StringLength(100, ErrorMessage = "{0} trebuie sa aiba cel putin {2} caractere lungime.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma parola")]
        [Compare("Password", ErrorMessage = "Parola introdusa pentru confirmare difera de parola noua.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} trebuie sa aiba minim {2} caractere lungime.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma Parola")]
        [Compare("Password", ErrorMessage = "Parola introdusa pentru confirmare difera de parola noua.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
