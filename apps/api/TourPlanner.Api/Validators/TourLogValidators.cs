using FluentValidation;
using TourPlanner.Api.DTOs;

namespace TourPlanner.Api.Validators;

public class CreateTourLogRequestValidator : AbstractValidator<CreateTourLogRequest>
{
    public CreateTourLogRequestValidator()
    {
        RuleFor(x => x.Comment).MaximumLength(2000);
        RuleFor(x => x.Difficulty).IsInEnum();
        RuleFor(x => x.Rating).IsInEnum();
        RuleFor(x => x.TotalDistanceKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalTimeSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DateTime).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
    }
}

public class UpdateTourLogRequestValidator : AbstractValidator<UpdateTourLogRequest>
{
    public UpdateTourLogRequestValidator()
    {
        RuleFor(x => x.Comment).MaximumLength(2000);
        RuleFor(x => x.Difficulty).IsInEnum();
        RuleFor(x => x.Rating).IsInEnum();
        RuleFor(x => x.TotalDistanceKm).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TotalTimeSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DateTime).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));
    }
}
