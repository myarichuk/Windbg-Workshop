using Bogus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Deadlock3
{
    //well, this is not exactly deadlock, this is livelock
    //AND for fun, this simulates Dining Philosophers :)
    class Program
    {
        public class Fork
        {
            public Philosopher Owner { get; set; }

            public event Action OnEating;

            public void SignalEating()
            {
                OnEating?.Invoke();
            }
        }

        public class Philosopher
        {
            private readonly ManualResetEventSlim _cancelEvent;
            public string Name { get; set; }
            public bool IsHungry { get; private set; }

            public Philosopher(string name, ManualResetEventSlim cancelEvent)
            {
                _cancelEvent = cancelEvent;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                IsHungry = true;
            }

            public void EatWith(Fork fork, params Philosopher[] philosophers)
            {
                var random = new Random();
                while (IsHungry && !_cancelEvent.IsSet)
                {
                    //don't have the spoon - patiently wait for spouse
                    if (fork.Owner != this)
                    {
                        Console.WriteLine($"{fork.Owner.Name} is waiting -> doesn't have the fork");
                        Thread.Sleep(1500);
                        continue;
                    }

                    //if someone else is hungry, insist on passing the spoon
                    if (philosophers.Any(p => p.IsHungry))
                    {
                        var hungryPhilosopher = philosophers.OrderBy(x => random.Next()).FirstOrDefault(p => p.IsHungry) ?? throw new InvalidDataException("philosophers collection to eat with should not be empty..");
                        Console.WriteLine($"Someone else is hungry, {fork.Owner.Name} passes the fork to {hungryPhilosopher.Name}");
                        fork.Owner = hungryPhilosopher;
                        Thread.Sleep(1500);
                        continue;
                    }

                    //everyone else are not hungry, finally the owner can eat, then give the spoon to someone else...
                    fork.SignalEating();
                    fork.Owner = philosophers.FirstOrDefault(p => p.IsHungry);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
            var mre = new ManualResetEventSlim();
            Console.Write("Working, press any key to stop...");

            const int philosophersCount = 3; //its enough to have 2!
            var philosophers = new List<Philosopher>();
            for (int i = 0; i < philosophersCount; i++)
                philosophers.Add(new Philosopher(new Faker().Person.FirstName, mre));

            var fork = new Fork { Owner = philosophers[0] };

            fork.OnEating += () => Console.WriteLine($"{fork.Owner.Name} is eating...");

            var threads = new List<Thread>();
            for (int i = 0; i < philosophersCount; i++)
            {
                var philosopher = philosophers[i];
                threads.Add(new Thread(() => philosopher.EatWith(fork, philosophers.Where(p => p != philosopher).ToArray())));
                threads[i].Start();
            }

            Console.ReadKey();
            mre.Set();

            for (int i = 0; i < philosophersCount; i++)
            {
                threads[i].Join(2000);
            }

            Console.WriteLine("done.");
        }
    }
}