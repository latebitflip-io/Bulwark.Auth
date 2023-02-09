using Bulwark.Auth.Tests;

namespace Bulwark.Auth.Repositories.Tests;

public class MongoDbAuthTokenTests : IClassFixture<MongoDbRandomFixture>
{
    private readonly ITokenRepository _authTokenRepository;
    
    public MongoDbAuthTokenTests(MongoDbRandomFixture dbFixture)
    {
        _authTokenRepository = new MongoDbAuthToken(dbFixture.Db);
    }

    
    [Fact]
    public async void Get()
    {
        try
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            await _authTokenRepository.Acknowledge(userId.ToString(), "devicehash",
                jwt, jwt);
            var auth =
                await _authTokenRepository.Get(userId.ToString(), "devicehash");

            Assert.True(auth.DeviceId == "devicehash" &&
                auth.UserId == userId);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void Awknowledge()
    {
        try
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            await _authTokenRepository.Acknowledge(userId.ToString(), "devicehash",
                jwt, jwt);

            await _authTokenRepository.Acknowledge(userId.ToString(),
                "devicehash", "replaced", "replaced");
            var auth =
                await _authTokenRepository.Get(userId.ToString(),
                "devicehash");

            Assert.True(auth.DeviceId == "devicehash" &&
                auth.UserId == userId &&
                auth.RefreshToken == "replaced" &&
                auth.AccessToken == "replaced");
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void Delete()
    {
        try
        {
            var userId = ObjectId.GenerateNewId();
            var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            await _authTokenRepository.Acknowledge(userId.ToString(), "devicehash",
                jwt, jwt);

            await _authTokenRepository.Delete(userId.ToString(),
                "devicehash");

            var result = await _authTokenRepository.Get(userId.ToString(),
                "devicehash");

            Assert.Null(result);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }

    [Fact]
    public async void DeleteAllDevices()
    {
        try
        {
            var userId = ObjectId.GenerateNewId();
            var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            await _authTokenRepository.Acknowledge(userId.ToString(), "devicehash",
                jwt, jwt);
           
            await _authTokenRepository.Acknowledge(userId.ToString(), "devicehash2",
                jwt, jwt);

            await _authTokenRepository.Delete(userId.ToString());

            var result = await _authTokenRepository.Get(userId.ToString(),
                "devicehash");

            Assert.Null(result);

            result = await _authTokenRepository.Get(userId.ToString(),
                "devicehash1");

            Assert.Null(result);
        }
        catch (System.Exception exception)
        {
            Assert.True(false, exception.Message);
        }
    }
}

