using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paypaycloud.BackgroundWorker.Queue;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Paypaycloud.BackgroundWorker.Services
{
    public class QueuedHostedService : BackgroundService
    {
        public IBackgroundTaskQueue TaskQueue { get; }
        private readonly ILogger logger;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError("QueuedHostedService ExecuteAsync:" + ex.Message);
                }
            }
        }
    }
}
