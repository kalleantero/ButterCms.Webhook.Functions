using ButterCms.Webhook.Functions.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace ButterCms.Webhook.Functions.Services
{
    public class RequestValidator : IRequestValidator
    {
        private const string ButterCmsHeaderName = "x-buttercms";
        private string _expectedHeaderValue;
        private readonly ILogger<RequestValidator> _logger;
        public RequestValidator(string expectedHeaderValue, ILogger<RequestValidator> logger)
        {
            _expectedHeaderValue = expectedHeaderValue;
            _logger = logger;
        }
        public bool ValidateHeader(HttpRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Request was invalid.");
                    return false;
                }
                if (!request.Headers.ContainsKey(ButterCmsHeaderName))
                {
                    _logger.LogWarning("Required Butter CMS header was missing.");
                    return false;
                }

                string providedHeaderValue = request.Headers[ButterCmsHeaderName].ToString();

                if (!providedHeaderValue.Equals(_expectedHeaderValue))
                {
                    _logger.LogWarning("Expected Butter CMS header value wasn't provided.");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while header validation.");
                return false;
            }
        }
    }
}
