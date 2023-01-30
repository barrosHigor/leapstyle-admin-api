using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class DashboardRepository: GenericRepository<Dashboard>, IDashboardRepository
    {
        private readonly IDashboardRepository _dashboardRepository;

        private async Task<MelhorEnvioRequest> MelhorEnvio_GetRestAsync(string ComplementoUrl)
        {
            try
            {
                var urlMelhorEnvio = Environment.GetEnvironmentVariable("urlMelhorEnvio");
                var TokenMelhorEnvio = Environment.GetEnvironmentVariable("TokenMelhorEnvio");

                var client = new RestClient(urlMelhorEnvio + ComplementoUrl);
                client.Options.UserAgent = "oi@leapstyle.me";

                var request = new RestRequest(urlMelhorEnvio, Method.Get);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Bearer " + TokenMelhorEnvio);

                var response = await client.GetAsync(request);
                var obj = JsonConvert.DeserializeObject<MelhorEnvioRequest>(response.Content);

                return obj;
            }
            catch
            {
                return null;
            }
        }

        public override async Task<ResultAPI> SelectAsync(int skip, int take, FiltroAPI filtros)
        {
            try
            {
                var queryDados = "SELECT * FROM Dashboard";
                var dashboard = new Dashboard();

                using (var conexao = GetConexao())
                {
                    dashboard = conexao.Query<Dashboard>(queryDados).FirstOrDefault();
                }

                //var TotalSaldoMelhorEnvio = await MelhorEnvio_GetRestAsync("me/balance");
                //if (TotalSaldoMelhorEnvio != null)
                //    dashboard.TotalSaldoMelhorEnvio = 0;

                var result = new ResultAPI()
                {
                    total = 1,
                    skip = skip,
                    take = take,
                    data = dashboard
                };

                return result;
            }
            catch (Exception ex)
            {
                Utils.Utils.salvaLog(ex, "Dashboard -> GetDashboardByFiltro");
                throw;
            }
        }
    }
}
