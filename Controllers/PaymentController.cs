using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers;


[Authorize]
[ApiController]
[Route("api/v1/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentController(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPayment = await _paymentRepository.CreatePaymentAsync(payment);
        return CreatedAtRoute("GetPayment", new { id = createdPayment.PaymentId }, createdPayment);
    }

    // Add additional methods for other payment-related functionalities
    // (e.g., GET by payment ID, etc.)
}
