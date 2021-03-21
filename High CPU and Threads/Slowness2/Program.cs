using Bogus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Slowness2
{
    public class User
    {
        public string Id;

        public string Name;

        public string Address;
    }

    //note: this is rather silly implementation of cache
    //the idea here is to allow seeing the effect of lock contention in WinDbg
    public class MegaUserCache
    {
        private readonly object _sync = new object();
        private readonly Dictionary<string, User> _cacheData = new Dictionary<string, User>();

        public bool TryGet(string id, out User user)
        {
            lock (_sync)
            {
                Thread.Sleep(150); //simulate roundtrip to distributed cache
                return _cacheData.TryGetValue(id, out user);
            }
        }

        public bool TryAdd(string id, User user)
        {
            lock(_sync)
            {
                Thread.Sleep(150); //simulate roundtrip to distributed cache
                var success = _cacheData.TryAdd(id, user);
                if(success)
                {
                    Task.Delay(500)
                        .ContinueWith(__ =>
                        {
                            lock(_sync)
                                _cacheData.Remove(id, out _);
                            return Task.CompletedTask;
                        });
                }
                return success;
            }
        }
    }

    public class UserRepository
    {
        private readonly MegaUserCache _userCache = new MegaUserCache();
        private readonly List<User> _data = new List<User>(); //simulation of a database
        private readonly List<string> _ids = new List<string>();

        public IReadOnlyList<string> Ids => _ids;

        public UserRepository()
        {
            var f = new Faker();

            //intentionally low amount of users to generate contention on the locks in cache
            for(int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid().ToString();
                _ids.Add(id);
                _data.Add(new User
                {
                    Id = id,
                    Name = f.Name.FullName(),
                    Address = $"{f.Address.StreetName()} {f.Address.BuildingNumber()}, {f.Address.City()}"
                });
            }
        }

        public User Get(string id)
        {
            if (_userCache.TryGet(id, out var user))
                return user;

            Thread.Sleep(2000); //simulate roundtrip to the database
            foreach(var fetchedUser in _data)
            {
                if(fetchedUser.Id.Equals(id, StringComparison.InvariantCulture))
                {
                    _userCache.TryAdd(id, fetchedUser);
                    return fetchedUser;
                }
            }

            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Process.GetCurrentProcess().Id);
            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            using var mre = new ManualResetEventSlim();

            var userRepository = new UserRepository();

            var server = new Thread(() =>
            {
                var r = new Random();
                User GetRandomUser() => userRepository.Get(userRepository.Ids[r.Next(0, userRepository.Ids.Count - 1)]);
                while (!mre.IsSet)
                {
                    Task.Run(() =>
                    {
                        _ = GetRandomUser();
                    });
                }
            })
            {
                Name = "Server Activity Simulation Thread"
            };

            server.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            mre.Set();
            server.Join();
        }
    }
}
