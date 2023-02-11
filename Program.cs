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
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();
LoadConfiguration(app);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI( c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Api Herbert");
    });
    Console.WriteLine("Estou em Dev");
}

app.Run();

void LoadConfiguration(WebApplication app)
{
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}
void ConfigureAuthentication(WebApplicationBuilder builder)
{
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
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample Api Herbert", Version = "v1" });
    });

    builder.Services.AddHttpClient<TestService<object>>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddMemoryCache();    
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