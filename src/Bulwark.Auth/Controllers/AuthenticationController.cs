using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bulwark.Auth.Common.Payloads;
using Bulwark.Auth.Core;
using Bulwark.Auth.Core.Domain;
using Bulwark.Auth.Core.Exception;
using Microsoft.AspNetCore.Http;
using RefreshToken = Bulwark.Auth.Common.Payloads.RefreshToken;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IAuthenticationManager _authManager;

    public AuthenticationController(IAuthenticationManager authManager,
        ILogger<AuthenticationController> logger)
    {
        _logger = logger;
        _authManager = authManager;
    }

    [HttpPost]
    [Route("authenticate")]
    public async Task<ActionResult<Authenticated>>
        Authenticate(Authenticate payload)
    {
        try
        {
            return await _authManager.Authenticate(payload.Email, payload.Password);
        }
        catch(BulwarkAuthenticationException exception)
        {
            return Problem(
                title: "Bad Input",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/400",
                statusCode: StatusCodes.Status400BadRequest
           );
        }
    }

    [HttpPost]
    [Route("acknowledge")]
    public async Task<ActionResult> Acknowledge(Acknowledge payload)
    {
        try
        {
            var authenticated = new Authenticated(payload.AccessToken,
                payload.RefreshToken);
            await _authManager.Acknowledge(authenticated,
                payload.Email, payload.DeviceId);
            return NoContent();
        }
        catch(BulwarkTokenException exception)
        {
            return Problem(
                title: "Bad Tokens",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/400",
                statusCode: StatusCodes.Status422UnprocessableEntity
           );
        }
    }

    [HttpPost]
    [Route("accesstoken/validate")]
    public async Task<ActionResult<Core.Domain.AccessToken>> ValidateAccessToken(AccessTokenValidate
        validate)
    {
        try
        {
            var token = await _authManager.ValidateAccessToken(validate.Email,
                validate.Token, validate.DeviceId);
            return token;
        }
        catch (BulwarkTokenException exception)
        {
            return Problem(
                title: "Invalid Token",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/400",
                statusCode: StatusCodes.Status422UnprocessableEntity
           );
        }
    }

    [HttpPost]
    [Route("renew")]
    public async Task<ActionResult<Authenticated>> RenewCredentials(RefreshToken payload)
    {
        try
        {
            var authenticated = await _authManager.Renew(payload.Email,
                payload.Token, payload.DeviceId);

            return authenticated;
        }
        catch (BulwarkTokenException exception)
        {
            return Problem(
                title: "Bad Tokens",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/400",
                statusCode: StatusCodes.Status422UnprocessableEntity
           );
        }
    }

    [HttpPost]
    [Route("revoke")]
    public async Task<ActionResult> Revoke(AccessTokenValidate validate)
    {
        try
        {
            await _authManager.Revoke(validate.Email,
                validate.Token, validate.DeviceId);

            return NoContent();
        }
        catch (BulwarkTokenException exception)
        {
            return Problem(
                title: "Can not revoke",
                detail: exception.Message,
                type: "https://www.Bulwark.Auth.io/422",
                statusCode: StatusCodes.Status422UnprocessableEntity
           );
        } 
    }   
}

