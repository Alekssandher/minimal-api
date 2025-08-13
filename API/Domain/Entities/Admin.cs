using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Domain.Entities
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [Required]
        [StringLength(255)]
        public string Email { get; init; } = default!;

        [Required]
        [StringLength(50)]
        public string Password { get; init; } = default!;

        [Required]
        [StringLength(10)]
        public string Profile { get; init; } = default!;
    }
}