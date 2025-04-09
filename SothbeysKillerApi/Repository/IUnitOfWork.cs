namespace SothbeysKillerApi.Repository;

public interface IUnitOfWork
{
    IAuctionRepository AuctionRepository { get; }
    IAuctionHistoryRepository AuctionHistoryRepository { get; }
    
    IUserRepository UserRepository { get; }
    IUserHistoryRepository UserHistoryRepository { get; }

    void Commit();

    void Rollback();
}