using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Reminders
{
    public interface IReminderService
    {
        Task<int> Execute(IConfiguration configuration);
    }
}