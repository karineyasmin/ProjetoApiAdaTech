using System.ComponentModel.DataAnnotations;
using GerenciadorUsuarios.Api.Models;

namespace GerenciadorUsuarios.Api.DTOs;

// uma vez criado nao pode ser alterado
public record CadastrarUsuarioDto
{   
    [Required]
    [MinLength(5)]
    public string Nome { get; init; }

    [Required]
    [EmailAddress]
    public string Email { get; init; }


    public Usuario ConverterParaModelo()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = Nome,
            Email = Email
        };
    }
}
