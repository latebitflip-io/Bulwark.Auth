using Bulwark.Repositories;
using Bulwark.Repositories.Util;
using dotenv.net;
using FluentEmail.MailKitSmtp;
using System;
using System.IO;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Social;
using Bulwark.Auth.Core.Social.Validators;
using Bulwark.Auth.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

//Inject
var builder = WebApplication.CreateBuilder(args);
DotEnv.Load(options: new DotEnvOptions(overwriteExistingVars: false));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "bulwark-auth", Version = "v1" });
});

builder.Services
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

builder.Services.AddSingleton<IMongoClient>(
    mongoClient);

var dbName="bulwark-auth";
if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_SEED")))
{
    dbName = $"{dbName}-{Environment.GetEnvironmentVariable("DB_SEED")}";
}

builder.Services.AddSingleton(mongoClient.GetDatabase(dbName));
builder.Services.AddTransient<ITokenRepository, MongoDbAuthToken>();
builder.Services.AddTransient<ICertRepository, MongoDbCert>();
builder.Services.AddTransient<IEncrypt, BulwarkBCrypt>();
builder.Services.AddSingleton<ICertManager, CertManager>();
builder.Services.AddTransient<IAccountRepository, MongoDbAccount>();
builder.Services.AddTransient<IAccountManager, AccountManager>();
builder.Services.AddTransient<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
builder.Services.AddTransient<IMagicCodeManager, MagicCodeManager>();
builder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
builder.Services.AddTransient<IAuthorizationRepository, MongoDbAuthorization>();

//social startup
var googleValidator = new GoogleValidator(Environment
    .GetEnvironmentVariable("GOOGLE_CLIENT_ID"));
var socialValidators = new ValidatorStrategies();
socialValidators.Add(googleValidator);
builder.Services.AddSingleton<IValidatorStrategies>(socialValidators);
builder.Services.AddTransient<ISocialManager, SocialManager>();
//end of social startup
//end of Inject

//config
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error-development");
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "bulwark-auth v1"));
}
else
{
    app.UseExceptionHandler("/error");
}

//app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthorization();
app.MapControllers();
app.Run();
//end of config
