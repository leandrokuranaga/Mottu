using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.ValueObjects;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class CNPJTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldThrow_WhenMissing(string? input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNPJ.Create(input));
            Assert.Equal("CNPJ is required.", ex.Message);
        }

        [Theory]
        [InlineData("1234567890123")]  
        [InlineData("123456789012345")] 
        public void Create_ShouldThrow_WhenLengthNot14(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNPJ.Create(input));
            Assert.Equal("CNPJ must have 14 digits.", ex.Message);
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        public void Create_ShouldThrow_WhenAllDigitsEqual(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNPJ.Create(input));
            Assert.Equal("CNPJ cannot have all digits equal.", ex.Message);
        }

        [Theory]
        [InlineData("04252011000111")]
        [InlineData("11222333000180")]
        [InlineData("12345678000190")]
        public void Create_ShouldThrow_WhenChecksumInvalid(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNPJ.Create(input));
            Assert.Equal("CNPJ is invalid.", ex.Message);
        }

    }
}
