using System.Threading.Tasks;

namespace ButterCms.Webhook.Functions.Interfaces
{
    public interface IDevOpsService
    {
        Task<bool> StartPipeline();
    }
}
