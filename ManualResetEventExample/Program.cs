using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ManualResetEventExample
{
    class Program
    {
        static void Main(string[] args)
        {
            ManualResetEventSlim _canExecute = new ManualResetEventSlim(true);
            ConcurrentQueue<Job> _queue = new ConcurrentQueue<Job>();
            BlockingCollection<Job> _jobs = new BlockingCollection<Job>(_queue);

            var job1 = new Job() { ExecuteJob = () => { Thread.Sleep(3000); Console.WriteLine("Line 1"); _canExecute.Set(); } };
            var job2 = new Job() { ExecuteJob = () => { Thread.Sleep(1000); Console.WriteLine("Line 2"); _canExecute.Set(); } };
            var job3 = new Job() { ExecuteJob = () => { Thread.Sleep(2000); Console.WriteLine("Line 3"); _canExecute.Set(); } };
            var job4 = new Job() { ExecuteJob = () => { Thread.Sleep(1000); Console.WriteLine("Line 4"); _canExecute.Set(); } };
            var job5 = new Job() { ExecuteJob = () => { Thread.Sleep(1500); Console.WriteLine("Line 5"); _canExecute.Set(); } };
            var job6 = new Job() { ExecuteJob = () => { Thread.Sleep(3000); Console.WriteLine("Line 6"); _canExecute.Set(); } };
            var job7 = new Job() { ExecuteJob = () => { Thread.Sleep(2000); Console.WriteLine("Line 7"); _canExecute.Set(); } };

            _jobs.Add(job1);
            _jobs.Add(job2);
            _jobs.Add(job3);
            _jobs.Add(job4);
            _jobs.Add(job5);
            _jobs.Add(job6);
            _jobs.Add(job7);

            Task.Run(() =>
            {
                while (!_jobs.IsCompleted)
                {
                    Task.Run(() =>
                    {
                        Job job = null;
                        _canExecute.Wait();
                        _canExecute.Reset();
                        try
                        {
                            job = _jobs.Take();
                        }
                        catch (InvalidOperationException) { }

                        job?.ExecuteJob();
                    });
                }
            });


            Console.ReadKey();
        }
    }

    public class Job
    {
        public Action ExecuteJob { get; set; }
    }
}
