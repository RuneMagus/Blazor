using System.Linq;

namespace BlazorApp.Shared.BLL
{
    public class PasswordStrengthValidator : IPasswordStrengthValidator
    {
        public bool IsStrongPassword(string password)
        {
            return HasMinimumLength(password, 8)
                && HasUpperCaseLetter(password)
                && HasLowerCaseLetter(password)
                && (HasDigit(password) || HasSpecialChar(password));
        }

        private bool HasMinimumLength(string password, int minLength)
        {
            return password.Length >= minLength;
        }

        private bool HasDigit(string password)
        {
            return password.Any(c => char.IsDigit(c));
        }

        private bool HasSpecialChar(string password)
        {
            return password.IndexOfAny("!@#$%^&*?_~-£()., ".ToCharArray()) != -1;
        }

        private bool HasUpperCaseLetter(string password)
        {
            return password.Any(c => char.IsUpper(c));
        }

        private bool HasLowerCaseLetter(string password)
        {
            return password.Any(c => char.IsLower(c));
        }
    }
}
