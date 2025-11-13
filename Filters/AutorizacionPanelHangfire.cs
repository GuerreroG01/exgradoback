using Hangfire.Dashboard;

namespace ExGradoBack.Filters
{
    public class AutorizacionPanelHangfire : IDashboardAuthorizationFilter
    {
        //Para entorno docker permitir acceso al dashboard de hangfire
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}