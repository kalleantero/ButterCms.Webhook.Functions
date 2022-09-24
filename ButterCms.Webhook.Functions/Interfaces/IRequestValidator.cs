using Microsoft.AspNetCore.Http;

namespace ButterCms.Webhook.Functions.Interfaces
{
    public interface IRequestValidator
    {
        bool ValidateHeader(HttpRequest request);
    }
}
