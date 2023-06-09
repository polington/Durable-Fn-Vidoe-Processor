﻿using System.Collections.Generic;
using System.Threading.Tasks;
//using Castle.Core.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace VideoProcessor
{
    public class OrchestratorFunctions
    {
        [FunctionName(nameof(ProcessVideoOrchestrator))]
        public static async Task<object> ProcessVideoOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log);

            var videoLocation = context.GetInput<string>();

            log.LogInformation("about to call transcode video activity");
            var transcodedLocation = await context.CallActivityAsync<string>
                ("TranscodeVideo", videoLocation);

            log.LogInformation("about to call extract thumbnail activity");
            var thumbnailLocation = await context.CallActivityAsync<string>
                ("ExtractThumbnail", transcodedLocation);

            log.LogInformation("about to call prepend intro activity");
            var withIntroLocation = await context.CallActivityAsync<string>
                ("PrependIntro", transcodedLocation);

            return new { 
                TransCoded = transcodedLocation, 
                WithIntro = withIntroLocation, 
                Thumbnail= thumbnailLocation
            };
        }
    }
}