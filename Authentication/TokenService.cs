using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public interface ITokenService
{
    Task<string> GenerateRefreshToken(string userId, DateTime expiry);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TokenService(ILogger<TokenService> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GenerateRefreshToken(string userId, DateTime expiry)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var result = Convert.ToBase64String(randomNumber);

        var refreshTokeBody = new RefreshTokens
        {
            UserId = userId,
            RefreshToken = result,
            Expiry = expiry,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        var priorRefreshTokens = await _dbContext.RefreshTokens
            .Where(a => a.UserId == userId)
            .ToListAsync();

        foreach (var token in priorRefreshTokens) token.IsDeleted = true;

        await _dbContext.RefreshTokens.AddAsync(refreshTokeBody);
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }
}