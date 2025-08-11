using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
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

        public Admin? Login(LoginDto loginDto)
        {
            return _context.Admins.Where(a => a.Email == loginDto.Email && a.Password == loginDto.Password).FirstOrDefault();
             
        }
    }
}