using System.Data;
using Dapper;

namespace SothbeysKillerApi.Repository;

public class UserHistory
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IUserHistoryRepository
{
    int Create(UserHistory entity);
}

public class UserHistoryRepository : IUserHistoryRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _transaction;

    public UserHistoryRepository(IDbConnection dbConnection, IDbTransaction transaction)
    {
        _dbConnection = dbConnection;
        _transaction = transaction;
    }

    public int Create(UserHistory entity)
    {
        var command =
            "insert into user_history (user_id, created_at) values (@UserId, @CreatedAt) returning id;";

        return _dbConnection.ExecuteScalar<int>(command, entity, _transaction);
    }
}