using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Service
{
    public class VehicleService : IVehicleService
    {
        private readonly MyDbContext _context;

        public VehicleService(MyDbContext context)
        {
            _context = context;
        }

        public List<Vehicle> All(int page = 1, string? name = null, string? brand = null)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v =>
                    v.Name.Contains(name)

                );
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v =>
                    v.Brand.Contains(brand)
                );
            }

            int pageSize = 10;

            query = query.Skip((page -1) * pageSize).Take(pageSize);
            
            return [.. query];
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Remove(vehicle);
            _context.SaveChanges();
        }

        public Vehicle FindById(int id)
        {
            return _context.Vehicles.Find(id) ?? throw new Exception("Not Found");
        }

        public void Include(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            return;
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }
    }
}