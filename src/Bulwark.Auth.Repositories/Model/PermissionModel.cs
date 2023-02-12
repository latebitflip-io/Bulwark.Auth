namespace Bulwark.Auth.Repositories.Model;
public class PermissionModel
{
    [BsonId]
    public string Id { get; set; }
    public DateTime Created { get; set; }

    public PermissionModel()
    {

    }
}


