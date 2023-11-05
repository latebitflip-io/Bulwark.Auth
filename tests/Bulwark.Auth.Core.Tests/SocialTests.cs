using System.Collections.Generic;
using Bulwark.Auth.Core.SigningAlgs;
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
    private readonly SocialLogin _socialLogin;

    public SocialTests(MongoDbRandomFixture dbFixture)
	{
        var encrypt = new BulwarkBCrypt();
        IValidatorStrategies validators = new ValidatorStrategies();
        IAccountRepository accountRepository = new MongoDbAccount(dbFixture.Client, 
            dbFixture.Db,
            encrypt);
        var signingKeyRepository = new MongoDbSigningKey(dbFixture.Db);
        var signingKey = new SigningKey(signingKeyRepository);
        var jwtTokenizer = new JwtTokenizer("test", "test", 10, 24,
            new List<ISigningAlgorithm> {new Rsa256()}, signingKey);
        new MongoDbAuthToken(dbFixture.Db);
        validators.Add(new MockSocialValidator("bulwark"));
        validators.Add(new GoogleValidator(
            "651882111548-0hrg7e4o90q1iutmfn02qkf9m90k3d3g.apps.googleusercontent.com"));
        validators.Add(new MicrosoftValidator("c9ece416-eadf-4c84-9569-692b8144f50f", "9188040d-6c67-4c5b-b112-36a304b66dad"));
        validators.Add(new GithubValidator("lateflip.io" ));
        var authorizationRepository = new MongoDbAuthorization(dbFixture.Db);
        _socialLogin = new SocialLogin(validators, accountRepository, 
            authorizationRepository, jwtTokenizer); 
    }

    [Fact]
    public async Task AuthenticateSocialToken()
    {
        var authenticated = 
            await _socialLogin.Authenticate("bulwark", "validtoken");

        Assert.NotNull(authenticated.AccessToken);
    }

    [Fact]
    public async Task AuthenticateGoogleToken()
    {
        try{
            var authenticated =
                await _socialLogin.Authenticate("google",
                    "eyJhbGciOiJSUzI1NiIsImtpZCI6IjU1MmRlMjdmNTE1NzM3NTM5NjAwZDg5YjllZTJlNGVkNTM1ZmI1MTkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJuYmYiOjE2NzAzNDgxNDYsImF1ZCI6IjY1MTg4MjExMTU0OC0waHJnN2U0bzkwcTFpdXRtZm4wMnFrZjltOTBrM2QzZy5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjEwNzU3Nzk1MDE3MTYyMjk3ODU4NCIsImVtYWlsIjoiZnJlZHJpY2suc2VpdHpAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImF6cCI6IjY1MTg4MjExMTU0OC0waHJnN2U0bzkwcTFpdXRtZm4wMnFrZjltOTBrM2QzZy5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsIm5hbWUiOiJGcmVkIFNlaXR6IiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FFZEZUcDZNUVhVeVp3czFpVHNqeUo2djc5cW1CdnVqWEpoenc2eW5CaFZSPXM5Ni1jIiwiZ2l2ZW5fbmFtZSI6IkZyZWQiLCJmYW1pbHlfbmFtZSI6IlNlaXR6IiwiaWF0IjoxNjcwMzQ4NDQ2LCJleHAiOjE2NzAzNTIwNDYsImp0aSI6IjkyY2I5MjY3MzY5MGQ0MGUzMjhiNzQxNDY1NGI2YjBkZTU4YjBiOTEifQ.k-2AyTnKiHieQ--ecIdasdsh0qBCZoEEBJ9GJZWsAr8nmY9QUQ6xui98JDssF5KHoa0TBRCspEJhsd_E96Ycb_6DcMQs5dTWtPWcnR6eoJfgLm8uhtZJnY11Z-YtU6oDapF8g3OHN-P0JHPQ-PFyJ-qd_peIptLGSBSPC-njTV6C8z_jynEEmV3No0YvVahe6N1qk5cmPIXmFPnvB_EpwCR7TEsDCAhFDHzpO2-gtnAF5fVBhkdjJEfbRlw9htzZkgy_MnxtxnInQC_nIBQjtQYksBBWnTk6UsdLiBNPhN9HKVnyCh4nyP0fbQne6KTxLA7hP5Y_QR2xHw0hSRHNQg");

            Assert.NotNull(authenticated.AccessToken);
        }
        catch(System.Exception e){
            Assert.True(e.InnerException?.Message == "JWT has expired.", 
                e.InnerException?.Message);
        }
    }
    
    [Fact]
    public async Task AuthenticateMicrosoftToken()
    {
        try{
            var authenticated =
                await _socialLogin.Authenticate("microsoft",
                    "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6InlyZVgyUHNMaS1xa2JSOFFET21CX3lTeHA4USJ9.eyJ2ZXIiOiIyLjAiLCJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vOTE4ODA0MGQtNmM2Ny00YzViLWIxMTItMzZhMzA0YjY2ZGFkL3YyLjAiLCJzdWIiOiJBQUFBQUFBQUFBQUFBQUFBQUFBQUFJOGFPcU01SUF5dXh0SWFwS2IyeVU4IiwiYXVkIjoiYzllY2U0MTYtZWFkZi00Yzg0LTk1NjktNjkyYjgxNDRmNTBmIiwiZXhwIjoxNjkwOTIwOTgxLCJpYXQiOjE2OTA4MzQyODEsIm5iZiI6MTY5MDgzNDI4MSwibmFtZSI6IkZyaXR6IFNlaXR6IiwicHJlZmVycmVkX3VzZXJuYW1lIjoiZnJlZHJpY2suc2VpdHpAZ21haWwuY29tIiwib2lkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTYzN2ItOThlMGQ4MzM5OWYxIiwidGlkIjoiOTE4ODA0MGQtNmM2Ny00YzViLWIxMTItMzZhMzA0YjY2ZGFkIiwibm9uY2UiOiI3MjYzM2JhYS0xZDQzLTRkOGEtOGFhMC1mZDY1ZGU0NDE2OWUiLCJhaW8iOiJEU2tqIUdWRDlEKldLMmNudVJ1Tm45cEM5Y2pHTkdjRjFqWWEzUSFmQTRSTSEqRmNkcUJoSjNWN0p4VWJPUXdSamp4cTFOTTBwNmwqV1JMdnVhTjBvZG11em8yZW9QcmRhcTB5ekdtNDJQYmZMWjFSRWM0Y3RvWm1SZXVBUTE3bG5mcTcwN0Rnb0NKRTU0TmJiRU0qUUhzJCJ9.b6-MBEcg88_OH-pw5hQQADbRpWHD5dtJAzctX6_lUoujNfnBeVQQMu4G0HhBlcbMU_JL7ujfcj2RfZGjZgxAyeORgheDYE-tQXCj6nd_r1rawPIqEZHBVe9fVqyR7Ditta2LwxXI3IY6H3LajWXJbZQnNhCqwts9JHu-vsSKg6ro1a5g8CAOo6J0W9xHWDNsa2njMkt1cttIz6DZ3JhMStr7NXM3obvcr6UzULJve1cga6ZdOSUdIHCPkpeZUwjPplhc14ydjUGTbUFGt1sOAWXDF_9968BXcUv-0lcZpwICRNjwG25YHnhdmDGn66XK1CwW-x8qu9gb6LMBmRGboA");

            Assert.NotNull(authenticated.AccessToken);
        }
        catch(System.Exception e){
            Assert.True(e.InnerException?.Message.Contains("The token is expired"), 
                e.InnerException?.Message);
        }
    }
    
    [Fact]
    public async Task AuthenticateGithubToken()
    {
        try{
            var authenticated =
                await _socialLogin.Authenticate("github", "gho_jsjjsksl0561661");

            Assert.NotNull(authenticated.AccessToken);
        }
        catch(System.Exception e){
            Assert.True(e.InnerException?.Message.Contains("Bad credentials"), 
                e.InnerException?.Message);
        }
    }
}