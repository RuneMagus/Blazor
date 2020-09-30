using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared.Models
{
    public class AddUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "password must be atleast 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool GdprAgreement { get; set; }
    }
}
