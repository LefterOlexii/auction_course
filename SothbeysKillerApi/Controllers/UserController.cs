using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace SothbeysKillerApi.Controllers;

public record UserSignupRequest(string Name, string Email, string Password);
public record UserSigninRequest(string Email, string Password);
public record UserResponse(Guid Id, string Name, string Email);

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
    
    [HttpPost]
    [Route("[action]")]
    public IActionResult Signup(UserSignupRequest request)
    {
        try
        {
            var errors = new List<object>();
            
            if (request.Name.Length < 3 || request.Name.Length > 255)
            {
                errors.Add(new { target = "Name", description = "Name must be between 3 and 255 characters" });
            }
            
            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errors.Add(new { target = "Email", description = "Invalid email address" });
            }
            if (_storage.Any(user => user.Email == request.Email))
            {
                errors.Add(new { target = "Email", description = "Email is already taken" });
            }
            
            if (request.Password.Length < 8 || 
                !Regex.IsMatch(request.Password, @"[a-z]") || 
                !Regex.IsMatch(request.Password, @"[A-Z]") || 
                !Regex.IsMatch(request.Password, @"[!@#$%^&*(),.?""':{}|<>]"))
            {
                errors.Add(new { target = "Password", description = "Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, and at least one special character." });
            }
            
            if (errors.Count > 0)
            {
                return BadRequest(new { errors });
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
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Houston, we have a problem..." });
        }
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Signin(UserSigninRequest request)
    {
       
        try
        {   
            
            var user = _storage.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound(new { message = $"User with Email: {request.Email} does not exist." });
            }

            if (user.Password != request.Password)
            {
                return Unauthorized();
            }
            return Ok(new UserResponse(user.Id, user.Name, user.Email));
        }
        catch
        {
            return StatusCode(500, new { message = "Houston, we have a problem..." });
        }
    }



}