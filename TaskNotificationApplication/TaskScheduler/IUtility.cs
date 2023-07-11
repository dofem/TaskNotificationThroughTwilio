namespace TaskSchedulerWindowService
{
    public interface IUtility
    {
        void CreateJob();
        void ListJobs();
        List<Job> LoadJobsFromFile();
        void SaveJobToFile(Job job);
        void UpdateJobStatus(int jobId);
    }
}