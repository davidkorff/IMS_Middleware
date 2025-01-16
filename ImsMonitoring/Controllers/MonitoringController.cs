using Microsoft.AspNetCore.Mvc;
using ImsMonitoring.Models;
using ImsMonitoring.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ImsMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringController : ControllerBase
    {
        private readonly ILogger<MonitoringController> _logger;
        private readonly SubmissionMonitoringService _monitoringService;

        public MonitoringController(
            SubmissionMonitoringService monitoringService,
            ILogger<MonitoringController> logger)
        {
            _monitoringService = monitoringService;
            _logger = logger;
        }

        [HttpGet("status")]
        public ActionResult<MonitoringStatus> GetStatus()
        {
            var status = new MonitoringStatus
            {
                IsRunning = true,
                LastCheck = DateTime.UtcNow,
                SubmissionsProcessed = _monitoringService.ProcessedCount,
                CurrentStatus = "Active",
                RecentErrors = new List<string>()
            };

            return Ok(status);
        }
    }
} 