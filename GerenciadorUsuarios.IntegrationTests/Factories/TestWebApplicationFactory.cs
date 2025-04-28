using GerenciadorUsuarios.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorUsuarios.IntegrationTests.Factories;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(service =>
        {
            service.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
        });
    }
}
