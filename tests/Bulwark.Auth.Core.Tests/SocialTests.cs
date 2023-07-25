using Bulwark.Auth.Core.Social;
using Bulwark.Auth.Core.Social.Validators;
using Bulwark.Auth.Core.Tests.Mocks;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Util;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Core.Tests;

[Collection("Sequential")]
public class SocialTests : IClassFixture<MongoDbRandomFixture>
{
    private ITokenRepository _tokenRepository;
    private readonly ISocialManager _socialManager;

    public SocialTests(MongoDbRandomFixture dbFixture)
	{
        var dbFixture1 = dbFixture;
        var encrypt = new BulwarkBCrypt();
        IValidatorStrategies validators = new ValidatorStrategies();
        IAccountRepository accountRepository = new MongoDbAccount(dbFixture1.Db,
            encrypt);
        ICertRepository certRepository = new MongoDbCert(dbFixture1.Db);
        ICertManager certManager = new CertManager(certRepository);
        _tokenRepository = new MongoDbAuthToken(dbFixture1.Db);
        validators.Add(new MockSocialValidator("bulwark"));
        validators.Add(new GoogleValidator(
            "651882111548-0hrg7e4o90q1iutmfn02qkf9m90k3d3g.apps.googleusercontent.com"));
        var authorizationRepository = new MongoDbAuthorization(dbFixture1.Db);
        _socialManager = new SocialManager(validators, accountRepository, 
            authorizationRepository, certManager); 
    }

    [Fact]
    public async void AuthenticateSocialToken()
    {
        var authenticated = 
            await _socialManager.Authenticate("bulwark", "validtoken");

        Assert.NotNull(authenticated.AccessToken);
    }

    [Fact]
    public async void AuthenticateGoogleToken()
    {
        try{
            var authenticated =
                await _socialManager.Authenticate("google",
                    "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU1MmRlMjdmNTE1NzM3NTM5NjAwZDg5YjllZTJlNGVkNTM1ZmI1MTkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJuYmYiOjE2NzAzNDgxNDYsImF1ZCI6IjY1MTg4MjExMTU0OC0waHJnN2U0bzkwcTFpdXRtZm4wMnFrZjltOTBrM2QzZy5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjEwNzU3Nzk1MDE3MTYyMjk3ODU4NCIsImVtYWlsIjoiZnJlZHJpY2suc2VpdHpAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF6cCI6IjY1MTg4MjExMTU0OC0waHJnN2U0bzkwcTFpdXRtZm4wMnFrZjltOTBrM2QzZy5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsIm5hbWUiOiJGcmVkIFNlaXR6IiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FFZEZUcDZNUVhVeVp3czFpVHNqeUo2djc5cW1CdnVqWEpoenc2eW5CaFZSPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IkZyZWQiLCJmYW1pbHlfbmFtZSI6IlNlaXR6IiwiaWF0IjoxNjcwMzQ4NDQ2LCJleHAiOjE2NzAzNTIwNDYsImp0aSI6IjkyY2I5MjY3MzY5MGQ0MGUzMjhiNzQxNDY1NGI2YjBkZTU4YjBiOTEifQ.k-2AyTnKiHieQ--ecIdasdsh0qBCZoEEBJ9GJZWsAr8nmY9QUQ6xui98JDssF5KHoa0TBRCspEJhsd_E96Ycb_6DcMQs5dTWtPWcnR6eoJfgLm8uhtZJnY11Z-YtU6oDapF8g3OHN-P0JHPQ-PFyJ-qd_peIptLGSBSPC-njTV6C8z_jynEEmV3No0YvVahe6N1qk5cmPIXmFPnvB_EpwCR7TEsDCAhFDHzpO2-gtnAF5fVBhkdjJEfbRlw9htzZkgy_MnxtxnInQC_nIBQjtQYksBBWnTk6UsdLiBNPhN9HKVnyCh4nyP0fbQne6KTxLA7hP5Y_QR2xHw0hSRHNQg");

            Assert.NotNull(authenticated.AccessToken);
        }
        catch(System.Exception e){
            Assert.True(e.InnerException.Message == "JWT has expired.", 
                e.InnerException.Message);
        }
    }
}

