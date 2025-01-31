using System.Threading.Tasks;

namespace ImsMonitoring.Services
{
    public interface IImsValidationService
    {
        Task<bool> ValidateProgramCodeFormat(string programCode);
    }

    public class ImsValidationService : IImsValidationService
    {
        public Task<bool> ValidateProgramCodeFormat(string programCode)
        {
            // Based on IMS documentation:
            // "Program code is a 5 character string (must be 5 characters) that is specified within the IMS itself."
            return Task.FromResult(
                !string.IsNullOrWhiteSpace(programCode) && 
                programCode.Length == 5
            );
        }
    }
} 