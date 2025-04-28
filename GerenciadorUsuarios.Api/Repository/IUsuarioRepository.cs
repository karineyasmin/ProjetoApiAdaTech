using GerenciadorUsuarios.Api.Models;

namespace GerenciadorUsuarios.Api.Repository;

public interface IUsuarioRepository
{
    List<Usuario> ObterUsuarios();
    Task<List<Usuario>> ObterUsuariosAsync();
}
