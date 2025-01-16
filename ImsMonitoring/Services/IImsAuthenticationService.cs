namespace ImsMonitoring.Services;

public interface IImsAuthenticationService
{
    Task<bool> ValidateCredentialsAsync(string username, string password);
    Task<string> GetAuthTokenAsync();
} 