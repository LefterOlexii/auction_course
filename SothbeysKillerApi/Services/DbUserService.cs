using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public class DbUserService: IUserService
{
    private readonly IDbConnection _dbConnection;

    public DbUserService()
    {
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;");
        _dbConnection.Open();

        EnsureUsersTableExists();
    }
    
    private void EnsureUsersTableExists()
    {
        var checkTableQuery = @"SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'users');";
        bool tableExists = _dbConnection.ExecuteScalar<bool>(checkTableQuery);
        if (!tableExists)
        {
            // Створення таблиці, якщо вона не існує
            var createTableQuery = @"
                CREATE TABLE users (
                    Id UUID PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL,
                    Email VARCHAR(255) NOT NULL,
                    Password TEXT NOT NULL
                );";
            _dbConnection.Execute(createTableQuery);

        }
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
        
        var checkEmailCommand = "SELECT COUNT(1) FROM users WHERE email = @Email;";
        var emailExists = _dbConnection.ExecuteScalar<int>(checkEmailCommand, new { Email = request.Email }) > 0;
        if (emailExists)
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
        
        var command = $@"insert into users (id, name, email, password) values (@Id, @Name, @Email, @Password) returning id;";
        
        var id = _dbConnection.ExecuteScalar<Guid>(command, user);
        
        return id;
    }
    
    public UserResponse SignIn(UserSigninRequest request)
    {
        var user = _dbConnection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE email = @Email LIMIT 1", new { Email = request.Email });
        
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