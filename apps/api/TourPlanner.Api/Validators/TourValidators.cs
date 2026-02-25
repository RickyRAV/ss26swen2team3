using FluentValidation;
using TourPlanner.Api.DTOs;

namespace TourPlanner.Api.Validators;

public class CreateTourRequestValidator : AbstractValidator<CreateTourRequest>
{
    public CreateTourRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.From).NotEmpty().MaximumLength(300);
        RuleFor(x => x.To).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TransportType).IsInEnum();
    }
}

public class UpdateTourRequestValidator : AbstractValidator<UpdateTourRequest>
{
    public UpdateTourRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.From).NotEmpty().MaximumLength(300);
        RuleFor(x => x.To).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.TransportType).IsInEnum();
        RuleFor(x => x.Distance).GreaterThan(0).When(x => x.Distance.HasValue);
        RuleFor(x => x.EstimatedTimeSeconds).GreaterThan(0).When(x => x.EstimatedTimeSeconds.HasValue);
    }
}
