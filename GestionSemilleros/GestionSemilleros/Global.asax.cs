using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using GestionSemilleros.Models.DAO;

namespace GestionSemilleros
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<SemillerosContext>());

            using (var baseDatos = new SemillerosContext())
            {
                Database.SetInitializer<SemillerosContext>(null);
            }

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}