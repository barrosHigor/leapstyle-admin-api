using Application.IAppServices;
using BackendGestaoEasy.Configuracoes;
using DataAccess.Utils;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Threading.Tasks;
using AuthorizeAttribute = leap_dashboard_admin_api.Configuracoes.AuthorizeAttribute;

namespace leap_dashboard_admin_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardAppServices _DashboardAppServices;
        public DashboardController(IDashboardAppServices DashboardAppServices) 
        {
            _DashboardAppServices = DashboardAppServices;
        }
        
        [HttpGet("{skip:int}/{take:int}/{filtrosStr?}")]
        [SwaggerOperationFilter(typeof(ReApplyOptionalRouteParameterOperationFilter))]
        public async Task<IActionResult> GetAsync(int skip = 0, int take = 100, string? filtrosStr = null)
        {
            try
            {
                if (take > 1000)
                    return BadRequest($"O valor de retorno {take} é muito grande e pode travar o banco de dados!");

                FiltroAPI? filtro = filtrosStr != null && filtrosStr != "," ? JsonSerializer.Deserialize<FiltroAPI>(filtrosStr) : null;
                var data = await _DashboardAppServices.SelectAsync(skip, take, filtro);

                return Ok(data);
            }
            catch (System.Exception ex)
            {
                Utils.salvaLog(ex, "Dashboard -> GetAsync");
                return StatusCode(500, new ResultAPI()
                {
                    errors = ex.Message
                });
            }
        }
    }
}
