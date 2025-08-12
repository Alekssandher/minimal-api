using System.ComponentModel.DataAnnotations;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.DTOs
{
    public class VehicleDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;

        [StringLength(100)]
        public string Brand { get; set; } = default!;

        [Required]
        public int Year { get; set; } = default!;

        public Vehicle ToEntity()
        {
            return new Vehicle
            {
                Name = Name,
                Brand = Brand,
                Year = Year
            };
            
        }
    }
}