using System.Text.RegularExpressions;
using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.MotorcycleAggregate.ValueObjects
{
    public sealed class LicensePlate : ValueObject
    {
        public string Value { get; }

        private LicensePlate() { }
        private LicensePlate(string value) => Value = value;

        private static readonly Regex OldPattern =
            new(@"^[A-Z]{3}[0-9]{4}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex MercosulPattern =
            new(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Aceita AAA-1234/AAA1234 (antiga) e ABC1D23 (Mercosul). Normaliza para maiúsculas sem hífen.
        /// </summary>
        public static LicensePlate Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRulesException("License plate is mandatory.");

            var raw = value.Trim().ToUpperInvariant().Replace("-", "");

            if (!OldPattern.IsMatch(raw) && !MercosulPattern.IsMatch(raw))
                throw new BusinessRulesException("License plate invalid format.");

            return new LicensePlate(raw);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
