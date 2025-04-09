using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using SothbeysKillerApi.Services;

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
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    [Route("[action]")]
    public IActionResult Signup(UserSignupRequest request)
    {
        var id = _userService.SignUp(request);
        return Ok(new { Id = id });
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Signin(UserSigninRequest request)
    {

        try
        {
            var user = _userService.SignIn(request);
            return Ok(user);
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }



}