using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
}

public interface IPaymentRepository
{
    Task<Payment> CreatePaymentAsync(Payment payment);
}
