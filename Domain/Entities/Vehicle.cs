using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using minimal_api.Domain.DTOs;

namespace minimal_api.Domain.Entities
{
    public class Vehicle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;

        [StringLength(100)]
        public string Brand { get; set; } = default!;

        [Required]
        public int Year { get; set; } = default!;


        public void Update(VehicleDto dto)
        {
            Name = dto.Name;
            Brand = dto.Brand;
            Year = dto.Year;
        }
    }
}