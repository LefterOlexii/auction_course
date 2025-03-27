using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public class MongoUserRepository : IUserRepository
{
    public User SignUp(User entity)
    {
        throw new NotImplementedException();
    }
    public User? SignIn(UserSigninRequest request)
    {
        throw new NotImplementedException();
    }

    public bool IsEmailTaken(string email)
    {
        throw new NotImplementedException();
    }
}