namespace GerenciadorUsuarios.Api.Models;


public class Usuario
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
}