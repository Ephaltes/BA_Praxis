using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace WebServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LogFileController : ControllerBase
{
    private readonly ILogger<LogFileController> _logger;
    public LogFileController(ILogger<LogFileController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] string data)
    {
        IPAddress? remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;

        _logger.LogInformation("Received data");

        await System.IO.File.WriteAllTextAsync($"{DateTime.Now:dd-MM-yyyy}-{remoteIpAddress?.MapToIPv4().ToString()}", data);

        return Ok();
    }
}