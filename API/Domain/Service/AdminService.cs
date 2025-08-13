using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Service
{
    public class AdminService : IAdminService
    {
        private readonly MyDbContext _context;
        public AdminService(MyDbContext context)
        {
            _context = context;
        }

        public List<AdminModelView> All(int? page)
        {
            var query = _context.Admins.AsQueryable();
            int pageSize = 10;
            int pageNumber = page ?? 1;

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var result = query.Select(adm => new AdminModelView( adm.Id, adm.Email, adm.Profile) );
            return [..result];
        }

        public AdminModelView FindById(int id)
        {
            var result = _context.Admins.Find(id) ?? throw new Exception("Not Found");
            return new AdminModelView(result.Id, result.Email, result.Profile);
        }

        public void Include(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();

            return;
        }

        public Admin? Login(LoginDto loginDto)
        {
            return _context.Admins.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password).FirstOrDefault();
             
        }
    }
}