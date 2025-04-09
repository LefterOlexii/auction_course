namespace SothbeysKillerApi.Exceptions;

public class UserValidationException: Exception
{
    public string Field { get; }
    public string Description { get; }

    public UserValidationException(string field, string description)
    {
        Field = field;
        Description = description;
    }
}