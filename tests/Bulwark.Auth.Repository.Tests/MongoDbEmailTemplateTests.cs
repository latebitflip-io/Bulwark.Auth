using System.IO;
using System.Text;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.TestFixture;

namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbEmailTemplateTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly IEmailTemplateRepository _templateRepository;
    
    public MongoDbEmailTemplateTests(MongoDbRandomFixture dbFixture)
    {
        var encryption = new BulwarkBCrypt();
        _templateRepository = new MongoDbEmailTemplate(dbFixture.Db);
    }
    
    [Fact]
    public void AddTemplateTest()
    {
        const string templateName = "VerifyAccount";
        const string templatePath = "VerifyAccount.cshtml";
        try
        {
            string email;
            using (var streamReader = new StreamReader(templatePath, Encoding.UTF8))
            {
                email = streamReader.ReadToEnd();
            }
            
            _templateRepository.AddTemplate(templateName, email);
            var template = _templateRepository.GetTemplate(templateName);
            Assert.Equal(templateName, template.Name);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }
}