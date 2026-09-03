using System.Web;
using System.Web.Http;

namespace Escola.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(config =>
            {
                Startup.ConfigureHttp(config);
            });
        }
    }
}
