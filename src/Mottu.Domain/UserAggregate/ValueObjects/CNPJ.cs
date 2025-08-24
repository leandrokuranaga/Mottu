using System.Text.RegularExpressions;
using Abp.Domain.Values;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.UserAggregate.ValueObjects
{
    public sealed class CNPJ : ValueObject
    {
        public string Number { get; private set; } = null!;

        private CNPJ(string digits14) => Number = digits14;

        private CNPJ() { }

        public static CNPJ Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRulesException("CNPJ is required.");

            var digits = DigitsOnly(value);

            if (digits.Length != 14)
                throw new BusinessRulesException("CNPJ must have 14 digits.");

            if (AllCharsEqual(digits))
                throw new BusinessRulesException("CNPJ cannot have all digits equal.");

            if (!IsValidChecksum(digits))
                throw new BusinessRulesException("CNPJ is invalid.");

            return new CNPJ(digits);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number;
        }

        private static string DigitsOnly(string s) => Regex.Replace(s, "[^0-9]", "");

        private static bool AllCharsEqual(string s)
        {
            for (int i = 1; i < s.Length; i++)
                if (s[i] != s[0]) return false;
            return true;
        }

        private static bool IsValidChecksum(string d)
        {
            int[] m1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] m2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int Calc(string src, int[] mult)
            {
                int sum = 0;
                for (int i = 0; i < mult.Length; i++) sum += (src[i] - '0') * mult[i];
                int r = sum % 11;
                return (r < 2) ? 0 : 11 - r;
            }

            var dv1 = Calc(d[..12], m1);
            var dv2 = Calc(d[..12] + dv1, m2);

            return d[12] - '0' == dv1 && d[13] - '0' == dv2;
        }
    }
}
