namespace HomeAssistant_Status.Services;

public class PasswordGenerator : IPasswordGenerator
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
    
    public string GetNewPassword()
    {
        var pass = string.Empty;

        while (pass.Length < 32)
        {
            pass += AllowedChars[new Random().Next(0, AllowedChars.Length)];
        }

        return pass;
    }
}