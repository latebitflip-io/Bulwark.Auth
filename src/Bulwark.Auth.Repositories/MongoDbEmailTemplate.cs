using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories;

public class MongoDbEmailTemplate : IEmailTemplateRepository
{
    private readonly IMongoCollection<EmailTemplateModel> _templateCollection;
    
    public MongoDbEmailTemplate(IMongoDatabase db)
    {
        _templateCollection = db.GetCollection<EmailTemplateModel>("emailTemplate");
        CreateIndexes();
    }
    
    public EmailTemplateModel GetTemplate(string templateName)
    {
        var emailTemplate = _templateCollection.AsQueryable()
            .Where(c => c.Name == templateName)
            .FirstOrDefault();
        return emailTemplate;
    }

    public void AddTemplate(string templateName, string template)
    {
        var email = new EmailTemplateModel(templateName, template);
        _templateCollection.InsertOne(email);
    }
    
    private void CreateIndexes()
    { 
        var indexKeysDefine = Builders<EmailTemplateModel>
            .IndexKeys
            .Ascending(indexKey => indexKey.Name);

        _templateCollection.Indexes.CreateOne(
            new CreateIndexModel<EmailTemplateModel>(indexKeysDefine,
                new CreateIndexOptions()
                {
                    Unique = true,
                    Name = "Attr_Unique"
                }));
    }
}