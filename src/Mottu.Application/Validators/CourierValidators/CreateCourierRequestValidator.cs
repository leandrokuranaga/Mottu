using FluentValidation;
using Mottu.Application.Courier.Models.Request;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.Enums;
using Mottu.Domain.UserAggregate.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Application.Validators.CourierValidators
{
    [ExcludeFromCodeCoverage]
    public class CreateCourierRequestValidator : AbstractValidator<CreateCourierRequest>
    {
        public CreateCourierRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Must(name =>
                {
                    try { PersonName.Create(name); return true; }
                    catch (BusinessRulesException) { return false; }
                })
                .WithMessage("Name length invalid.");

            RuleFor(x => x.BirthdayDate)
                .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("BirthdayDate cannot be in the future.")
                .Must(d => IsAtLeastAge(d, 18))
                .WithMessage("Courier must be at least 18 years old.");

            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("CNPJ is required.");

            RuleFor(x => x.TypeCNH)
                .Must(cat => cat == ECNH.A || cat == ECNH.B || cat == ECNH.AB)
                .WithMessage("TypeCNH must be A, B or AB.");

            RuleFor(x => x.CNH)
                .NotEmpty().WithMessage("CNH number is required.");

            RuleFor(x => x).Custom((req, ctx) =>
            {
                try { PersonName.Create(req.Name); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.Name), ex.Message); }

                try { CNPJ.Create(req.CNPJ); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.CNPJ), ex.Message); }

                try { CNH.Create(req.CNH, req.TypeCNH); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.CNH), ex.Message); }
            });
        }

        private static bool IsAtLeastAge(DateOnly birth, int years)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birth.Year;
            if (birth > today.AddYears(-age)) age--;
            return age >= years;
        }
    }
}
