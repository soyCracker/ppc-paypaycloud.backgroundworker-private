using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paypaycloud.BackgroundWorker.Models;
using Paypaycloud.BackgroundWorker.Queue;
using Paypaycloud.BackgroundWorker.Services;

namespace Paypaycloud.BackgroundWorker.Controllers.api
{
    [Route("api/[controller]")]
    public class CertificateToS3Controller : Controller
    {
        public IBackgroundTaskQueue queue { get; }
        private readonly ILogger logger;
        private AzS3Service azS3Service;

        public CertificateToS3Controller(IBackgroundTaskQueue queue, ILogger<CertificateToS3Controller> logger)
        {
            this.queue = queue;
            this.logger = logger;
            azS3Service = new AzS3Service(logger);
        }     

        [HttpPost]
        public IActionResult Index([FromBody]List<CertificateTwins> twins)
        {
            logger.LogDebug("UTC: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            logger.LogDebug("CertificateToS3Controller req:" + JsonConvert.SerializeObject(twins) + "\n");
            CreateCertificateTask(twins);
            return Json(new { Value = true, ErrorCode = 0 });
        }

        private void CreateCertificateTask(List<CertificateTwins> twins)
        {
            queue.QueueBackgroundWorkItem(async token =>
            {
                await azS3Service.CreateCertificateAsync(twins);
                //azS3Service.TestCount();
            });
        }       
    }
}