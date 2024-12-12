using ContactsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Dto
{
    public class RegisterDto
    {
        [Required]
        public required string PersonName { get; set; }
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Remote("IsRegisteredEmail", "Accounts", ErrorMessage = "Email is already used")]
        public required string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must only contain numbers")]
        [DataType(DataType.PhoneNumber)]
        public required string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Confirm password must match with password")]
        public required string ConfirmPassword { get; set; }
        public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;
    }
}
