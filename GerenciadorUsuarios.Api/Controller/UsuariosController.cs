using Microsoft.AspNetCore.Mvc;
using GerenciadorUsuarios.Api.Models;
using GerenciadorUsuarios.Api.DTOs;
using System.Net.Mime;
using GerenciadorUsuarios.Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.Extensions.Caching.Memory;

namespace GerenciadorUsuarios.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("/api/v{version:apiVersion}/usuarios")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController] // faz com que as regras de validacao sejam aplicadas
    [Authorize(Roles = "Admin")]    
    public class UsuariosController : ControllerBase
    {      
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMemoryCache _cache;

        public UsuariosController(IUsuarioRepository usuarioRepository, IMemoryCache cache)
        {
            _usuarioRepository = usuarioRepository;
            _cache = cache;
        }
       
       [HttpGet]
       [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<Usuario>))]
        public IActionResult BuscarUsuarios([FromQuery] string filtroNome = "")
        { 
            // colecao que nao sera alterada
            IEnumerable<Usuario> usuariosFiltrados = _usuarioRepository.ObterUsuarios().Where(x=> x.Nome.StartsWith(filtroNome, StringComparison.OrdinalIgnoreCase));
            return Ok(usuariosFiltrados);
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<Usuario>))]
        public async Task<IActionResult> BuscarUsuariosV2([FromQuery] string filtroNome = "")
        {   
            var usuarioViaCache = await _cache.GetOrCreateAsync(filtroNome, async operacao => {
                IEnumerable<Usuario> usuariosFiltrados = (await _usuarioRepository.ObterUsuariosAsync())
                .Where(x => x.Nome.StartsWith(filtroNome, StringComparison.OrdinalIgnoreCase));

                return usuariosFiltrados;
            });

            // colecao que nao sera alterada
            IEnumerable<Usuario> usuariosFiltrados = _usuarioRepository.ObterUsuarios().Where(x=> x.Nome.StartsWith(filtroNome, StringComparison.OrdinalIgnoreCase));
            return Ok(new {
                Itens = usuarioViaCache
            });
        }
 

        [HttpGet("{id:guid}", Name = nameof(BuscarPorId))]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type=typeof(Usuario))]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [Authorize("buscar-por-id")]
        public IActionResult BuscarPorId([FromRoute] Guid id)
        {
            Usuario usuario = _usuarioRepository.ObterUsuarios().FirstOrDefault(x => x.Id == id);
            if (usuario is not null)
            {
                return Ok(usuario);
            }

            return NotFound();
        }

        

        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, Type = typeof(Usuario))]
        public IActionResult CriarUsuario([FromBody] CadastrarUsuarioDto dto)
        {   
            Usuario usuario = dto.ConverterParaModelo();
            _usuarioRepository.ObterUsuarios().Add(usuario);
            return CreatedAtAction(nameof(BuscarPorId), new {usuario.Id}, usuario);
        }


        [HttpPatch("{id:guid}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
        public IActionResult AtualizarUsuario([FromRoute] Guid id, [FromBody] AtualizarUsuarioDto dto)
        {
            Usuario usuario = _usuarioRepository.ObterUsuarios().FirstOrDefault(x => x.Id == id);
            if (usuario is null)
            {
                return NotFound();
            }

            usuario.Nome = dto.Nome;
            return NoContent();

        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
        public IActionResult RemoverUsuario([FromRoute] Guid id)
        {
            Usuario usuario = _usuarioRepository.ObterUsuarios().FirstOrDefault(x => x.Id == id);
            if (usuario is null)
            {
                return NotFound();
            }

            _usuarioRepository.ObterUsuarios().Remove(usuario);
            return NoContent();

        }
    }
}