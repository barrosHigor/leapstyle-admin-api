using BackendGestaoEasy.Configuracoes;
using leap_dashboard_admin_api;
using leap_dashboard_admin_api.Configuracoes;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var _version = typeof(Program).Assembly.GetName().Version.ToString();

// add services to DI container
{   
    var services = builder.Services;
    services.AddCors();
    services.AddOptions();
    services.AddHttpContextAccessor();
    services.AddResponseCompression();
    services.AddControllers();
    services.AddResponseCompression();

    new Inject(services);

    services.AddSwaggerGen(c =>
    {
        c.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "LeapStyle", Version = _version });
        c.CustomSchemaIds(type => type.ToString());
    });
}

var app = builder.Build();

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseRouting();

    app.UseResponseCompression();
    app.UseHttpsRedirection();

    // custom basic auth middleware
    app.UseMiddleware<BasicAuthMiddleware>();

    app.MapControllers();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", $"LeapStyle {_version}");
    });
}

app.Run();