using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace SothbeysKillerApi.Controllers;

public record UserSignupRequest(string Name, string Email, string Password);

public class User
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }

    public string Password { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    
    private static List<User> _storage = [];

    [HttpGet]
    [Route("getAllUser")]
    public IActionResult GetAllUser()
    {
        return Ok(_storage);
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Signup(UserSignupRequest request)
    {
        
        if (request.Name.Length < 2 || request.Name.Length > 255)
        {
            return BadRequest("Name must be between 2 and 255 characters");
        }
        
        if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return BadRequest("Invalid email address");
        }
        if (_storage.Any(user => user.Email == request.Email))
        {
            return BadRequest("Email is already taken");
        }
        
        if (request.Password.Length < 8 || !Regex.IsMatch(request.Password, @"[a-z]") || !Regex.IsMatch(request.Password, @"[A-Z]") || !Regex.IsMatch(request.Password, @"[!@#$%^&*(),.?""':{}|<>]"))
        {
            return BadRequest("The password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, and at least one special character.");
        }

        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };
        
        _storage.Add(user);
        
        return NoContent();
    }

}