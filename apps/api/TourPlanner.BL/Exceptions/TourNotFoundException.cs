namespace TourPlanner.BL.Exceptions;

public class TourNotFoundException : BusinessLogicException
{
    public TourNotFoundException(Guid tourId) : base($"Tour with ID '{tourId}' was not found.") { }
}
