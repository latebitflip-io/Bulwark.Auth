using System;
using System.IO;
using System.Threading.Tasks;
using Bulwark.Auth.Common.Payloads;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.PasswordPolicy;
using Bulwark.Auth.Core.Util;
using Bulwark.Auth.Templates;
using FluentEmail.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly Account _account;
    private readonly IFluentEmail _email;
    private readonly PasswordPolicy _passwordPolicy;
    private readonly EmailTemplate _emailTemplate;
    public AccountsController(Account account, IFluentEmail email, 
        PasswordPolicy passwordPolicy, EmailTemplate emailTemplate)
    {
        _account = account;
        _email = email;
        _passwordPolicy = passwordPolicy;
        _emailTemplate = emailTemplate;

    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> CreateAccount(Create create)
    { 
        var subject = "Please verify your account";
        try
        {
            EmailValidator.Validate(create.Email);
            _passwordPolicy.Validate(create.Password);
            var verificationToken = await _account.Create(create.Email,
                create.Password);
            // feature flag for testing, allows easy extraction from email to run unit tests
            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = verificationToken.Value;
            }

            var verificationEmail = _email
                .To(create.Email)
                .Subject(subject)
                .UsingTemplate(_emailTemplate.GetTemplate("VerifyAccount"),
                    new
                    {
                        create.Email,
                        VerificationToken = verificationToken.Value,
                        VerificationUrl = Environment.GetEnvironmentVariable("VERIFICATION_URL"),
                        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
                    });

            var emailResponse = await verificationEmail.SendAsync();
            if (!emailResponse.Successful)
            {
                return Problem(
                    title: "Account created but failed to send verification email",
                    detail: string.Join( ",", emailResponse.ErrorMessages),
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Cannot create account",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }

    [HttpPost]
    [Route("verify")]
    public async Task<ActionResult> VerifyAccount(Verify payload)
    {
        try
        {
            await _account.Verify(payload.Email, payload.Token);
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
    public async Task<ActionResult> DeleteAccount(Delete payload)
    {
        try
        {
            await _account.Delete(payload.Email, payload.AccessToken);
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
    public async Task<ActionResult> ChangeEmail(ChangeEmail payload)
    {
        var subject = "Please verify email change";
        try
        {
            EmailValidator.Validate(payload.NewEmail);
            var verificationToken = await _account.ChangeEmail(payload.Email,
                payload.NewEmail, payload.AccessToken);
            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = verificationToken.Value;
            }

            var verificationEmail = _email
                .To(payload.NewEmail)
                .Subject(subject)
                .UsingTemplate(_emailTemplate.GetTemplate("ChangeEmail"),
                    new
                    {
                        Email = payload.NewEmail,
                        VerificationToken = verificationToken.Value,
                        VerificationUrl = Environment.GetEnvironmentVariable("VERIFICATION_URL"),
                        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
                    });

            var emailResponse = await verificationEmail.SendAsync();
            if (!emailResponse.Successful)
            {
                return Problem(
                    title: "Email successfully changed but failed to send verification email",
                    detail: string.Join( ",", emailResponse.ErrorMessages),
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Cannot change email",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }

    [HttpPut]
    [Route("password")]
    public async Task<ActionResult> ChangePassword(ChangePassword payload)
    {
        try
        {
            _passwordPolicy.Validate(payload.NewPassword);
            await _account.ChangePassword(payload.Email,
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
            EmailValidator.Validate(email);
            var subject = "Requested to reset password";
            var token = await _account.ForgotPassword(email);
            
            if (Environment.GetEnvironmentVariable("SERVICE_MODE")?.ToLower() == "test")
            {
                subject = token;
            }

            var forgotEmail = _email
                .To(email)
                .Subject(subject)
                .UsingTemplate(_emailTemplate.GetTemplate("Forgot"),
                    new
                    {
                        Email = email,
                        ForgotToken = token,
                        ForgotUrl = Environment.GetEnvironmentVariable("FORGOT_PASSWORD_URL"),
                        WebsiteName = Environment.GetEnvironmentVariable("WEBSITE_NAME")
                    });

            var emailResponse = await forgotEmail.SendAsync();
            if (!emailResponse.Successful)
            {
                return Problem(
                    title: "Failed to send forgot password email",
                    detail: string.Join( ",", emailResponse.ErrorMessages),
                    statusCode: StatusCodes.Status400BadRequest);
            }
            return NoContent();
        }
        catch (Exception exception)
        {
            return Problem(
                title: "Failed to send forgot password email",
                detail: exception.Message,
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }

    [HttpPut]
    [Route("forgot")]
    public async Task<ActionResult> ForgotPassword(ForgotPassword
        payload)
    {
        try
        {
            _passwordPolicy.Validate(payload.Password);
            await _account.ResetPasswordWithToken(
                payload.Email, payload.Token, payload.Password);

            return NoContent();
        }
        catch(Exception exception)
        {
            return Problem(
                title: "Could not reset password",
                detail: exception.Message,
                statusCode: StatusCodes.Status422UnprocessableEntity
            );
        }
    }
}
