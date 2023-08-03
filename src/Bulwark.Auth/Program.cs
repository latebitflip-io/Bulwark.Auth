using dotenv.net;
using FluentEmail.MailKitSmtp;
using System;
using System.IO;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Social;
using Bulwark.Auth.Core.Social.Validators;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

//Inject
var applicationBuilder = WebApplication.CreateBuilder(args);
DotEnv.Load(options: new DotEnvOptions(overwriteExistingVars: false));

applicationBuilder.Logging.ClearProviders();
applicationBuilder.Logging.AddConsole();
applicationBuilder.Services.AddControllers();

applicationBuilder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

applicationBuilder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bulwark.Auth", Version = "v1" });
});

applicationBuilder.Services
    .AddFluentEmail(Environment.GetEnvironmentVariable("EMAIL_SEND_ADDRESS")?.Trim())
    .AddRazorRenderer(Directory.GetCurrentDirectory())
    .AddMailKitSender(new SmtpClientOptions
    {
        Server = Environment
            .GetEnvironmentVariable("EMAIL_SMTP_HOST"),
        Port = int.Parse(Environment
            .GetEnvironmentVariable("EMAIL_SMTP_PORT") ?? "1025"),
        UseSsl = bool.Parse(Environment
            .GetEnvironmentVariable("EMAIL_SMTP_SECURE") ?? "false"),
        User = Environment
            .GetEnvironmentVariable("EMAIL_SMTP_USER"),
        Password = Environment
            .GetEnvironmentVariable("EMAIL_SMTP_PASS"),
        RequiresAuthentication = false
    });
var mongoClient = new MongoClient(Environment
   .GetEnvironmentVariable("DB_CONNECTION"));

applicationBuilder.Services.AddSingleton<IMongoClient>(
    mongoClient);

var dbName="BulwarkAuth";
if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_SEED")))
{
    dbName = $"{dbName}-{Environment.GetEnvironmentVariable("DB_SEED")}";
}

applicationBuilder.Services.AddSingleton(mongoClient.GetDatabase(dbName));
applicationBuilder.Services.AddTransient<ITokenRepository, MongoDbAuthToken>();
applicationBuilder.Services.AddTransient<ISigningKeyRepository, MongoDbSigningKey>();
applicationBuilder.Services.AddTransient<IEncrypt, BulwarkBCrypt>();
applicationBuilder.Services.AddSingleton<ISigningKeyService, SigningKeyService>();
applicationBuilder.Services.AddTransient<IAccountRepository, MongoDbAccount>();
applicationBuilder.Services.AddTransient<IAccountService, AccountService>();
applicationBuilder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
applicationBuilder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
applicationBuilder.Services.AddTransient<IMagicCodeService, MagicCodeService>();
applicationBuilder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
applicationBuilder.Services.AddTransient<IAuthorizationRepository, MongoDbAuthorization>();

//social startup
var googleValidator = new GoogleValidator(Environment
    .GetEnvironmentVariable("GOOGLE_CLIENT_ID"));
var socialValidators = new ValidatorStrategies();
socialValidators.Add(googleValidator);
applicationBuilder.Services.AddSingleton<IValidatorStrategies>(socialValidators);
applicationBuilder.Services.AddTransient<ISocialService, SocialService>();
//end of social startup
//end of Inject

//config
var webApplication = applicationBuilder.Build();

if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseExceptionHandler("/error-development");
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "Bulwark.Auth v1"));
}
else
{
    webApplication.UseExceptionHandler("/error");
}

webApplication.UseRouting();
webApplication.MapControllers();
webApplication.Run();
//end of config
