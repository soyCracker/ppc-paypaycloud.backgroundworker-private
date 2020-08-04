using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Paypaycloud.BackgroundWorker.Base;

namespace Paypaycloud.BackgroundWorker.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : Controller
    {
        private readonly ILogger logger;

        public InfoController(ILogger<InfoController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Index()
        {
            logger.LogDebug("Version: " + Config.Ver + "\n");
            logger.LogDebug("IsReleaseMode: " + Config.IsReleaseMode + "\n");
            logger.LogDebug("UTC: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            return Json(new { Value = true, ErrorCode = 0, Version = Config.Ver,
                IsReleaseMode = Config.IsReleaseMode, UTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}