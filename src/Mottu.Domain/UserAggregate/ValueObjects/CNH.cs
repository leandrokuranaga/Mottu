using System.Text.RegularExpressions;
using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.Enums;

namespace Mottu.Domain.UserAggregate.ValueObjects
{
    public sealed class CNH : ValueObject
    {
        public string Number { get; private set; } = null!;
        public ECNH Category { get; private set; }

        private CNH() { }

        private CNH(string numberDigits, ECNH category)
        {
            Number = numberDigits;
            Category = category;
        }

        public static CNH Create(string? number, ECNH category)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new BusinessRulesException("CNH number is required.");

            var digits = DigitsOnly(number);

            if (digits.Length != 11)
                throw new BusinessRulesException("CNH number must have 11 digits.");

            if (AllCharsEqual(digits))
                throw new BusinessRulesException("CNH number cannot have all digits equal.");

            if (!IsValidCategory(category))
                throw new BusinessRulesException("Invalid CNH category. Allowed: A, B, A+B.");

            return new CNH(digits, category);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number;
            yield return Category;
        }

        private static string DigitsOnly(string s) => Regex.Replace(s, "[^0-9]", "");

        private static bool AllCharsEqual(string s)
        {
            for (int i = 1; i < s.Length; i++)
                if (s[i] != s[0]) return false;
            return true;
        }

        private static bool IsValidCategory(ECNH cat) =>
            cat == ECNH.A || cat == ECNH.B || cat == ECNH.AB;

        public override string ToString() => $"{Number}-{Category}";
    }
}
