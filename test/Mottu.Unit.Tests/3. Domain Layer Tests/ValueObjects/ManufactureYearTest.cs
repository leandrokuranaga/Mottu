using Mottu.Domain.MotorcycleAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class ManufactureYearTests
    {
        [Fact]
        public void Create_ShouldAccept_From1980_ToNextYear()
        {
            var current = DateTime.UtcNow.Year;

            var y1 = ManufactureYear.Create(1980);
            var y2 = ManufactureYear.Create(current);
            var y3 = ManufactureYear.Create(current + 1);

            Assert.Equal(1980, y1.Value);
            Assert.Equal(current, y2.Value);
            Assert.Equal(current + 1, y3.Value);
        }

        [Fact]
        public void Create_ShouldThrow_WhenBelow1980()
        {
            var ex = Assert.Throws<BusinessRulesException>(() => ManufactureYear.Create(1979));
            Assert.StartsWith("Invalid year:", ex.Message);
        }

        [Fact]
        public void Create_ShouldThrow_WhenAboveNextYear()
        {
            var current = DateTime.UtcNow.Year;
            var ex = Assert.Throws<BusinessRulesException>(() => ManufactureYear.Create(current + 2));
            Assert.StartsWith("Invalid year:", ex.Message);
        }
    }
}
