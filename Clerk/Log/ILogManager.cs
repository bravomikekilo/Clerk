using System;
using System.Security.Policy;
using System.Threading.Tasks;
using Clerk.Model;

namespace Clerk.Log
{
    public interface ILogManager
    {
        public Task<int> WriteLog(string key, string log);

        public Task ReadLog(string key, Func<Task<string>, Task> cc);
        public Task<T> ReadLog<T>(string key, Func<Task<string>, Task<T>> cc);
        public Task<LogMessage> ReadLog(string key);

        public void OpenLog(string key);
        public Task FinishLog(string key);
    }
}