using System.Data;
using Npgsql;

namespace SothbeysKillerApi.Repository;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _transaction;
    
    public IAuctionRepository AuctionRepository { get; }
    public IAuctionHistoryRepository AuctionHistoryRepository { get; }
    
    public IUserRepository UserRepository { get; }
    public IUserHistoryRepository UserHistoryRepository { get; }

    public UnitOfWork()
    {
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;Username=postgres;Password=root");
        _dbConnection.Open();

        _transaction = _dbConnection.BeginTransaction();

        AuctionRepository = new DbAuctionRepository(_dbConnection, _transaction);
        AuctionHistoryRepository = new AuctionHistoryRepository(_dbConnection, _transaction);
        
        UserRepository = new DbUserRepository(_dbConnection, _transaction);
        UserHistoryRepository = new UserHistoryRepository(_dbConnection, _transaction);
    }
    
    public void Commit()
    {
        try
        {
            _transaction.Commit();
        }
        catch (Exception e)
        {
            _transaction.Rollback();
        }
        finally
        {
            _transaction.Dispose();
            _dbConnection.Dispose();
        }
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        try
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("Transaction already disposed.");
        }
        
        _dbConnection.Dispose();
    }
}