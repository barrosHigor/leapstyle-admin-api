using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Services;

namespace Domain.Services
{
    public class DashboardServices: GenericServices<Dashboard>, IDashboardServices
    {
        private readonly IDashboardRepository _dashboardRepository;
        public DashboardServices(IDashboardRepository dashboardRepository): base(dashboardRepository) => _dashboardRepository = dashboardRepository;
    }
}
