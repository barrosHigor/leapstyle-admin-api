using Application.AppServices;
using Application.IAppServices;
using DataAccess.Repository;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Services;
using Domain.Services;

namespace leap_dashboard_admin_api
{
    public class Inject
    {
        private readonly IServiceCollection _services;

        public Inject(IServiceCollection services)
        {

            _services = services;

            _services.AddTransient<IDashboardAppServices, DashboardAppServices>();
            _services.AddTransient<IDashboardServices, DashboardServices>();
            _services.AddTransient<IDashboardRepository, DashboardRepository>();
        }
    }
}
