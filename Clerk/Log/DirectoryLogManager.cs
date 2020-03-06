using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clerk.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Clerk.Log
{
    public class DirectoryLogManager : ILogManager
    {
        private ConcurrentDictionary<string, SingleLogManager> logs;

        private IHostApplicationLifetime _lifetime;

        private DirectoryInfo baseDir;

        private void OnStop()
        {
            foreach (var pair in logs)
            {
                Console.WriteLine("stopping DirectorLogManager");
                pair.Value.Dispose();
            }
        }

        public DirectoryLogManager(
            IConfiguration config,
            IServiceScopeFactory scopeFactory,
            IHostApplicationLifetime lifetime
        )
        {
            _lifetime = lifetime;
            _lifetime.ApplicationStopped.Register(OnStop);
            logs = new ConcurrentDictionary<string, SingleLogManager>();
            var configSection = config.GetSection("LogManager");
            var baseDirPath = configSection.GetValue<string>("baseDir");
            baseDir = new DirectoryInfo(baseDirPath);

            if (!baseDir.Exists)
            {
                baseDir.Create();
            }

            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetService<ClerkContext>();
            var unfinished = db.Experiments.Where(e => !e.Finished).ToList();
            unfinished.ForEach(e => TryOpenLog(e.Id.ToString()));
        }

        private SingleLogManager FindManagerByKey(string key)
        {
            SingleLogManager ret;
            var result = logs.TryGetValue(key, out ret);
            if (!result)
            {
                throw new KeyNotFoundException(key);
            }

            return ret;
        }

        public Task<int> WriteLog(string key, string log)
        {
            var result = logs.TryGetValue(key, out var manager);
            if (result) return manager.WriteLog(log);

            manager = OpenLogInternal(key);
            return manager.WriteLog(log);
        }

        public Task<T> ReadLog<T>(string key, Func<Task<string>, Task<T>> cc)
        {
            SingleLogManager ret;
            var result = logs.TryGetValue(key, out ret);
            if (result) return ret.ReadLog(cc);

            // if experiment is not watched by now, probably finished or not exist
            var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var log = File.ReadAllTextAsync(filePath);
            return log.ContinueWith(p => cc(p)).Unwrap();
        }

        public Task ReadLog(string key, Func<Task<string>, Task> cc)
        {
            var result = logs.TryGetValue(key, out var ret);
            if (result) return ret.ReadLog(cc);

            // if experiment is not watched by now, probably finished or not exist
            var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var log = File.ReadAllTextAsync(filePath);
            return log.ContinueWith(p => cc(p)).Unwrap();
        }

        public Task<LogMessage> ReadLog(string key)
        {
            try
            {
                var logManager = FindManagerByKey(key);
                return logManager.ReadLog();
            }
            catch (KeyNotFoundException)
            {
                var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath);
                }

                var log = File.ReadAllTextAsync(filePath);
                return log.ContinueWith(msg =>
                {
                    var result = msg.Result;
                    return new LogMessage
                    {
                        Id = 0,
                        Msg = result,
                    };
                });
            }
        }

        public void TryOpenLog(string key)
        {
            var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
            if (!File.Exists(filePath)) return;

            var fileInfo = new FileInfo(filePath);
            var logManager = new SingleLogManager(fileInfo);
            logs.TryAdd(key, logManager);
        }

        private SingleLogManager OpenLogInternal(string key)
        {
            var ret = logs.GetOrAdd(key, key =>
            {
                var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
                var fileInfo = new FileInfo(filePath);
                return new SingleLogManager(fileInfo);
            });
            return ret;
        }

        public void OpenLog(string key)
        {
            // throw new System.NotImplementedException();
            var ret = logs.GetOrAdd(key, key =>
            {
                var filePath = Path.Combine(baseDir.FullName, $"{key}.log");
                var fileInfo = new FileInfo(filePath);
                return new SingleLogManager(fileInfo);
            });
        }

        public async Task FinishLog(string key)
        {
            var result = logs.TryRemove(key, out var logManager);
            if (result)
            {
                await logManager.FinishTask();
            }
        }

        public void StopApplication()
        {
            Console.WriteLine("dispose log manager");
            foreach (var pair in logs)
            {
                pair.Value.Dispose();
            }
        }

        public CancellationToken ApplicationStarted { get; }
        public CancellationToken ApplicationStopped { get; }
        public CancellationToken ApplicationStopping { get; }
    }
}