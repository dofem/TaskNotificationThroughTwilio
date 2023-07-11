using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSchedulerWindowService
{
    public class Utility : IUtility
    {

        private const string JobsFilePath = "jobs.txt";

        public void CreateJob()
        {
            Console.WriteLine();
            Console.WriteLine("Create New Job");
            Console.WriteLine("-----------");

            Console.Write("Enter job name: ");
            string name = Console.ReadLine();

            Console.Write("Enter job due date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                Console.WriteLine("Invalid date format. Job creation canceled.");
                return;
            }
            // Generate a new unique ID for the task
            int id = GenerateUniqueId();

            Job job = new Job
            {
                Id = id,
                Name = name,
                DueDate = dueDate,
                isComplete = false
            };

            SaveJobToFile(job);

            Console.WriteLine("Task created successfully!");
        }




        public void ListJobs()
        {
            Console.WriteLine();
            Console.WriteLine("List Jobs");
            Console.WriteLine("----------");

            List<Job> jobs = LoadJobsFromFile();

            if (jobs.Count == 0)
            {
                Console.WriteLine("No jobs found.");
            }
            else
            {
                Console.WriteLine("ID\tName\tDue Date\tCompleted");
                Console.WriteLine("---------------------------------");

                foreach (Job job in jobs)
                {
                    Console.WriteLine($"{job.Id}\t{job.Name}\t{job.DueDate.ToShortDateString()}\t{job.isComplete}");
                }
            }
        }



        public void SaveJobToFile(Job job) //single task
        {
            using (StreamWriter writer = File.AppendText(JobsFilePath))
            {
                writer.WriteLine($"{job.Id},{job.Name},{job.DueDate},{job.isComplete}");
            }
        }



        public List<Job> LoadJobsFromFile()
        {
            List<Job> jobs = new List<Job>();

            if (File.Exists(JobsFilePath))
            {
                string[] lines = File.ReadAllLines(JobsFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length == 4 && int.TryParse(parts[0], out int id) && DateTime.TryParse(parts[2], out DateTime dueDate) && bool.TryParse(parts[3], out bool isCompleted))
                    {
                        Job job = new Job
                        {
                            Id = id,
                            Name = parts[1],
                            DueDate = dueDate,
                            isComplete = isCompleted
                        };

                        jobs.Add(job);
                    }
                }
            }

            return jobs;
        }


        private void SaveJobsToFile(List<Job> jobs) // save list of tasks
        {
            using (StreamWriter writer = new StreamWriter(JobsFilePath))
            {
                foreach (Job job in jobs)
                {
                    string line = $"{job.Id},{job.Name},{job.DueDate},{job.isComplete}";
                    writer.WriteLine(line);
                }
            }
        }


        public void UpdateJobStatus(int jobId)
        {
            List<Job> jobs = LoadJobsFromFile();
            Job jobToUpdate = jobs.FirstOrDefault(job => job.Id == jobId);

            if (jobToUpdate != null)
            {
                jobToUpdate.isComplete = true;

                if (jobToUpdate.isComplete)
                {
                    jobs.Remove(jobToUpdate);
                    Console.WriteLine("Job deleted.");
                }

                SaveJobsToFile(jobs);
                Console.WriteLine("Jobs status updated successfully.");
            }
            else
            {
                Console.WriteLine("Job not found.");
            }
        }


        private  int GenerateUniqueId()
        {
            List<Job> jobs = LoadJobsFromFile();

            int id = 1;

            foreach (Job job in jobs)
            {
                if (job.Id >= id)
                {
                    id = job.Id + 1;
                }
            }

            return id;
        }

    }
};