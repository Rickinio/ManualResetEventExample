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
            ManualResetEventSlim _canExcecute = new ManualResetEventSlim(true);
            ConcurrentQueue<Job> _queue = new ConcurrentQueue<Job>();
            BlockingCollection<Job> _jobs = new BlockingCollection<Job>(_queue);

            var job1 = new Job() { ExcecuteJob = () => { Thread.Sleep(3000); Console.WriteLine("Line 1"); _canExcecute.Set(); } };
            var job2 = new Job() { ExcecuteJob = () => { Thread.Sleep(1000); Console.WriteLine("Line 2"); _canExcecute.Set(); } };
            var job3 = new Job() { ExcecuteJob = () => { Thread.Sleep(2000); Console.WriteLine("Line 3"); _canExcecute.Set(); } };
            var job4 = new Job() { ExcecuteJob = () => { Thread.Sleep(1000); Console.WriteLine("Line 4"); _canExcecute.Set(); } };
            var job5 = new Job() { ExcecuteJob = () => { Thread.Sleep(1500); Console.WriteLine("Line 5"); _canExcecute.Set(); } };
            var job6 = new Job() { ExcecuteJob = () => { Thread.Sleep(3000); Console.WriteLine("Line 6"); _canExcecute.Set(); } };
            var job7 = new Job() { ExcecuteJob = () => { Thread.Sleep(2000); Console.WriteLine("Line 7"); _canExcecute.Set(); } };

            _jobs.Add(job1);
            _jobs.Add(job2);
            _jobs.Add(job3);
            _jobs.Add(job4);
            _jobs.Add(job5);
            _jobs.Add(job6);
            _jobs.Add(job7);


            while (!_jobs.IsCompleted)
            {
                Task.Run(() =>
                {
                    Job job = null;
                    _canExcecute.Wait();
                    _canExcecute.Reset();
                    try
                    {
                        job = _jobs.Take();
                    }
                    catch (InvalidOperationException) { }

                    job?.ExcecuteJob();

                });
            }


            Console.ReadKey();
        }
    }

    public class Job
    {
        public Action ExcecuteJob { get; set; }
    }
}
