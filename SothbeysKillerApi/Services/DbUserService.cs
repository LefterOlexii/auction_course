using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Repository;
using SothbeysKillerApi.Contexts;
using SothbeysKillerApi.Exceptions;

namespace SothbeysKillerApi.Services;

public class DbUserService: IUserService
{
    // private readonly IDbConnection _dbConnection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserDbContext _userDbContext;

    public DbUserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Guid SignUp(UserSignupRequest request)
    {
        if (request.Name.Length < 3 || request.Name.Length > 255)
        {
            throw new UserValidationException(nameof(request.Name),"Name must be between 3 and 255 characters");
        }
            
        if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new UserValidationException(nameof(request.Email),"Invalid email address");
        }

        if (_unitOfWork.UserRepository.IsEmailTaken(request.Email))
        {
            throw new UserValidationException(nameof(request.Email),"Email is already taken");
        }
        
        if (request.Password.Length < 8 || 
            !Regex.IsMatch(request.Password, @"[a-z]") || 
            !Regex.IsMatch(request.Password, @"[A-Z]") || 
            !Regex.IsMatch(request.Password, @"[!@#$%^&*(),.?""':{}|<>]"))
        {
            throw new UserValidationException(nameof(request.Password),"Password must be at least 8 characters long, contain at least one lowercase letter, one uppercase letter, and at least one special character.");
        }
    
        var user = new User()
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        _userDbContext.Type.Add(user);
        
        _userDbContext.SaveChanges();
        
        return user.Id;
    }
    
    public UserResponse SignIn(UserSigninRequest request)
    {
        var user = _userDbContext.Type.FirstOrDefault(a => a.Email == request.Email);
        
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