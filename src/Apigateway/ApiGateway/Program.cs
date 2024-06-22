using System.Security.Cryptography;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var Symmetric = false;
// Symmetric
var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
var authenticationProviderKey = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

// Asymmetric
var rsa = RSA.Create();
var publicKey = File.ReadAllText("./public_key.pem");
rsa.ImportFromPem(publicKey);
var  authenticationProviderKeyAsymmetric = new SigningCredentials(
    key: new RsaSecurityKey(rsa),
    algorithm: SecurityAlgorithms.RsaSha256);





builder.Services
    .AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        options.RequireHttpsMetadata = false;
        options.UseSecurityTokenValidators = false;
        options.TokenValidationParameters.ValidateActor = false;
        options.TokenValidationParameters.ValidateIssuer = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.IssuerSigningKey = Symmetric ? authenticationProviderKey.Key : authenticationProviderKeyAsymmetric.Key;
        
        options.TokenValidationParameters.ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
    });


// Configure CORS
builder.Services
    .AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .WithOrigins(builder.Configuration.GetSection("CORS:Origins").Get<string[]>() ?? [])
                .AllowAnyMethod()
                .AllowCredentials()
                .AllowAnyHeader()
                .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
    });

// Configure health checks
builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    // Configure here Serilog instance...
    loggerConfiguration
        .MinimumLevel.Information()
        .Enrich.WithProperty("ApplicationContext", "Ocelot.APIGateway")
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Add Ocelot json file configuration
builder.Configuration.AddJsonFile("ocelot.json");

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(_ => { });

// Map health check endpoints
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.UseCors("CorsPolicy");

await app.UseOcelot();

await app.RunAsync();