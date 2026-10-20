using System.Web.Mvc;
using System.Web.Routing;

namespace webProgramming
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // 1. Triggers your DbInitializer to build the SQL Database and Admin account
            System.Data.Entity.Database.SetInitializer<webProgramming.Models.ApplicationDbContext>(null);

            AreaRegistration.RegisterAllAreas();
            
            // 2. Defines the routing (Defaults to Facility/Index)
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Facility", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}