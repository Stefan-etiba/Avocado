using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Domain.Requests;
using Domain.Responses;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace APIs.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(IConfiguration configuration,
        ApplicationDbContext dbContext, ILogger<AuthController> logger, UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("SignUp")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
    {
        var responseObject = new Response();
        try
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);

            if (userExists != null)
            {
                responseObject.Success = false;
                responseObject.Error = "User exists!";
                responseObject.StatusCode = 400;

                return BadRequest(responseObject);
            }

            var user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                responseObject.Success = true;
                responseObject.StatusCode = 201;
                return CreatedAtAction(nameof(RegisterUser), responseObject);
            }
            else
            {
                responseObject.Success = false;
                responseObject.StatusCode = 400;
                foreach (var error in result.Errors) responseObject.Error += $"{error.Description}\n";

                return BadRequest(responseObject);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex}, Message: {ex.Message}");
            responseObject.Success = false;
            responseObject.Error = "Something went wrong, Try again";
            responseObject.StatusCode = 400;
            return BadRequest(responseObject);
        }
    }

    [HttpPost("SignIn")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var responseObject = new Response();
        try
        {
            _logger.LogInformation("Login started");
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogInformation("Login success");
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.GivenName, $"{user.FirstName}{user.LastName}"),
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles) authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    _configuration["JWT:ValidIssuer"],
                    _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMonths(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id, DateTime.UtcNow.AddDays(100));

                var result = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiry = token.ValidTo,
                    userId = user.Id
                };
                responseObject.Result = result;
                responseObject.Success = true;
                responseObject.StatusCode = 200;
                return Ok(responseObject);
            }

            _logger.LogInformation("Login failed");
            responseObject.Success = false;
            responseObject.Error = "Wrong credentials.";
            responseObject.StatusCode = 401;
            return Unauthorized(responseObject);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e}, Message:{e.Message}");
            responseObject.Success = false;
            responseObject.Error = "Internal Error";
            responseObject.StatusCode = 400;
            return StatusCode(500, responseObject);
        }
    }
}