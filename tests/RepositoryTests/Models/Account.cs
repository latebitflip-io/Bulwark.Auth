using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Bulwark.Auth.Repositories.Tests.Models
{
    [BsonIgnoreExtraElements]
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")] public string Email { get; set; }
        [BsonElement("salt")] public string Salt { get; set; }
        [BsonElement("isVerified")] public bool IsVerified { get; set; }
        [BsonElement("isEnabled")] public bool IsEnabled { get; set; }
        [BsonElement("isDeleted")] public bool IsDeleted { get; set; }

        [BsonElement("socialProviders")]
        [BsonIgnoreIfNull]
        public List<SocialProvider> SocialProviders { get; set; }

        [BsonElement("roles")] public List<string> Roles { get; set; }

        [BsonElement("created")] public DateTime Created { get; set; }
        [BsonElement("modified")] public DateTime Modified { get; set; }

        public Account()
        {
            Roles = new List<string>();
            SocialProviders = new List<SocialProvider>();
            Email = string.Empty;
            Salt = string.Empty;
        }
    }

    public class SocialProvider
    {
        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("socialId")] public string SocialId { get; set; }
    }
}
