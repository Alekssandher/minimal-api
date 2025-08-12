using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.ModelViews;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(LoginDto loginDto);

        void Include(Admin admin);

        List<AdminModelView> All(int? page);

        AdminModelView FindById(int id);
    }
}