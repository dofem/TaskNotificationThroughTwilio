namespace TaskSchedulerWindowService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly INotification _notification;
        private readonly IUtility _utility;

        public Worker(ILogger<Worker> logger,INotification notification,IUtility utility)
        {
            _logger = logger;
            _notification = notification;
            _utility = utility;
        }


        public async Task RunConsole()
        {
            Console.WriteLine("Job Scheduler");
            Console.WriteLine("==============");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("1. Create Job");
                Console.WriteLine("2. List Jobs");
                Console.WriteLine("3. Update Job Status");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        _utility.CreateJob();
                        break;
                    case "2":
                        _utility.ListJobs();
                        break;
                    case "3":
                        Console.Write("Enter the ID of the task to update: ");
                        if (int.TryParse(Console.ReadLine(), out int jobId))
                        {
                            _utility.UpdateJobStatus(jobId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid job ID. Please try again.");
                        }
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _notification.ScheduleDailySummary();
            _logger.LogInformation("Worker Scheduling Daily Summary running at: {time}", DateTimeOffset.Now);
            while (!stoppingToken.IsCancellationRequested)
            {
                 _notification.NotifyTasks();
                _logger.LogInformation("Worker Notify task running at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Adjust the delay time as needed
            }
        }
    }
}