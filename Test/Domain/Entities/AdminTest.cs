using minimal_api.Domain.Entities;

namespace Test.Domain.Entities
{
    [TestClass]
    public class AdminTest
    {
        [TestMethod]
        public void GetSetTest()
        {
            var adm = new Admin
            {
                Id = 1,
                Email = "test@test.com",
                Password = "password123",
                Profile = "Editor"
            };

            Assert.AreEqual(1, adm.Id);
            
            Assert.AreEqual("test@test.com", adm.Email);
            Assert.AreEqual("password123", adm.Password);
            Assert.AreEqual("Editor", adm.Profile);
        }
    }
}