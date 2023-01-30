using Application.IAppServices;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Services;

namespace Application.AppServices
{
    public class DashboardAppServices: GenericServices<Dashboard>, IDashboardAppServices
    {
        private readonly IDashboardServices _dashboardServices;
        public DashboardAppServices(IDashboardServices dashboardServices): base(dashboardServices) => _dashboardServices = dashboardServices;
    }
}
