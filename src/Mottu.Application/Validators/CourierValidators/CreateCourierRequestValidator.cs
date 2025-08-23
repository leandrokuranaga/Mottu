using FluentValidation;
using Mottu.Application.Courier.Models.Request;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.Enums;
using Mottu.Domain.UserAggregate.ValueObjects;

namespace Mottu.Application.Validators.CourierValidators
{
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
                .NotEmpty().WithMessage("BirthdayDate is required.")
                .Must(d => d.Date <= DateTime.UtcNow.Date)
                .WithMessage("BirthdayDate cannot be in the future.")
                .Must(d => IsAtLeastAge(d, 18))
                .WithMessage("Courier must be at least 18 years old.");

            RuleFor(x => x.CNPJ)
                .NotEmpty().WithMessage("CNPJ is required.");

            RuleFor(x => x.TypeCNH)
                .NotEmpty().WithMessage("TypeCNH is required.")
                .Must(s => TryParseCnhCategory(s, out _))
                .WithMessage("TypeCNH must be A, B or AB.");

            RuleFor(x => x.CNH)
                .NotEmpty().WithMessage("CNH number is required.");

            When(x => !string.IsNullOrWhiteSpace(x.CNHUri), () =>
            {
                RuleFor(x => x.CNHUri!)
                    .Must(IsValidHttpUrl)
                    .WithMessage("CNHUri must be a valid http/https URL.");
            });

            RuleFor(x => x).Custom((req, ctx) =>
            {
                try { PersonName.Create(req.Name); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.Name), ex.Message); }

                try { CNPJ.Create(req.CNPJ); }
                catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.CNPJ), ex.Message); }

                if (TryParseCnhCategory(req.TypeCNH, out var cat))
                {
                    try { CNH.Create(req.CNH, cat); }
                    catch (BusinessRulesException ex) { ctx.AddFailure(nameof(req.CNH), ex.Message); }
                }
                else
                {
                    ctx.AddFailure(nameof(req.TypeCNH), "Invalid CNH category. Allowed: A, B, AB.");
                }
            });
        }

        private static bool IsAtLeastAge(DateTime birth, int years)
        {
            var today = DateTime.UtcNow.Date;
            var b = birth.Date;
            var age = today.Year - b.Year;
            if (b > today.AddYears(-age)) age--;
            return age >= years;
        }

        private static bool TryParseCnhCategory(string? s, out ECNH cat)
        {
            cat = default;
            if (string.IsNullOrWhiteSpace(s)) return false;

            var norm = s.Trim().ToUpperInvariant()
                        .Replace("+", "")
                        .Replace("-", "")
                        .Replace(" ", "");

            switch (norm)
            {
                case "A": cat = ECNH.A; return true;
                case "B": cat = ECNH.B; return true;
                case "AB":
                case "BA": cat = ECNH.AB; return true;
                default: return false;
            }
        }

        private static bool IsValidHttpUrl(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out var u)
                   && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
        }
    }
}
