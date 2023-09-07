using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Repositories;
using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Core.Social;
public class SocialService : ISocialService
{
    private readonly Dictionary<string, ISocialValidator> _socialValidators;
    private readonly IAccountRepository _accountRepository;
    private readonly TokenStrategyContext _tokenStrategy;
    private readonly IAuthorizationRepository _authorizationRepository;
    
    public SocialService(IValidatorStrategies validatorStrategies,
        IAccountRepository accountRepository, IAuthorizationRepository authorizationRepository,
        ISigningKeyService signingKeyService)
    {
        _socialValidators = validatorStrategies.GetAll();
        _accountRepository = accountRepository;
        _authorizationRepository = authorizationRepository;
        _tokenStrategy = signingKeyService.TokenContext;
    }

    public void AddValidator(ISocialValidator validator)
    {
        _socialValidators.Add(validator.Name, validator);
    }

    public async Task<Authenticated> Authenticate(string provider,
        string token, string tokenizerName = "jwt")
    {
        var social = await _socialValidators[provider].ValidateToken(token);
        AccountModel accountModel;
        try
        {
            accountModel = await _accountRepository.GetAccount(social.Email);
        }
        catch (BulwarkDbNotFoundException)
        {
            //TODO: Log
            accountModel = null; 
        }

        if (accountModel == null)
        {
            var verification =
                await _accountRepository.Create(social.Email,
                Guid.NewGuid().ToString());
            await _accountRepository.Verify(social.Email,
                verification.Token);
            var socialProvider = new SocialProvider()
            {
                Name = provider,
                SocialId = social.SocialId

            };
            await _accountRepository.LinkSocial(social.Email,
                socialProvider);

            accountModel = await _accountRepository.GetAccount(social.Email);
        }

        if (!IsLinked(accountModel, provider, social.SocialId))
        {
            var socialProvider = new SocialProvider()
            {
                Name = provider,
                SocialId = social.SocialId

            };

            await _accountRepository.LinkSocial(social.Email,
                socialProvider);

            accountModel =
                await _accountRepository.GetAccount(social.Email);
        }
        var roles = await _authorizationRepository.ReadAccountRoles(accountModel.Id);
        var permissions = await _authorizationRepository.ReadAccountPermissions(accountModel.Id);
        return Util.Authenticate
                .CreateTokens(accountModel, roles, permissions,
                _tokenStrategy.GetTokenizer(tokenizerName));
    }

    private bool IsLinked(AccountModel account, string provider, string socialId)
    {
        var link = account
            .SocialProviders.Find(s => s.Name == provider &&
                                                 s.SocialId == socialId);

        return link != null;
    }
}

