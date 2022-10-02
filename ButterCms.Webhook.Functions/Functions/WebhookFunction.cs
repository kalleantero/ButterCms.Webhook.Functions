using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ButterCms.Webhook.Functions.Interfaces;
using ButterCms.Webhook.Functions.Models;

namespace ButterCms.Webhook.Functions.Functions
{
    public class WebhookFunction
    {
        private readonly IDevOpsService _devOpsService;
        private readonly IRequestValidator _requestValidator;

        public WebhookFunction(IDevOpsService devOpsService, IRequestValidator requestValidator)
        {
            _devOpsService = devOpsService;
            _requestValidator = requestValidator;
        }

        [FunctionName("WebhookFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "posts/publish")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Event webhook was triggered.");

            if (!_requestValidator.ValidateHeader(req))
            {
                return new BadRequestObjectResult("Invalid Butter CMS header was provided.");
            }

            if(req.Body == null)
            {
                log.LogInformation("Request body was null.");
                return new BadRequestObjectResult("Invalid request.");
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if(string.IsNullOrEmpty(requestBody))
            {
                log.LogInformation("Request body was null or empty.");
                return new BadRequestObjectResult("Request body was null or empty.");
            }

            var eventData = JsonConvert.DeserializeObject<ButterCmsEvent>(requestBody);

            if (eventData == null)
            {
                log.LogInformation("Request body was invalid and deserialized to ButterCms event.");
                return new BadRequestObjectResult("ButterCMS event data was invalid.");
            }

            var serializedEventData = JsonConvert.SerializeObject(eventData);

            log.LogInformation($"ButterCMS event received: {serializedEventData}");

            if (eventData?.Webhook?.Event == ButterCmsEventNames.PostPublished || eventData?.Webhook?.Event == ButterCmsEventNames.PostDeleted)
            {
                log.LogInformation("Starting DevOps pipeline.");
                await _devOpsService.StartPipeline();
            }

            return new OkObjectResult("Operation completed succefully.");
        }
    }
}
