using System.Threading.Tasks;

namespace Reminders.FunctionApp
{
    public interface IReminderService
    {
        Task<int> Execute();
    }
}