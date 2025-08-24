using Mottu.Domain.RentalAggregate;
using Mottu.Domain.RentalAggregate.Enums;
using Mottu.Domain.RentalAggregate.Services;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Unit.Tests.Domain.RentalAggregate
{
    public class RentalTests_NoAbstractions
    {
        [Fact]
        public void Create_ShouldInitialize_WithCatalogValues_AndDatesRelativeToUtcNow()
        {
            // Arrange
            var plan = ERentalPlan.Days7;
            var (daily, _, days) = RentalPlanCatalog.Get(plan);
            var before = DateTime.UtcNow;

            var expectedToday = DateOnly.FromDateTime(before);
            var expectedStart = expectedToday.AddDays(1);
            var expectedForecastEnd = expectedStart.AddDays(days - 1);

            // Act
            var rental = Rental.Create(1, 2, plan);
            var after = DateTime.UtcNow;

            // Assert
            Assert.Equal(1, rental.MotorcycleId);
            Assert.Equal(2, rental.CourierId);
            Assert.Equal(plan, rental.Plan);

            Assert.NotNull(rental.DailyPrice);
            Assert.Equal(daily, rental.DailyPrice.Value);

            Assert.Equal(expectedStart, rental.StartDate);
            Assert.Equal(expectedForecastEnd, rental.ForecastEndDate);

            Assert.True(rental.CreatedAtUtc >= before && rental.CreatedAtUtc <= after);

            Assert.Equal(ERentalStatus.Pending, rental.Status);
            Assert.Null(rental.TotalPrice);
            Assert.Null(rental.EndDate);
        }

        [Fact]
        public void Return_OnForecastDate_ShouldChargeOnlyDailyBasis_NoFees()
        {
            // Arrange
            var plan = ERentalPlan.Days7;
            var (daily, _, days) = RentalPlanCatalog.Get(plan);
            var rental = Rental.Create(10, 20, plan);

            var end = rental.ForecastEndDate;

            // Act
            var (total, dailyBasis, fee, isEarly, isLate) = rental.Return(end);

            // Assert
            var daysUsed = days; 
            Assert.Equal(daysUsed * daily, dailyBasis);
            Assert.Equal(0m, fee);
            Assert.Equal(dailyBasis, total);

            Assert.False(isEarly);
            Assert.False(isLate);
            Assert.Equal(ERentalStatus.Closed, rental.Status);
            Assert.Equal(end, rental.EndDate);
            Assert.NotNull(rental.TotalPrice);
            Assert.Equal(total, rental.TotalPrice!.Value);
            Assert.Equal(rental.DailyPrice.Currency, rental.TotalPrice.Currency);
        }

        [Fact]
        public void Return_Early_ShouldApplyEarlyFee_OnNotUsedDays_WhenCatalogHasEarlyPct()
        {
            // Arrange
            var plan = ERentalPlan.Days7;
            var (daily, earlyPct, days) = RentalPlanCatalog.Get(plan);
            var rental = Rental.Create(1, 2, plan);

            var end = rental.StartDate.AddDays(2);

            // Act
            var (total, dailyBasis, fee, isEarly, isLate) = rental.Return(end);

            // Assert
            var daysUsed = 3;
            var plannedDays = days;
            var expectedDaily = daysUsed * daily;

            decimal expectedFee = 0m;
            if (earlyPct is not null)
            {
                var notUsed = plannedDays - daysUsed;
                expectedFee = notUsed * daily * earlyPct.Value;
            }

            Assert.Equal(expectedDaily, dailyBasis);
            Assert.Equal(expectedFee, fee);
            Assert.Equal(expectedDaily + expectedFee, total);

            Assert.True(isEarly);
            Assert.False(isLate);
            Assert.Equal(ERentalStatus.Closed, rental.Status);
            Assert.Equal(end, rental.EndDate);
            Assert.Equal(total, rental.TotalPrice!.Value);
        }

        [Fact]
        public void Return_Late_ShouldAddFixedExtra_50PerLateDay()
        {
            // Arrange
            var plan = ERentalPlan.Days7;
            var (daily, _, _) = RentalPlanCatalog.Get(plan);
            var rental = Rental.Create(1, 2, plan);

            var end = rental.ForecastEndDate.AddDays(2);

            // Act
            var (total, dailyBasis, fee, isEarly, isLate) = rental.Return(end);

            // Assert
            var daysUsed = (end.ToDateTime(TimeOnly.MinValue) - rental.StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            var expectedDaily = daysUsed * daily;
            var expectedFee = 2 * 50m;

            Assert.Equal(expectedDaily, dailyBasis);
            Assert.Equal(expectedFee, fee);
            Assert.Equal(expectedDaily + expectedFee, total);

            Assert.False(isEarly);
            Assert.True(isLate);
            Assert.Equal(ERentalStatus.Closed, rental.Status);
            Assert.Equal(end, rental.EndDate);
            Assert.Equal(total, rental.TotalPrice!.Value);
        }

        [Fact]
        public void Return_ShouldThrow_WhenEndDateBeforeStart()
        {
            var rental = Rental.Create(1, 2, ERentalPlan.Days7);

            var beforeStart = rental.StartDate.AddDays(-1);

            var ex = Assert.Throws<BusinessRulesException>(() =>
                rental.Return(beforeStart));

            Assert.Equal("End date cannot be before start date.", ex.Message);
        }

        [Fact]
        public void Return_ShouldThrow_WhenAlreadyClosed()
        {
            var rental = Rental.Create(1, 2, ERentalPlan.Days7);

            rental.Return(rental.ForecastEndDate);

            var ex = Assert.Throws<BusinessRulesException>(() =>
                rental.Return(rental.ForecastEndDate.AddDays(1)));

            Assert.Equal("Rental already closed.", ex.Message);
        }

        [Fact]
        public void Return_ShouldSetTotalPriceCurrency_EqualsDailyCurrency()
        {
            var rental = Rental.Create(1, 2, ERentalPlan.Days7);
            rental.Return(rental.ForecastEndDate);

            Assert.NotNull(rental.TotalPrice);
            Assert.Equal(rental.DailyPrice.Currency, rental.TotalPrice!.Currency);
        }
    }
}
