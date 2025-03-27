using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public class InMemoryUserRepository : IUserRepository
{
    public User SignUp(User user)
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