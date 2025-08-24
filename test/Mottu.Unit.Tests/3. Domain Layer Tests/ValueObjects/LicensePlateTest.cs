using Mottu.Domain.MotorcycleAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class LicensePlateTests
    {
        [Theory]
        [InlineData("AAA-1234", "AAA1234")] 
        [InlineData("AAA1234", "AAA1234")] 
        [InlineData("ABC1D23", "ABC1D23")] 
        public void Create_ShouldNormalize_ToUpper_NoHyphen(string input, string expected)
        {
            var lp = LicensePlate.Create(input);
            Assert.Equal(expected, lp.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_ShouldThrow_WhenMissing(string? input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => LicensePlate.Create(input));
            Assert.Equal("License plate is mandatory.", ex.Message);
        }

        [Theory]
        [InlineData("AA-12345")] 
        [InlineData("ABCD123")]  
        [InlineData("A1C1D23")]  
        public void Create_ShouldThrow_WhenFormatInvalid(string input)
        {
            var ex = Assert.Throws<BusinessRulesException>(() => LicensePlate.Create(input));
            Assert.Equal("License plate invalid format.", ex.Message);
        }
    }
}
