using System;

namespace Bulwark.Auth;

public class AppConfig
{
    public string DbConnection { get; }
    public string DbNameSeed { get; }
    public string GoogleClientId { get;  }
    public string MicrosoftClientId { get; }
    public string MicrosoftTenantId { get; }
    public string GithubAppName { get; }
    public string Domain { get;  }
    public string WebsiteName { get; }

    public string EmailTemplateDir { get; }

    public AppConfig()
    {
        DbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "mongodb://localhost:27017";
        DbNameSeed = Environment.GetEnvironmentVariable("DB_NAME_SEED") ?? string.Empty;
        GoogleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? string.Empty;
        MicrosoftClientId = Environment.GetEnvironmentVariable("MICROSOFT_CLIENT_ID") ?? string.Empty;
        MicrosoftTenantId = Environment.GetEnvironmentVariable("MICROSOFT_TENANT_ID") ?? string.Empty;
        GithubAppName = Environment.GetEnvironmentVariable("GITHUB_APP_NAME") ?? string.Empty;
        Domain = Environment.GetEnvironmentVariable("DOMAIN") ?? 
                 throw new Exception("DOMAIN environment variable is required.");
        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME") ?? 
                      throw new Exception("WEBSITE_NAME environment variable is required.");
        EmailTemplateDir = Environment.GetEnvironmentVariable("EMAIL_TEMPLATE_DIR") ?? 
                           "Templates/Email/";
               
    }
}