using Mottu.Domain.RentalAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests._3._Domain_Layer_Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Ctor_ShouldAccept_ZeroAndPositive_DefaultBRL()
        {
            var m0 = new Money(0);
            var m1 = new Money(10.5m);

            Assert.Equal(0, m0.Value);
            Assert.Equal("BRL", m0.Currency);

            Assert.Equal(10.5m, m1.Value);
            Assert.Equal("BRL", m1.Currency);
        }

        [Fact]
        public void Ctor_ShouldAccept_ValidCurrencies_CaseInsensitive()
        {
            var usd = new Money(99.99m, "usd");
            var eur = new Money(50m, "EUR");

            Assert.Equal("USD", usd.Currency);
            Assert.Equal("EUR", eur.Currency);
        }

        [Fact]
        public void Ctor_ShouldThrow_WhenNegative()
        {
            var ex = Assert.Throws<BusinessRulesException>(() => new Money(-1m));
            Assert.Equal("The price must be greater than or equal to 0.", ex.Message);
        }

        [Fact]
        public void Ctor_ShouldThrow_WhenCurrencyInvalid()
        {
            var ex = Assert.Throws<BusinessRulesException>(() => new Money(10m, "XYZ"));
            Assert.Contains("Invalid currency: XYZ.", ex.Message);
        }
    }
}
