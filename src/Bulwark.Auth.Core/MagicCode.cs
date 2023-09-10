using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Core;

/// <summary>
/// This class manages the use of magic codes instead of passwords to login
/// </summary>
public class MagicCode{
    private readonly IAccountRepository _accountRepository;
    private readonly IMagicCodeRepository _magicCodeRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly JwtTokenizer _tokenizer;
    
    private const int MagicCodeLength = 6;

    public MagicCode(IMagicCodeRepository magicCodeRepository,
        IAccountRepository accountRepository, IAuthorizationRepository authorizationRepository,
        JwtTokenizer tokenizer)
	{
        _accountRepository = accountRepository;
        _magicCodeRepository = magicCodeRepository;
        _authorizationRepository = authorizationRepository;

        _tokenizer = tokenizer;
    }

    /// <summary>
    /// Authenticate a magic code for an account
    /// </summary>
    /// <param name="email"></param>
    /// <param name="code"></param>
    /// <param name="tokenizerName"></param>
    /// <returns></returns>
    /// <exception cref="BulwarkMagicCodeException"></exception>
    public async Task<Authenticated> AuthenticateCode(string email,
        string code, string tokenizerName = "jwt")
    {
        var accountModel = await _accountRepository.GetAccount(email);
        var magicCodeModel =
            await _magicCodeRepository.Get(accountModel.Id,
            code);

        if (!IsCodeExpired(magicCodeModel))
        {
            throw new BulwarkMagicCodeException("Magic code has expired");
        }

        await _magicCodeRepository.Delete(accountModel.Id,
            code);

        var roles = await _authorizationRepository.ReadAccountRoles(accountModel.Id);
        var permissions = await _authorizationRepository.ReadAccountPermissions(accountModel.Id);
        return Util.Authenticate
               .CreateTokens(accountModel, roles, permissions,
               _tokenizer);

    }

    /// <summary>
    /// This will create a magic code for an account default is 6 characters
    /// </summary>
    /// <param name="email"></param>
    /// <param name="expireInMin"></param>
    /// <returns></returns>
    public async Task<string> CreateCode(string email, int expireInMin)
    {
        //make the length configurable
        var code = GetUniqueKey(MagicCodeLength);
        var accountModel = await _accountRepository.GetAccount(email);
        await _magicCodeRepository.Add(accountModel.Id,
            code, DateTime.Now.AddMinutes(expireInMin));

        return code;
    }

    /// <summary>
    /// Check is a magic code is expired
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private static bool IsCodeExpired(MagicCodeModel code)
    {
        return code.Expires >= DateTime.Now;
    }

    /// <summary>
    /// Generates a magic code string
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private static string GetUniqueKey(int size)
    {
        var chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[4 * size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }  
}


