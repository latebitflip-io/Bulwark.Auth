using System.IO;
using System.Text;
using Bulwark.Auth.Repositories;

namespace Bulwark.Auth.Templates;

public class EmailTemplate
{
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    
    public EmailTemplate(IEmailTemplateRepository emailTemplateRepository)
    {
        _emailTemplateRepository = emailTemplateRepository;
    }
    
    public string GetTemplate(string templateName)
    {
        var email = _emailTemplateRepository.GetTemplate(templateName);
        return email == null ? null : email.Template;
    }
    
    public void AddTemplate(string templateName, string templatePath)
    {
        string readContents;
        using (var streamReader = new StreamReader(templatePath, Encoding.UTF8))
        {
            readContents = streamReader.ReadToEnd();
        }

        _emailTemplateRepository.AddTemplate(templateName, readContents);
    }
}