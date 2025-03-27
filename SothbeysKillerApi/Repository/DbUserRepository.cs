using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;


public class DbUserRepository : IUserRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<DbUserRepository> _logger;

    public DbUserRepository(ILogger<DbUserRepository> logger)
    {
        _logger = logger;
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=root;");
        _dbConnection.Open();
        _logger.LogInformation($"{DateTime.Now}: connection state: {_dbConnection.State}.");
    }

    public User SignUp(User entity)
    {
        var command = $@"insert into users (id, name, email, password) values (@Id, @Name, @Email, @Password) returning id;";
        
        var user = _dbConnection.QueryFirst<User>(command, entity);

        return user;
    }
    
    
    public User? SignIn(UserSigninRequest request)
    {
        var command = $@"SELECT * FROM users WHERE email = @Email LIMIT 1";
        
        var user = _dbConnection.QueryFirstOrDefault<User>(command, new { Email = request.Email });

        return user;
    }
    
    public bool IsEmailTaken(string email)
    {
        var query = "SELECT COUNT(1) FROM users WHERE email = @Email";
        return _dbConnection.ExecuteScalar<int>(query, new { Email = email }) > 0;
    }
    
    public void Dispose()
    {
        _logger.LogInformation($"{DateTime.Now}: {nameof(DbAuctionRepository)} disposed.");
        _dbConnection.Dispose();
        
        _logger.LogInformation($"{DateTime.Now}: connection state: {_dbConnection.State}.");
    }
}