using System.Reflection;
using System.Text;
using Asp.Versioning;
using GerenciadorUsuarios.Api.Filters;
using GerenciadorUsuarios.Api.Repository;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddRateLimiter(_ => {
    _.AddFixedWindowLimiter("janela-fixa", options =>
    {
        options.QueueLimit = 5;
        options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        options.PermitLimit = 2;
        options.Window = TimeSpan.FromSeconds(5);
    });
});

builder.Services.AddAuthentication().AddJwtBearer(options => 
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = "usuarios-api",
        ValidAudience = "usuarios-api",
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("ChaveAutenticacao"))), // copia da key
        ClockSkew = TimeSpan.Zero // nao vamos aceitar tokens vencidos 
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("buscar-por-id", policy => policy.RequireClaim("ler-dados-por-id"));
});

builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>(); // registra a injecao de dependencia
// Add services to the container.
builder.Services.AddControllers(options => options.Filters.Add<ExceptionFilter>()); // adicionei o tratameto de erros aqui
builder.Services.AddEndpointsApiExplorer();

// adicionado infos ao swagger

var documentacao = new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "usuarioadmin@gmail.com",
            Name = "Equipe API",
            Url = new Uri("https://www.google.com.br"),
        },
        Description = "API destinada ao gerenciamento de usuários.",
        Title = "API - Gestão de Usuários",
    };

builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", documentacao);
    options.SwaggerDoc("v2", documentacao);



    // incluir comentarios de docs do sumary
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddApiVersioning(options => 
    {
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    }).AddApiExplorer(options => 
    {
        options.GroupNameFormat = "'v'V"; //formato versionamento
        options.SubstituteApiVersionInUrl = true;
    });

builder.Configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true); // carrego o arquivo secrets.json

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        foreach (var apiVersion in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint($"/swagger/{apiVersion.GroupName}/swagger.json", apiVersion.GroupName);
        }
    }); // adiciona os endpoints versionados no swagger
}

app.UseRateLimiter();

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
// app.UseAuthorization();    // Configuração de autorização (se necessário)

// Configurar para usar Controllers
app.MapControllers(); 


//Middleware
app.Use((httpContext, next) =>
{
    var logger = httpContext.RequestServices.GetService<ILogger<Program>>();
    logger.LogInformation("Requisição com o método {Metodo} para a rota {Rota}", 
    httpContext.Request.Method, 
    httpContext.Request.Path);

    return next();
});

app.Run();


public partial class Program { }