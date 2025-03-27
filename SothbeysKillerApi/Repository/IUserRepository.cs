using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public interface IUserRepository
{
    User SignUp(User entity);
    User? SignIn(UserSigninRequest request);
    bool IsEmailTaken(string email);
    
}