namespace TaskSchedulerWindowService
{
    public interface INotification
    {
        void NotifyTasks();
        Task ScheduleDailySummary();
        void SendDailyTaskSummary();
        void SendNotification(string message);
    }
}