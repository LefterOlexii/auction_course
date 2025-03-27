using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Repository;

namespace SothbeysKillerApi.Services;

public class DbUserService: IUserService
{
    // private readonly IDbConnection _dbConnection;
    private readonly IUserRepository _userRepository;

    public DbUserService(IUserRepository userRepository)
    {
        // _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;");
        // _dbConnection.Open();
        _userRepository = userRepository;
    }
    
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

        if (_userRepository.IsEmailTaken(request.Email))
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
        
        var created = _userRepository.SignUp(user);
        
        return created.Id;
    }
    
    public UserResponse SignIn(UserSigninRequest request)
    {
        var user = _userRepository.SignIn(request);

        if (user is null)
        {
            throw new NullReferenceException($"User with Email: {request.Email} does not exist.");
        }

        if (user.Password != request.Password)
        {
            throw new UnauthorizedAccessException();
        }
        return new UserResponse(user.Id, user.Name, user.Email);
    }
    
}