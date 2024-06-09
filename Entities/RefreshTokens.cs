namespace Domain.Entities;

public class RefreshTokens
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiry { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser User { get; set; }
}