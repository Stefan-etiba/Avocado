using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Entities;

public class Student
{
    [Key]
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Payment>? Payments { get; set; }  // Navigation property for payments
}
public class Payment
{
    [Key]
    public int PaymentId { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public Student Student { get; set; }  // Navigation property for the student
}
