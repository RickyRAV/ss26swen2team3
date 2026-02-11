namespace TourPlanner.BL.Exceptions;

public class AuthException : BusinessLogicException
{
    public AuthException(string message) : base(message) { }
}
