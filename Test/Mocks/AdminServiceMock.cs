using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace Test.Mocks
{
    public class AdminServiceMock
    {
         private static readonly List<Admin> Admins = [
            new Admin{
                Id = 1,
                Email = "adm@teste.com",
                Password = "123456",
                Profile = "Adm"
            },
            new Admin{
                Id = 2,
                Email = "editor@teste.com",
                Password = "123456",
                Profile = "Editor"
            }
        ];

        public static Admin? FindById(int id)
        {
            return Admins.Find(a => a.Id == id);
        }

        public static Admin Include(Admin Admin)
        {
            Admin.Id = Admins.Count + 1;
            Admins.Add(Admin);

            return Admin;
        }

        public static Admin? Login(LoginDto loginDTO)
        {
            return Admins.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        }

        public static List<Admin> All(int? pagina)
        {
            return Admins;
        }
    }
}