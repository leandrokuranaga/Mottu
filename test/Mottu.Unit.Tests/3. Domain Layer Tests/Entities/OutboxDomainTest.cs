using System;
using Mottu.Domain.OutboxAggregate;
using Xunit;

namespace Mottu.Unit.Tests.Domain.OutboxAggregate
{
    public class OutboxTests
    {
        [Fact]
        public void Create_ShouldSetTypeContentAndOccuredOnUtc()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var outbox = Outbox.Create("UserRegistered", "{\"id\":123}");

            var after = DateTime.UtcNow;

            // Assert
            Assert.Equal("UserRegistered", outbox.Type);
            Assert.Equal("{\"id\":123}", outbox.Content);
            Assert.Equal(DateTimeKind.Utc, outbox.OccuredOn.Kind);
            Assert.True(outbox.OccuredOn >= before && outbox.OccuredOn <= after);
            Assert.Null(outbox.ProcessedOn);
        }

        [Fact]
        public void Ctor_ShouldSetAllFields_AndNormalizeToUtc_WhenInputsAreLocal()
        {
            // Arrange
            var occuredLocal = DateTime.SpecifyKind(new DateTime(2025, 08, 23, 10, 30, 00), DateTimeKind.Local);
            var processedLocal = DateTime.SpecifyKind(new DateTime(2025, 08, 23, 11, 45, 00), DateTimeKind.Local);

            // Act
            var outbox = new Outbox("PaymentCaptured", "{}", occuredLocal, processedLocal);

            // Assert
            Assert.Equal("PaymentCaptured", outbox.Type);
            Assert.Equal("{}", outbox.Content);

            Assert.Equal(DateTimeKind.Utc, outbox.OccuredOn.Kind);
            Assert.Equal(occuredLocal, outbox.OccuredOn);

            Assert.NotNull(outbox.ProcessedOn);
            Assert.Equal(DateTimeKind.Utc, outbox.ProcessedOn!.Value.Kind);
            Assert.Equal(processedLocal, outbox.ProcessedOn!.Value);
        }

        [Fact]
        public void Ctor_ShouldSetOccuredOnUtc_WhenInputIsUnspecified()
        {
            // Arrange
            var unspecified = DateTime.SpecifyKind(new DateTime(2025, 08, 23, 12, 00, 00), DateTimeKind.Unspecified);

            // Act
            var outbox = new Outbox("InvoiceIssued", "{}", unspecified);

            // Assert
            Assert.Equal(DateTimeKind.Utc, outbox.OccuredOn.Kind);
            Assert.Equal(unspecified, outbox.OccuredOn);
            Assert.Null(outbox.ProcessedOn);
        }

        [Fact]
        public void Ctor_ShouldAllow_NullProcessedOn()
        {
            var occured = DateTime.UtcNow.AddMinutes(-5);
            var outbox = new Outbox("Any", "{}", occured, null);

            Assert.Equal(DateTimeKind.Utc, outbox.OccuredOn.Kind);
            Assert.Null(outbox.ProcessedOn);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_ShouldThrow_WhenTypeIsInvalid(string type)
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Outbox(type!, "{}", DateTime.UtcNow));

            Assert.Equal("type", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_ShouldThrow_WhenContentIsInvalid(string content)
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new Outbox("Evt", content!, DateTime.UtcNow));

            Assert.Equal("content", ex.Message);
        }

        [Fact]
        public void Create_ShouldNotMutateProcessedOn()
        {
            var outbox = Outbox.Create("Evt", "{}");
            Assert.Null(outbox.ProcessedOn);
        }
    }
}
