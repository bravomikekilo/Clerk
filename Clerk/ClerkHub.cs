using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Clerk.Log;
using Clerk.Model;
using Microsoft.AspNetCore.SignalR;

namespace Clerk
{
    public class ClerkHub : Hub
    {
        private ILogManager _logManager;

        public ClerkHub(ILogManager logManager)
        {
            _logManager = logManager;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReviceMessage", user, message);
        }

        public async Task WatchProject(int projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
        }

        public async Task WatchLog(int experimentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"log-{experimentId}");
        }

        public async Task<bool> ReadLog(int experimentId)
        {
            try
            {
                var mainTask = _logManager.ReadLog(experimentId.ToString(), p =>
                {
                    return p.ContinueWith(async readContent =>
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"log-{experimentId}");
                        await Clients.Caller.SendAsync("AppendLog", readContent.Result);
                    }).Unwrap();
                });
                await mainTask;
                return true;
            }
            catch (FileNotFoundException e)
            {
                return false;
            }
        }

        public async Task IgnoreLog(int experimentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"log-{experimentId}");
        }

        public async Task IgnoreProject(int projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
        }

        public async Task WatchExperiment(int experimentId)
        {
            Console.WriteLine("watch experiment {0}", experimentId);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"exper-{experimentId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"exper-{experimentId}");
        }

        public async Task IgnoreExperiment(int experimentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"exper-{experimentId}");
        }
    }
}