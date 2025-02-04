using System;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Login endpoint
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input.");
        }

        try
        {
            var token = await _authService.AuthenticateUserAsync(request.Email, request.Password);
            return Ok(new { Token = token });
        }
        catch (ArgumentException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "An error occurred during authentication.", Details = ex.Message }
            );
        }
    }

    // Register endpoint
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid input.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest("Passwords do not match.");
        }

        try
        {
            var token = await _authService.RegisterUserAsync(request.Email, request.Password);
            return CreatedAtAction(nameof(Register), new { Token = token });
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "An error occurred during registration.", Details = ex.Message }
            );
        }
    }
}

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}
