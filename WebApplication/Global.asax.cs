using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication.Models.Entities;
using WebApplication.Models;

namespace WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Seedowanie danych (uruchomienie, jeśli nie ma użytkowników)
            SeedData().Wait();
        }

        private async Task SeedData()
        {
            using (var context = new AppDbContext())
            {
                var userManager = new AppUserManager(new UserStore<User>(context));

                // Seedowanie danych użytkowników
                await Seed.SeedData(context, userManager);
            }
        }
    }
}
