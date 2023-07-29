using System;
using System.Threading.Tasks;
using Bulwark.Auth.Common;
using Bulwark.Auth.Common.Payloads;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Core.Social;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordLessController : ControllerBase
{
    private readonly IMagicCodeManager _magicCodeManager;
    private readonly ISocialManager _socialManager;
    private readonly IFluentEmail _email;

    public PasswordLessController(IMagicCodeManager magicCodeManager,
        ISocialManager socialManager,
        IFluentEmail email) 
    {
        _magicCodeManager = magicCodeManager;
        _socialManager = socialManager;
        _email = email;
    }

    [HttpGet]
    [Route("magic/request/{email}")]
    public async Task<ActionResult> SendMagicLink(string email)
    {
        try
        {
            var subject = "Login requested";
            var templateDir = "Templates/Email/MagicLink.cshtml";
            var expireInMinutes =
                int.Parse(Environment.GetEnvironmentVariable("MAGIC_CODE_EXPIRE_IN_MINUTES") ?? "60");
            var code = await _magicCodeManager.CreateCode(email, expireInMinutes);

            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = code;
            }

            var magicLinkEmail = _email
                .To(email)
                .Subject(subject)
                .UsingTemplateFromFile(templateDir,
                    new
                    {
                        Email = email,
                        Code = code,
                        MagicLink = Environment.GetEnvironmentVariable("MAGIC_LINK_URL"),
                        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
                    });

            await magicLinkEmail.SendAsync();
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Could not send magic link",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }

    [HttpPost]
    [Route("magic/authenticate")]
    public async Task<ActionResult<Authenticated>> AuthenticateMagic(
        MagicAuthentication magicAuthentication)
    {
        try{
            return await _magicCodeManager.AuthenticateCode(
                magicAuthentication.Email,
                magicAuthentication.Code);
        }
        catch (BulwarkMagicCodeException exception)
        {
            return Problem(
                title: "Could not authenticate magic code",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }

    [HttpPost]
    [Route("social/authenticate")]
    public async Task<ActionResult<Authenticated>> AuthenticateSocial(
        SocialSignIn socialAuthentication)
    {
        try{
            return await _socialManager.Authenticate(socialAuthentication.Provider,
                socialAuthentication.SocialToken);
        }
        catch (BulwarkSocialException exception)
        {
            return Problem(
                title: "Could not authenticate social account",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }
}