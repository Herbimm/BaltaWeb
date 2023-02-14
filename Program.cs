using AutoMapper;
using BaltaWeb;
using BaltaWeb.Data;
using BaltaWeb.Interfaces;
using BaltaWeb.Mapper;
using BaltaWeb.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();
LoadConfiguration(app);

app.UseHttpsRedirection(); // HTTPS redirect
app.UseAuthentication(); // O usuário precisa ser logado para usar tais métodos marcados;
app.UseAuthorization(); // o usuário precisa de tais permissões.
app.UseStaticFiles(); // Adiciona a solução a possibilidade de guardar arquivos na APi como IMG ou outros, usar a pasta "wwwroot"
app.MapControllers();
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{    
    Console.WriteLine("Estou em Dev");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Api Herbert");
});

app.Run();

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

    //COnfig de EmAIL
    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}
void ConfigureAuthentication(WebApplicationBuilder builder)
{

    // [ApiKey] configura para o client poder fazer requisição usando uma ApiKey, a apikey vai no header da solicitação.
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
}
void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddCors();
    // para rodar o httpclient precisa disso
    builder.Services.AddHttpClient<TestService<object>>();

    builder.Services.AddEndpointsApiExplorer();
    #region Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        var desc = $"Sample Api Herbert <br />{new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime}";

        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
            Title = "Sample Api Herbert",
            Description = desc
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
                   "JWT Authorization header using the Bearer scheme." +
                   " \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "identity",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                 });
    });
    #endregion


    // trabalhar com mémoria precisa disso
    builder.Services.AddMemoryCache();

    // Comprime as respostas Json em um formato GzipCompression, existem outros, isso é utilizado para otimizar as respostas, mas só vale a pena usar
    //se os json forem realmente grandes.
    builder.Services.AddResponseCompression(options =>
    {
        // options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        // options.Providers.Add<CustomCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    }); 

    // Essa configuração Adiciona controllers na api e o JsonIgnore vai remover a validação padrão que antes do ModelState já retornar direto o erro
    // para o usuário, então se for adicionar essas linhas do código, é recomendavel usar validação com o ModelState.isValid
    builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }).AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    }); 
}
void ConfigureServices(WebApplicationBuilder builder)
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile <BlogMapperProfile>();
    });
    IMapper mapper = config.CreateMapper();

    builder.Services.AddDbContext<BlogDataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddTransient<TokenService>();
    builder.Services.AddScoped(typeof(ITesteService), typeof(TestService<object>));
    builder.Services.AddScoped<ISeniorAuthenticationService, SeniorAuthenticationService>();
    builder.Services.AddTransient<EmailService>();
    builder.Services.AddSingleton(mapper);
}