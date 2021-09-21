using System;
using FluentValidation;

namespace News.API.Commands
{
    public class NewsCommandValidator : AbstractValidator<NewsCommand>
    {
        public NewsCommandValidator()
        {
            RuleFor(x => x.BeginDate)
                .NotEmpty().WithMessage("Begin date required")
                .NotNull().WithMessage("Begin date required");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate date required")
                .NotNull().WithMessage("EndDate date required")
                .GreaterThanOrEqualTo(r => r.BeginDate)
                .WithMessage("End date must be greater than begin date.");
            
            RuleFor(x => x.Topic)
                .NotEmpty().WithMessage("Topic is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("Topic must not exceed 50 characters.");
        }
    }
}