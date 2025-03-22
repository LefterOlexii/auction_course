using SothbeysKillerApi.Controllers;
using System.Text.RegularExpressions;

namespace SothbeysKillerApi.Services;

public interface IUserService
{
    Guid SignUp(UserSignupRequest request);
    UserResponse SignIn(UserSigninRequest request);

}

public class UserService : IUserService
{
    private readonly List<User> _users = [];

    public Guid SignUp(UserSignupRequest request)
    {

        if (request.Name.Length < 3 || request.Name.Length > 255)
        {
            throw new ArgumentException("Name must be between 3 and 255 characters");
        }
            
        if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email address");
        }
        if (_users.Any(user => user.Email == request.Email))
        {
            throw new ArgumentException("Email is already taken");
        }
        
        if (request.Password.Length < 8 || 
            !Regex.IsMatch(request.Password, @"[a-z]") || 
            !Regex.IsMatch(request.Password, @"[A-Z]") || 
            !Regex.IsMatch(request.Password, @"[!@#$%^&*(),.?""':{}|<>]"))
        {
            throw new ArgumentException("Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, and at least one special character.");
        }
    
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };
        
        _users.Add(user);
        
        return user.Id;

    }

    public UserResponse SignIn(UserSigninRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
        {
            throw new ArgumentException($"User with Email: {request.Email} does not exist.");
        }

        if (user.Password != request.Password)
        {
            throw new UnauthorizedAccessException();
        }

        return new UserResponse(user.Id, user.Name, user.Email);

    }

}
