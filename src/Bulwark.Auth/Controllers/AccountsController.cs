using System;
using System.IO;
using System.Threading.Tasks;
using Bulwark.Auth.Common;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Exception;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;
    private readonly IAccountManager _accountManager;
    private readonly IFluentEmail _email;
    public AccountsController(ILogger<AccountsController> logger,
        IAccountManager accountManager, IFluentEmail email)
    {
        _logger = logger;
        _accountManager = accountManager;
        _email = email;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> CreateAccount(CreatePayload createPayload)
    { 
        var subject = "Please verify your account";
        var templateDir = "Templates/Email/VerifyAccount.cshtml";
        var verificationToken = await _accountManager.Create(createPayload.Email,
            createPayload.Password);
        
        if(Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
        {
            subject = verificationToken.Value;
        }
        
        var verificationEmail = _email
            .To(createPayload.Email)
            .Subject(subject)
            .UsingTemplateFromFile(templateDir,
            new
            {
                Email = createPayload.Email,
                VerificationToken = verificationToken.Value,
                VerificationUrl = Environment.GetEnvironmentVariable("VERIFICATION_URL"),
                WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
            });

        await verificationEmail.SendAsync();
        return NoContent();
    
    }

    [HttpPost]
    [Route("verify")]
    public async Task<ActionResult> VerifyAccount(VerifyPayload payload)
    {
        try
        {
            await _accountManager.Verify(payload.Email, payload.Token);
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Cannot verify account",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }

    [HttpPut]
    [Route("delete")]
    public async Task<ActionResult> DeleteAccount(DeletePayload payload)
    {
        try
        {
            await _accountManager.Delete(payload.Email, payload.AccessToken);
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Bad Input",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
           );
        }
    }
    /// <summary>
    /// When a user has a valid access token they can change there email
    /// Should log the user out and re-authenticate after this. 
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("email")]
    public async Task<ActionResult> ChangeEmail(ChangeEmailPayload payload)
    {
        try
        {
            await _accountManager.ChangeEmail(payload.Email,
                payload.NewEmail, payload.AccessToken);
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Bad Input",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
           );
        }
    }

    [HttpPut]
    [Route("password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordPayload payload)
    {
        try
        {
            await _accountManager.ChangePassword(payload.Email,
                payload.NewPassword, payload.AccessToken);
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Bad Input",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
           );
        }

        return NoContent();
    }

    [HttpGet]
    [Route("forgot/{email}")]
    public async Task<ActionResult> ForgotToken(string email)
    {
        try
        {
            var subject = "Requested to reset password";
            var token = await _accountManager.ForgotPassword(email);
            var templateDir =
                $"{Directory.GetCurrentDirectory()}/Templates/Email/Forgot.cshtml";

            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = token;
            }

            var forgotEmail = _email
                .To(email)
                .Subject(subject)
                .UsingTemplateFromFile(templateDir,
                    new
                    {
                        Email = email,
                        ForgotToken = token,
                        ForgotUrl = Environment.GetEnvironmentVariable("FORGOT_PASSWORD_URL"),
                        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
                    });

            await forgotEmail.SendAsync();
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Failed to send forgot password email",
                detail: exception.Message,  
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }

    [HttpPut]
    [Route("forgot")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordPayload
        payload)
    {
        try
        {
            await _accountManager.ResetPasswordWithToken(
                payload.Email, payload.Token, payload.Password);

            return NoContent();
        }
        catch(Exception exception)
        {
            return Problem(
                title: "Could not reset password",
                detail: exception.Message,  
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }
}
