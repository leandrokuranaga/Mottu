using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.MotorcycleAggregate.ValueObjects
{
    public sealed class BrandName : ValueObject
    {
        public string Value { get; }

        private BrandName() { }
        private BrandName(string value) => Value = value;

        public static BrandName Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRulesException("Brand is mandatory.");

            var normalized = value.Trim();

            if (normalized.Length is < 2 or > 60)
                throw new BusinessRulesException("Brand must be between 2 and 60 characters.");

            if (normalized.Any(char.IsControl))
                throw new BusinessRulesException("Brand contains invalid characters.");

            return new BrandName(normalized);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
