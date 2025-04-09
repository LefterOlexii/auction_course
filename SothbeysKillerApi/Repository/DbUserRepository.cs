using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;


public class DbUserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _transaction;

    public DbUserRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _dbConnection = connection;
        _transaction = transaction;
    }

    public User SignUp(User entity)
    {
        var command = $@"insert into users (id, name, email, password) values (@Id, @Name, @Email, @Password) returning id;";
        
        var user = _dbConnection.QueryFirst<User>(command, entity, transaction: _transaction);

        return user;
    }
    
    
    public User? SignIn(UserSigninRequest request)
    {
        var command = $@"SELECT * FROM users WHERE email = @Email LIMIT 1";
        
        var user = _dbConnection.QueryFirstOrDefault<User>(command, new { Email = request.Email }, transaction: _transaction);

        return user;
    }
    
    public bool IsEmailTaken(string email)
    {
        var query = "SELECT COUNT(1) FROM users WHERE email = @Email";
        return _dbConnection.ExecuteScalar<int>(query, new { Email = email }, transaction: _transaction) > 0;
    }
    
}