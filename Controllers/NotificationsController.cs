using Domain.Entities;
using Domain.Responses;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/sms")]
public class NotificationsController : ControllerBase
{
    private readonly ISmsRepository _smsRepository;

    public NotificationsController(ISmsRepository smsRepository)
    {
        _smsRepository = smsRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification([FromQuery] string userSms)
    { 
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdNotification = await _smsRepository.SendNotification(userSms);
        return Ok(createdNotification);
    }

    // Add additional methods for other payment-related functionalities
    // (e.g., GET by payment ID, etc.)}
    [HttpPost("/callback")]
    public async Task<IActionResult> CallbackStatus()
    {
        using var reader = new StreamReader(HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();
        try
        {
            if (!string.IsNullOrEmpty(body))
            {
                var resp = await _smsRepository.ProcessCallBackResult(body);

                return Ok(resp);
            }
        }
        catch (Exception ex)
        {
            throw;
        }

        return BadRequest();
    }
    
}