using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TaskSchedulerWindowService
{
    public class Notification : INotification
    {

        private const string JobsFilePath = "jobs.txt";
        private const int NotifyDaysBeforeDue = 3;
        private const int DailySummaryHour = 9; // Send the daily summary at 9 AM
        private readonly TwilioSettings _settings;
        private readonly IUtility _utility;

        public Notification(IOptions<TwilioSettings> settings, IUtility utility)
        {
            _settings = settings.Value;
            _utility = utility;
        }



        public async Task ScheduleDailySummary()
        {
            TimeSpan targetTime = new TimeSpan(DailySummaryHour, 0, 0);
            TimeSpan timeUntilTarget = targetTime - DateTime.Now.TimeOfDay;

            if (timeUntilTarget < TimeSpan.Zero)
            {
                timeUntilTarget = TimeSpan.FromDays(1) + timeUntilTarget;
            }

            await Task.Delay(timeUntilTarget);
            SendDailyTaskSummary();
        }


        public void NotifyTasks()
        {
            List<Job> jobs = _utility.LoadJobsFromFile();

            foreach (Job job in jobs)
            {
                if (!job.isComplete && job.DueDate.Date == DateTime.Today.AddDays(NotifyDaysBeforeDue).Date)
                {
                    SendNotification($"Your task '{job.Name}' is due in 3 days!");
                }
                else if (!job.isComplete && job.DueDate.Date == DateTime.Today.Date)
                {
                    SendNotification($"Your task '{job.Name}' is due today!");
                }
            }
        }

        public void SendNotification(string message)
        {
            TwilioClient.Init(_settings.TwilioAccountSid, _settings.TwilioAuthToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(_settings.TwilioRecepientNumber))
            {
                From = new PhoneNumber(_settings.TwilioPhoneNumber),
                Body = message
            };

            MessageResource.Create(messageOptions);
        }


        public void SendDailyTaskSummary()
        {
            List<Job> jobs = _utility.LoadJobsFromFile();

            if (jobs.Count == 0)
            {
                SendNotification("No jobs found for today.");
                return;
            }

            string summary = "Daily job Summary:\n";

            foreach (Job job in jobs)
            {
                summary += $"- Task: {job.Name}\n";
                summary += $"  Due Date: {job.DueDate.ToShortDateString()}\n";
                summary += "\n";
            }

            SendNotification(summary);
        }


    }
}