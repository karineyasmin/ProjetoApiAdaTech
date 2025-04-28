using FluentAssertions;
using GerenciadorUsuarios.Api.DTOs;
using GerenciadorUsuarios.Api.Models;

namespace GerenciadorUsuarios.UnitTests.DTOs;

public class CadastrarUsuarioDtoTest
{
    [Fact (DisplayName = "Testa conversão do modelo dto para o Usuário")]
    public void DadoConverterParaModelo_QuandoSolicitadaUmaConversao_EntaoConverterParaModeloUsuario() {
    
    // Arrange
    CadastrarUsuarioDto dto = new() {
        Nome = "Usuario 01",
        Email = "usuario01@gmail.com"
    };
    // Act
    Usuario usuario = dto.ConverterParaModelo();

    // Assert
    usuario.Nome.Should().Be(dto.Nome);
    usuario.Email.Should().Be(dto.Email);
    usuario.Id.Should().NotBeEmpty();

    }
}
