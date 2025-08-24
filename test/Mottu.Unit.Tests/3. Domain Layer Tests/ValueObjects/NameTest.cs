using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.ValueObjects;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class PersonNameTests
    {
        [Fact]
        public void Create_ShouldTrim_AndAccept2to120Chars()
        {
            var p = PersonName.Create("  João da Silva  ");
            Assert.Equal("João da Silva", p.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldThrow_WhenMissing(string? input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => PersonName.Create(input));
            Assert.Equal("Name is required.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenTooShort()
        {
            var ex = Assert.Throws<BusinessRulesException>(() => PersonName.Create("A"));
            Assert.Equal("Name length invalid.", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenTooLong()
        {
            var longName = new string('X', 121);
            var ex = Assert.Throws<BusinessRulesException>(() => PersonName.Create(longName));
            Assert.Equal("Name length invalid.", ex.Message);
        }
    }
}
