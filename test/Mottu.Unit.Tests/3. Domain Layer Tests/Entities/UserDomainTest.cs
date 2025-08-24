using System;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate;
using Mottu.Domain.UserAggregate.Enums;
using Xunit;

namespace Mottu.Unit.Tests.Domain.UserAggregate
{
    public class UserTests
    {
        [Fact]
        public void ParameterlessCtor_ShouldCreate_EmptyUser()
        {
            var u = new User();

            Assert.Null(u.Name);
            Assert.Equal(default, u.BirthDate);
            Assert.Equal(default, u.CreatedAtUtc);
            Assert.Null(u.UpdatedAtUtc);
            Assert.Null(u.Cnpj);
            Assert.Null(u.CnhNumber);
            Assert.Null(u.CnhType);
            Assert.Null(u.CnhImageUri);
            Assert.Equal(default(EUserRole), u.Role);
        }

        [Fact]
        public void CreateCourier_ShouldPopulate_AllFields_AndSetRoleCourier()
        {
            // Arrange
            var before = DateTime.UtcNow;
            var name = "John Doe";
            var birth = new DateOnly(1997, 5, 12);
            var cnpj = "13133807000144"; 
            var cnhNumber = "12345678900";  
            var cnhType = ECNH.A;
            var img = "https://cdn.example.com/cnh/123.png";

            // Act
            var user = User.CreateCourier(name, birth, cnpj, cnhNumber, cnhType, img);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(user.Name);
            Assert.Equal(EUserRole.Courier, user.Role);
            Assert.Equal(birth, user.BirthDate);
            Assert.NotNull(user.Cnpj);
            Assert.NotNull(user.CnhNumber);
            Assert.Equal(cnhType, user.CnhType);
            Assert.Equal(img, user.CnhImageUri);

            Assert.True(user.CreatedAtUtc >= before && user.CreatedAtUtc <= after);
            Assert.Null(user.UpdatedAtUtc);
        }

        [Fact]
        public void CreateCourier_ShouldAllow_NullImage()
        {
            var user = User.CreateCourier(
                "Jane Roe",
                new DateOnly(1990, 1, 1),
                "13133807000144",
                "12345678900",
                ECNH.B,
                cnhImageUri: null
            );

            Assert.Null(user.CnhImageUri);
            Assert.Equal(EUserRole.Courier, user.Role);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateCourier_ShouldThrow_WhenNameInvalid(string invalidName)
        {
            Assert.Throws<BusinessRulesException>(() =>
                User.CreateCourier(
                    invalidName!,
                    new DateOnly(1990, 1, 1),
                    "12.345.678/0001-90",
                    "12345678900",
                    ECNH.B));
        }

        [Theory]
        [InlineData("123")]               
        [InlineData("00.000.000/0000-00")] 
        [InlineData("   ")]       
        public void CreateCourier_ShouldThrow_WhenCnpjInvalid(string invalidCnpj)
        {
            Assert.Throws<BusinessRulesException>(() =>
                User.CreateCourier(
                    "Valid Name",
                    new DateOnly(1990, 1, 1),
                    invalidCnpj,
                    "12345678900",
                    ECNH.B));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABCDEF")]
        public void CreateCourier_ShouldThrow_WhenCnhNumberInvalid(string invalidCnh)
        {
            Assert.Throws<BusinessRulesException>(() =>
                User.CreateCourier(
                    "Valid Name",
                    new DateOnly(1990, 1, 1),
                    "12.345.678/0001-90",
                    invalidCnh!,
                    ECNH.B));
        }

        [Fact]
        public void CreateCourier_ShouldSet_CnhType_AndCnhNumberNotNull()
        {
            var user = User.CreateCourier(
                "Valid Name",
                new DateOnly(1990, 1, 1),
                "13133807000144",
                "12345678900",
                ECNH.AB);

            Assert.Equal(ECNH.AB, user.CnhType);
            Assert.NotNull(user.CnhNumber);
        }
    }
}
