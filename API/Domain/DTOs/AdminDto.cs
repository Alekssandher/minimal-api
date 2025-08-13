using System.ComponentModel.DataAnnotations;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;

namespace minimal_api.Domain.DTOs
{
    public class AdminDto
    {
        [EmailAddress]
        [Required]
        [StringLength(200)]
        public string Email { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Password { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public Profile Profile { get; set; } = default!;

        public Admin ToAdmin()
        {
            return new Admin
            {
                Email = Email,
                Password = Password,
                Profile = Profile.ToString()
            };
        }
    }
}