using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests.Domain.Entities
{
    public class MotorcycleDomainTest
    {
        [Fact]
        public void ParameterlessConstructor_ShouldCreateInstance()
        {
            var moto = new Motorcycle();

            Assert.NotNull(moto);
            Assert.Null(moto.Brand);
            Assert.Null(moto.LicensePlate);
            Assert.Null(moto.Year);
            Assert.False(moto.IsDeleted);
        }

        [Fact]
        public void Create_ShouldSetProperties()
        {
            var moto = Motorcycle.Create(2024, "Honda", "ABC1234");

            Assert.NotNull(moto.Year);
            Assert.Equal(2024, moto.Year.Value);

            Assert.NotNull(moto.Brand);
            Assert.Equal("Honda", moto.Brand.Value);

            Assert.NotNull(moto.LicensePlate);
            Assert.Equal("ABC1234", moto.LicensePlate.Value);

            Assert.False(moto.IsDeleted);
            Assert.NotEqual(default, moto.CreationTime);
            Assert.Null(moto.LastModificationTime);
        }

        [Fact]
        public void ChangePlate_ShouldUpdateLicensePlate_AndLastModificationTime()
        {
            var moto = Motorcycle.Create(2024, "Yamaha", "XYZ9A23");

            var oldTime = moto.LastModificationTime;
            moto.ChangePlate("NEW1234");

            Assert.Equal("NEW1234", moto.LicensePlate.Value);
            Assert.NotNull(moto.LastModificationTime);
            Assert.NotEqual(oldTime, moto.LastModificationTime);
        }

        [Fact]
        public void ChangePlate_ShouldNotUpdate_WhenSamePlate()
        {
            var moto = Motorcycle.Create(2024, "Yamaha", "XYZ9A23");

            var lastTime = moto.LastModificationTime;
            moto.ChangePlate("XYZ9A23");

            Assert.Equal("XYZ9A23", moto.LicensePlate.Value);
            Assert.Equal(lastTime, moto.LastModificationTime);
        }

        [Fact]
        public void MarkAsDeleted_ShouldSetIsDeleted_AndLastModificationTime()
        {
            var moto = Motorcycle.Create(2024, "Honda", "AAA0001");

            moto.MarkAsDeleted();

            Assert.True(moto.IsDeleted);
            Assert.NotNull(moto.LastModificationTime);
        }

        [Fact]
        public void MarkAsDeleted_ShouldThrow_WhenAlreadyDeleted()
        {
            var moto = Motorcycle.Create(2023, "Suzuki", "BBB2222");
            moto.MarkAsDeleted();

            var ex = Assert.Throws<BusinessRulesException>(() => moto.MarkAsDeleted());
            Assert.Equal("Motorcycle already deleted.", ex.Message);
        }

        [Fact]
        public void ChangePlate_ShouldThrow_WhenAlreadyDeleted()
        {
            var moto = Motorcycle.Create(2022, "Honda", "CCC3333");
            moto.MarkAsDeleted();

            var ex = Assert.Throws<BusinessRulesException>(() => moto.ChangePlate("DDD4444"));
            Assert.Equal("Motorcycle already deleted.", ex.Message);
        }
    }
}
