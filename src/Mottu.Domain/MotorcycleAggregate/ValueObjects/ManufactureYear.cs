using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.MotorcycleAggregate.ValueObjects
{
    public sealed class ManufactureYear : ValueObject
    {
        public int Value { get; }

        private ManufactureYear() { }
        private ManufactureYear(int value) => Value = value;

        public static ManufactureYear Create(int value)
        {
            var current = DateTime.UtcNow.Year;
            if (value < 1980 || value > current + 1)
                throw new BusinessRulesException($"Invalid year: {value}. Accepted range is 1980 to {current + 1}.");

            return new ManufactureYear(value);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
