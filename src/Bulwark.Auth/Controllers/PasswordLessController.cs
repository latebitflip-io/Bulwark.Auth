using System;
using System.Threading.Tasks;
using Bulwark.Auth.Common.Payloads;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Bulwark.Auth.Core.Social;
using Bulwark.Auth.Templates;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class PasswordLessController : ControllerBase
{
    private readonly MagicCode _magicCode;
    private readonly SocialLogin _socialLogin;
    private readonly IFluentEmail _email;
    private readonly EmailTemplate _emailTemplate;

    public PasswordLessController(MagicCode magicCode,
        SocialLogin socialLogin,
        IFluentEmail email, EmailTemplate emailTemplate) 
    {
        _magicCode = magicCode;
        _socialLogin = socialLogin;
        _email = email;
        _emailTemplate = emailTemplate;
    }

    [HttpGet]
    [Route("magic/request/{email}")]
    public async Task<ActionResult> SendMagicLink(string email)
    {
        try
        {
            var subject = "Login requested";
            var expireInMinutes =
                int.Parse(Environment.GetEnvironmentVariable("MAGIC_CODE_EXPIRE_IN_MINUTES") ?? "60");
            var code = await _magicCode.CreateCode(email, expireInMinutes);

            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = code;
            }

            var magicLinkEmail = _email
                .To(email)
                .Subject(subject)
                .UsingTemplate(_emailTemplate.GetTemplate("MagicLink"),
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
            return await _magicCode.AuthenticateCode(
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
            return await _socialLogin.Authenticate(socialAuthentication.Provider,
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