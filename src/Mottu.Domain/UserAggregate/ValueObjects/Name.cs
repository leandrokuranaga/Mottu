using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.UserAggregate.ValueObjects
{
    public sealed class PersonName : ValueObject
    {
        public string Value { get; }
        private PersonName(string value) => Value = value;

        public static PersonName Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new BusinessRulesException("Name is required.");
            var v = value.Trim();
            if (v.Length is < 2 or > 120) throw new BusinessRulesException("Name length invalid.");
            return new PersonName(v);
        }

        public override string ToString() => Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }
    }
}
