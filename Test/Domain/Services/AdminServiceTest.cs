using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Service;
using minimal_api.Infrastructure.Db;

namespace Test.Domain.Services
{
    [TestClass]
    public class AdminServiceTest
    {
        private MyDbContext CreateContext()
        {
            var path = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();


            return new MyDbContext(config);
        }
        [TestMethod]
        public void SavingAdmin()
        {
            var adm = new Admin
            {
                Id = 1,
                Email = "test@test.com",
                Password = "password123",
                Profile = "Editor"
            };

            var context = CreateContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");
            var adminService = new AdminService(context);

            adminService.Include(adm);
            var admRes = adminService.FindById(adm.Id);


            Assert.AreEqual(1, admRes.Id);

            Assert.AreEqual("test@test.com", adm.Email);
            Assert.AreEqual("password123", adm.Password);
            Assert.AreEqual("Editor", adm.Profile);
        }
    }
}