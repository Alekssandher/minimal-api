using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using minimal_api;
using minimal_api.Domain.Interfaces;
using Test.Mocks;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5002";
        public static TestContext _testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        public static void ClassInit(TestContext testContext)
        {
            _testContext = testContext;
            http = new WebApplicationFactory<Startup>();

            http = http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", PORT).UseEnvironment("Testing");
                
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdminService, AdminServiceMock>();
                });

            });

            client = http.CreateClient();
        }

        public static void ClassCleanup()
        {
            http.Dispose();
        }
    }
}