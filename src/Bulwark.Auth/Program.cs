using System.Collections.Generic;
using dotenv.net;
using FluentEmail.MailKitSmtp;
using System.IO;
using Bulwark.Auth;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.PasswordPolicy;
using Bulwark.Auth.Core.SigningAlgs;
using Bulwark.Auth.Core.Social;
using Bulwark.Auth.Core.Social.Validators;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.Templates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//trigger build: dependency upgrade 
var applicationBuilder = WebApplication.CreateBuilder(args);
DotEnv.Load(options: new DotEnvOptions(overwriteExistingVars: false));
//AppConfig must be initialized after DotEnv.Load for environment variables to be available
var appConfig = new AppConfig();

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
    .AddFluentEmail(appConfig.EmailFromAddress)
    .AddRazorRenderer(Directory.GetCurrentDirectory())
    .AddMailKitSender(new SmtpClientOptions
    {
        Server = appConfig.EmailSmtpHost,
        Port = appConfig.EmailSmtpPort,
        UseSsl = appConfig.EmailSmtpSecure,
        User = appConfig.EmailSmtpUser,
        Password = appConfig.EmailSmtpPass,
        RequiresAuthentication = false
    });
var mongoClient = new MongoClient(appConfig.DbConnection);

applicationBuilder.Services.AddSingleton<IMongoClient>(
    mongoClient);

var dbName="BulwarkAuth";
if(!string.IsNullOrEmpty(appConfig.DbNameSeed))
{
    dbName = $"{dbName}-{appConfig.DbNameSeed}";
}

var passwordPolicy = new PasswordPolicy();
var passwordLength = new PasswordLength(8, 512);
passwordPolicy.Add(passwordLength);
var passwordLowerCase = new PasswordLowerCase();
passwordPolicy.Add(passwordLowerCase);
var passwordUpperCase = new PasswordUpperCase();
passwordPolicy.Add(passwordUpperCase);
var passwordSymbol = new PasswordSymbol();
passwordPolicy.Add(passwordSymbol);
var passwordNumber = new PasswordNumber();
passwordPolicy.Add(passwordNumber);

var signingAlgorithms = new List<ISigningAlgorithm>
{
    new Rsa256(),
    new Rsa384(),
    new Rsa512()
};

applicationBuilder.Services.AddSingleton(passwordPolicy);
applicationBuilder.Services.AddSingleton<JwtTokenizer>(t => new JwtTokenizer("bulwark", "bulwark",
    appConfig.AccessTokenExpireInMinutes, appConfig.RefreshTokenExpireInHours,
    signingAlgorithms, t.GetService<SigningKey>()));
applicationBuilder.Services.AddSingleton(mongoClient.GetDatabase(dbName));
applicationBuilder.Services.AddTransient<ITokenRepository, MongoDbAuthToken>();
applicationBuilder.Services.AddTransient<ISigningKeyRepository, MongoDbSigningKey>();
applicationBuilder.Services.AddTransient<IEncrypt, BulwarkBCrypt>();
applicationBuilder.Services.AddSingleton<SigningKey>();
applicationBuilder.Services.AddTransient<IAccountRepository, MongoDbAccount>();
applicationBuilder.Services.AddTransient<Account>();
applicationBuilder.Services.AddTransient<Authentication>();
applicationBuilder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
applicationBuilder.Services.AddTransient<MagicCode>();
applicationBuilder.Services.AddTransient<IMagicCodeRepository, MongoDbMagicCode>();
applicationBuilder.Services.AddTransient<IAuthorizationRepository, MongoDbAuthorization>();
applicationBuilder.Services.AddTransient<IEmailTemplateRepository, MongoDbEmailTemplate>();
applicationBuilder.Services.AddTransient<EmailTemplate>();
//social startup
var socialValidators = new ValidatorStrategies();

if (!string.IsNullOrEmpty(appConfig.GoogleClientId))
{
    var googleValidator = new GoogleValidator(appConfig.GoogleClientId);
    socialValidators.Add(googleValidator);
}

if (!string.IsNullOrEmpty(appConfig.MicrosoftClientId) && 
    !string.IsNullOrEmpty(appConfig.MicrosoftTenantId))
{
    var microSoftValidator = new MicrosoftValidator(appConfig.MicrosoftClientId, appConfig.MicrosoftTenantId);
    socialValidators.Add(microSoftValidator);
}

if (!string.IsNullOrEmpty(appConfig.GithubAppName))
{
    var gitHubValidator = new GithubValidator(appConfig.GithubAppName);
    socialValidators.Add(gitHubValidator);
}

applicationBuilder.Services.AddSingleton<IValidatorStrategies>(socialValidators);
applicationBuilder.Services.AddTransient<SocialLogin>();
//end of social startup
//end of Inject

//config
var webApplication = applicationBuilder.Build();
var email = webApplication.Services.GetService<EmailTemplate>();

if(email.GetTemplate("VerifyAccount") == null)
{
    email.AddTemplate("VerifyAccount", "Templates/Email/VerifyAccount.cshtml");
}
if(email.GetTemplate("ChangeEmail") == null)
{
    email.AddTemplate("ChangeEmail", "Templates/Email/ChangeEmail.cshtml");
}
if(email.GetTemplate("Forgot") == null)
{
    email.AddTemplate("Forgot", "Templates/Email/Forgot.cshtml");
}
if(email.GetTemplate("MagicLink") == null)
{
    email.AddTemplate("MagicLink", "Templates/Email/MagicLink.cshtml");
}

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
