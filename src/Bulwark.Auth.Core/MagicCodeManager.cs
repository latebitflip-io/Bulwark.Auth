using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Core;

public class MagicCodeManager : IMagicCodeManager
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMagicCodeRepository _magicCodeRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly TokenStrategyContext _tokenStrategy;

    public MagicCodeManager(IMagicCodeRepository magicCodeRepository,
        IAccountRepository accountRepository, IAuthorizationRepository authorizationRepository,
        ICertManager certManager)
	{
        _accountRepository = accountRepository;
        _magicCodeRepository = magicCodeRepository;
        _authorizationRepository = authorizationRepository;

        _tokenStrategy = certManager.TokenContext;
    }

    public async Task<Authenticated> AuthenticateCode(string email,
        string code, string tokenizerName = "default")
    {
        var accountModel = await _accountRepository.GetAccount(email);
        var magicCodeModel =
            await _magicCodeRepository.Get(accountModel.Id.ToString(),
            code);

        if (!IsCodeExpired(magicCodeModel))
        {
            throw new BulwarkMagicCodeException("Magic code has expired");
        }

        await _magicCodeRepository.Delete(accountModel.Id.ToString(),
            code);

        var roles = await _authorizationRepository.ReadAccountRoles(accountModel.Id.ToString());
        var permissions = await _authorizationRepository.ReadAccountPermissions(accountModel.Id.ToString());
        return Util.Authenticate
               .CreateTokens(accountModel, roles, permissions,
               _tokenStrategy.GetTokenizer(tokenizerName));

    }

    public async Task<string> CreateCode(string email, int expireInMins)
    {
        //make the length configurable
        var code = GetUniqueKey(6);
        var accountModel = await _accountRepository.GetAccount(email);
        await _magicCodeRepository.Add(accountModel.Id.ToString(),
            code, DateTime.Now.AddMinutes(expireInMins));

        return code;
    }

    private bool IsCodeExpired(MagicCodeModel code)
    {
        return code.Expires >= DateTime.Now;
    }

    public static string GetUniqueKey(int size)
    {
        char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }  
}


