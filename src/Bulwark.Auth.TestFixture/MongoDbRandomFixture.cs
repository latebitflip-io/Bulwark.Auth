using MongoDB.Driver;

namespace Bulwark.Auth.TestFixture;

public class MongoDbRandomFixture : IDisposable
{
    public MongoClient Client { get; private set; }
    public IMongoDatabase Db { get; private set; }
    private const string _connection = "mongodb://localhost:27017";
    private string _testDb;

    public MongoDbRandomFixture()
    {
        var rnd = new Random();
        var num = rnd.Next();

        _testDb = $"bulwark_tests_{num}";
        Client = new MongoClient(_connection);
        Db = Client.GetDatabase(_testDb);
    }
    public IMongoDatabase GetDatabase(string testDb)
    {
        return Client.GetDatabase(testDb);
    }
    public void Dispose()
    {
        Client.DropDatabase(_testDb);
    }
}


