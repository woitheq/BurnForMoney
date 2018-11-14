﻿using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Extensions;
using BurnForMoney.Functions.Shared.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BurnForMoney.Functions.Manual.Functions
{
    public static class AddActivityFunc
    {
        [FunctionName(QueueNames.AddActivity)]
        public static async Task<IActionResult> AddActivityAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "athlete/{athleteId:int}/activities")] HttpRequest req, ExecutionContext executionContext,
            ILogger log,
            int athleteId,
            [Queue(AppQueueNames.AddActivityRequests)] CloudQueue outputQueue)
        {
            log.LogFunctionStart(QueueNames.AddActivity);

            var requestData = await req.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AddActivityRequest>(requestData);
            try
            {
                ValidateRequest(model);
            }
            catch (Exception ex)
            {
                log.LogError(QueueNames.AddActivity, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }

            var pendingActivity = new PendingRawActivity
            {
                SourceActivityId = model.SourceActivityId,
                SourceAthleteId = athleteId,
                ActivityType = model.ActivityCategory,
                StartDate = model.StartDate.Value,
                DistanceInMeters = model.DistanceInMeters,
                MovingTimeInMinutes = model.MovingTimeInMinutes,
                Source = "Manual"
            };

            var output = JsonConvert.SerializeObject(pendingActivity);
            await outputQueue.AddMessageAsync(new CloudQueueMessage(output));
            log.LogFunctionEnd(QueueNames.AddActivity);
            return new OkObjectResult("Request received.");
        }

        private static void ValidateRequest(AddActivityRequest request)
        {
            if (request.StartDate == null)
            {
                throw new ArgumentNullException(nameof(request.StartDate));
            }
            if (string.IsNullOrWhiteSpace(request.ActivityCategory))
            {
                throw new ArgumentNullException(nameof(request.ActivityCategory));
            }
            if (request.MovingTimeInMinutes <= 0)
            {
                throw new ArgumentNullException(nameof(request.MovingTimeInMinutes));
            }
        }
    }

    public class AddActivityRequest
    {
        public long SourceActivityId { get; set; }
        public DateTime? StartDate { get; set; }
        public string ActivityCategory { get; set; }
        public double DistanceInMeters { get; set; }
        public double MovingTimeInMinutes { get; set; }
    }
}