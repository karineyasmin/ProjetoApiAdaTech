using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GerenciadorUsuarios.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{   
    private const string mensagemDeErro = "Um erro ocorreu, por favor, tente novamente.";
    public void OnException(ExceptionContext context)
    {   
        var erro = new
        {
            mensagem = mensagemDeErro
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new JsonResult(erro);
    }
}
