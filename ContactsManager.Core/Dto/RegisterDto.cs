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
        public required string Email { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$")]
        [DataType(DataType.PhoneNumber)]
        public required string Phone { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
