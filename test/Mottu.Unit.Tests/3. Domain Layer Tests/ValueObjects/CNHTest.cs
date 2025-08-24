using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.Enums;
using Mottu.Domain.UserAggregate.ValueObjects;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class CNHTests
    {
        [Fact]
        public void Create_ShouldNormalize_DigitsOnly_AndSetCategory()
        {
            var cnh = CNH.Create("123.456.789-00", ECNH.AB);

            Assert.Equal("12345678900", cnh.Number);
            Assert.Equal(ECNH.AB, cnh.Category);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldThrow_WhenNumberMissing(string? input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNH.Create(input, ECNH.A));
            Assert.Equal("CNH number is required.", ex.Message);
        }

        [Theory]
        [InlineData("1234567890")]
        [InlineData("123456789000")]
        public void Create_ShouldThrow_WhenNumberLengthNot11(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNH.Create(input, ECNH.B));
            Assert.Equal("CNH number must have 11 digits.", ex.Message);
        }

        [Theory]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("22222222222")]
        public void Create_ShouldThrow_WhenAllDigitsEqual(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => CNH.Create(input, ECNH.A));
            Assert.Equal("CNH number cannot have all digits equal.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenCategoryInvalid()
        {
            var invalid = (ECNH)999;
            var ex = Assert.Throws<BusinessRulesException>(() => CNH.Create("12345678900", invalid));
            Assert.Equal("Invalid CNH category. Allowed: A, B, A+B.", ex.Message);
        }
    }
}
