using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GerenciadorUsuarios.Api.Models;
using GerenciadorUsuarios.IntegrationTests.Factories;

namespace GerenciadorUsuarios.IntegrationTests.Controllers;

public class UsuariosControllerTest : IClassFixture<TestWebApplicationFactory>
{       
    private readonly TestWebApplicationFactory _webApplicationFactory;

    public UsuariosControllerTest(TestWebApplicationFactory webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }


    // given when then
    [Fact]
    public async Task DadoBuscarUsuarios_QuandoRequisitadoAPesquisa_EntaoDevolverListaUsuario()
    {
        // Arrange
        HttpClient client = _webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync("api/v1/usuarios");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var usuarios = await response.Content.ReadFromJsonAsync<IEnumerable<Usuario>>();
        usuarios.Should().NotBeEmpty();
        var usuarioParaComparacao = usuarios.ElementAt(0);
        usuarioParaComparacao.Nome.Should().Be("Usuario 01");
        usuarioParaComparacao.Email.Should().Be("usuario01@email.com");
    }
}

