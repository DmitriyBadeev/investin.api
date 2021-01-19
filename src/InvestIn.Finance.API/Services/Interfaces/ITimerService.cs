using System.Timers;

namespace InvestIn.Finance.API.Services.Interfaces
{
    public interface ITimerService
    {
        string Subscribe(ElapsedEventHandler handler);
        void Unsubscribe(string handlerId);
        void Reload();
    }
}