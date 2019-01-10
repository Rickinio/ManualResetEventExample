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
            // Set Manual Reset Event to true cause i want to immediately 
            // start executing the jobs, i don't want the tread to wait
            ManualResetEventSlim _canExecute = new ManualResetEventSlim(true);
            ConcurrentQueue<Job> _queue = new ConcurrentQueue<Job>();
            BlockingCollection<Job> _jobs = new BlockingCollection<Job>(_queue);

            var job1 = new Job(1);
            var job2 = new Job(2);
            var job3 = new Job(3);
            var job4 = new Job(4);
            var job5 = new Job(5);
            var job6 = new Job(6);
            var job7 = new Job(7);

            job1.OnJobFinished += (e, s) => _canExecute.Set();
            job2.OnJobFinished += (e, s) => _canExecute.Set();
            job3.OnJobFinished += (e, s) => _canExecute.Set();
            job4.OnJobFinished += (e, s) => _canExecute.Set();
            job5.OnJobFinished += (e, s) => _canExecute.Set();
            job6.OnJobFinished += (e, s) => _canExecute.Set();
            job7.OnJobFinished += (e, s) => _canExecute.Set();

            _jobs.Add(job1);
            _jobs.Add(job2);
            _jobs.Add(job3);
            _jobs.Add(job4);
            _jobs.Add(job5);
            _jobs.Add(job6);
            _jobs.Add(job7);


            var thread = new Thread(() =>
            {
                while (!_jobs.IsCompleted)
                {
                    var innerThread = new Thread(() =>
                    {
                        Job job = null;
                        // Now i wait for the ManualResetEvent to be set to True
                        _canExecute.Wait();
                        //and immediatly Reset it so that the Thread will pause
                        // on the above line and again wait for the ManualResetEvent
                        // to be set to true
                        _canExecute.Reset();
                        try
                        {
                            job = _jobs.Take();
                        }
                        catch (InvalidOperationException) { }

                        job?.Execute();
                    })
                    {
                        // This is important as it allows the process to exit
                        // while this thread is running
                        IsBackground = true
                    };
                    innerThread.Start();
                }
            })
            {
                // This is important as it allows the process to exit
                // while this thread is running
                IsBackground = true
            };
            thread.Start();
           
            Console.ReadKey();
        }
    }



    public class Job
    {
        public delegate void JobFinishedHandler(object myObject, JobFinishedArgs myArgs);
        public event JobFinishedHandler OnJobFinished;
        public int JobId { get; set; }
        public Job(int jobId)
        {
            this.JobId = jobId;
        }

        public void Execute()
        {
            Console.WriteLine($"Executing Job {this.JobId}");
            Thread.Sleep(new Random().Next(1000, 3000));
            OnJobFinished(this, new JobFinishedArgs("Job Finished!"));
        }
    }

    public class JobFinishedArgs : EventArgs
    {
        private readonly string _message;
        public string Message { get { return _message; } }

        public JobFinishedArgs(string message)
        {
            this._message = message;
        }
    }
}
