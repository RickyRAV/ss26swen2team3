namespace TourPlanner.BL.Exceptions;

public class TourLogNotFoundException : BusinessLogicException
{
    public TourLogNotFoundException(Guid logId) : base($"Tour log with ID '{logId}' was not found.") { }
}
