using FluentValidation;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Domain.MotorcycleAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Application.Validators.MotorcycleValidators
{
    [ExcludeFromCodeCoverage]
    public class CreateMotorcycleRequestValidator : AbstractValidator<CreateMotorcycleRequest>
    {
        public CreateMotorcycleRequestValidator()
        {
            RuleFor(x => x.Year)
                .GreaterThanOrEqualTo(1980).WithMessage("Year must be >= 1980.")
                .Must(y => y <= DateTime.UtcNow.Year + 1)
                .WithMessage(_ => $"Year must be <= {DateTime.UtcNow.Year + 1}.");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is mandatory.");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is mandatory.");

            RuleFor(x => x).Custom((req, ctx) =>
            {
                try { ManufactureYear.Create(req.Year); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.Year), ex.Message); }

                try { BrandName.Create(req.Brand); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.Brand), ex.Message); }

                try { LicensePlate.Create(req.LicensePlate); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.LicensePlate), ex.Message); }
            });
        }
    }
}
