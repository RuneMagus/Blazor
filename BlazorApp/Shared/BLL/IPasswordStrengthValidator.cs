namespace BlazorApp.Shared.BLL
{
    public interface IPasswordStrengthValidator
    {
        bool IsStrongPassword(string password);
    }
}