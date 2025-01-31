using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImsMonitoring.Data;
using ImsMonitoring.Models;
using System.Security.Claims;
using System.Net.Http;
using System.Text;
using ImsMonitoring.Services;

namespace ImsMonitoring.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/imsinstances")]
    public class ImsInstanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImsValidationService _imsValidationService;

        public ImsInstanceController(
            ApplicationDbContext context,
            IImsValidationService imsValidationService)
        {
            _context = context;
            _imsValidationService = imsValidationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImsInstance>>> GetInstances()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var instances = await _context.ImsInstances
                .Where(i => i.UserId == userId)
                .ToListAsync();

            return Ok(instances);
        }

        [HttpPost]
        public async Task<ActionResult<ImsInstance>> CreateInstance(ImsInstance instance)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Validate program code format
            if (!await _imsValidationService.ValidateProgramCodeFormat(instance.ProgramCode))
            {
                return BadRequest(new { message = "Invalid program code format. Must be exactly 5 characters." });
            }

            // Normalize the BaseUrl to ensure it ends with a forward slash
            instance.BaseUrl = instance.BaseUrl.TrimEnd('/') + "/";
            
            instance.UserId = userId;
            instance.CreatedAt = DateTime.UtcNow;

            _context.ImsInstances.Add(instance);
            
            try 
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the error
                return BadRequest(new { message = "Failed to save IMS instance. Please try again." });
            }

            return CreatedAtAction(nameof(GetInstances), new { id = instance.Id }, instance);
        }

        [HttpPost("test-connection")]
        public async Task<ActionResult<object>> TestConnection(ImsConnectionTest request)
        {
            try 
            {
                // Create SOAP client for logon.asmx
                var soapClient = new HttpClient();
                var soapUrl = $"{request.BaseUrl.TrimEnd('/')}/logon.asmx";
                
                // Build SOAP request
                var soapEnvelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                 xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                 xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                      <soap:Body>
                        <Login xmlns=""http://tempuri.org/"">
                          <programCode>{request.ProgramCode}</programCode>
                          <contactType>string</contactType>
                          <email>{request.Email}</email>
                          <password>{request.Password}</password>
                          <projectName>ImsMonitoring</projectName>
                        </Login>
                      </soap:Body>
                    </soap:Envelope>";

                var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", "http://tempuri.org/Login");

                var response = await soapClient.PostAsync(soapUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { message = "Failed to connect to IMS" });
                }

                var responseString = await response.Content.ReadAsStringAsync();
                // Parse XML response to get token
                // TODO: Add proper XML parsing
                
                return Ok(new { message = "Connection successful", token = "sample-token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Connection failed: {ex.Message}" });
            }
        }

        public class ImsConnectionTest
        {
            public string BaseUrl { get; set; } = string.Empty;
            public string ProgramCode { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
} 