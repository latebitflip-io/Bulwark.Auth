using System.Collections.Generic;
using System.Linq;
using Bulwark.Auth.Common;
using Bulwark.Auth.Common.Payloads;
using Bulwark.Auth.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Bulwark.Auth.Controllers;

[ApiController]
[Route("")]
public class IndexController : ControllerBase
{
    private readonly ISigningKeyRepository _signingKeyRepository;

    public IndexController(ISigningKeyRepository signingKeyRepository)
    {
        _signingKeyRepository = signingKeyRepository;
    }

    [HttpGet]
    [Route("health")]
    public ActionResult Health()
    {
        return StatusCode(200);
    }
    
    [HttpGet]
    [Route("certs")]
    public ActionResult<List<PublicCert>> Certs()
    {
        var certs = _signingKeyRepository.GetAllKeys();
        return certs.Select(x => new PublicCert()
        {
            Generation = x.Generation,
            PublicKey = x.PublicKey
        }).ToList();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("error")]
    public IActionResult HandleError() => Problem();

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-development")]
    public IActionResult HandleErrorDevelopment(
    [FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: exceptionHandlerFeature.Error.Message);
    }
}


