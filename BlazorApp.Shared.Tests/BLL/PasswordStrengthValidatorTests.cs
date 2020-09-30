using BlazorApp.Shared.BLL;
using Xunit;

namespace BlazorApp.Shared.Tests
{
    public class PasswordStrengthValidatorTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("p@ssword", false)]
        [InlineData("password", false)]
        [InlineData("Password", false)]
        [InlineData("Passw0rd", true)]
        [InlineData("P@ssword", true)]
        [InlineData("P@sw0rd", false)]
        public void IsStrongPasswordTest(string password, bool expected)
        {
            var passwordStrengthValidator = new PasswordStrengthValidator();
            var actual = passwordStrengthValidator.IsStrongPassword(password);
            Assert.Equal(expected, actual);
        }
    }
}
