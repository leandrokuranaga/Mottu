using Mottu.Domain.MotorcycleAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class BrandNameTests
    {
        [Fact]
        public void Create_ShouldTrim_AndKeepCase()
        {
            var b = BrandName.Create("  Honda  ");
            Assert.Equal("Honda", b.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldThrow_WhenMissing(string? input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => BrandName.Create(input));
            Assert.Equal("Brand is mandatory.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenTooShort()
        {
            var ex = Assert.Throws<BusinessRulesException>(() => BrandName.Create("H"));
            Assert.Equal("Brand must be between 2 and 60 characters.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenTooLong()
        {
            var tooLong = new string('X', 61);
            var ex = Assert.Throws<BusinessRulesException>(() => BrandName.Create(tooLong));
            Assert.Equal("Brand must be between 2 and 60 characters.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenContainsControlChars()
        {
            var withCtrl = "Hon\nda";
            var ex = Assert.Throws<BusinessRulesException>(() => BrandName.Create(withCtrl));
            Assert.Equal("Brand contains invalid characters.", ex.Message);
        }
    }
}
