using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public interface IEmailTemplateRepository
{
    EmailTemplateModel GetTemplate(string templateName);
    void AddTemplate(string templateName, string template);
}