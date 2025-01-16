namespace ImsMonitoring.Interfaces;

public interface IImsAuthenticationService
{
    Task<string> GetAuthTokenAsync();
    Task ValidateTokenAsync();
    Task<bool> IsTokenValidAsync(string token);
}