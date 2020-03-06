using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using Clerk.Log;
using Clerk.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Clerk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ExperimentController : ControllerBase
    {
        private ClerkContext _db;
        private IHubContext<ClerkHub> _hub;
        private ILogManager _logManager;
        private readonly UserManager<ClerkUser> _userManager;
        
        public ExperimentController(
            ClerkContext dbContext, 
            UserManager<ClerkUser> userManager,
            IHubContext<ClerkHub> clerkHub,
            ILogManager logManager)
        {
            _db = dbContext;
            _hub = clerkHub;
            _logManager = logManager;
            _userManager = userManager;
        }


        public string GetUserId()
        {
            return _userManager.GetUserId(User);
        }
        /// <summary>
        /// Get Experiment By Id
        /// </summary>
        /// <param name="id">
        /// Experiment Id
        /// </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperimentById(int id)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments.FindAsync(id);
            if (exper == null || exper.OwnerId != userId)
            {
                return NotFound();
            }

            return new JsonResult(exper);
        }
        
        /// <summary>
        /// Update Experiment Comment By Id
        /// </summary>
        /// <param name="id">
        /// Experiment Id
        /// </param>
        /// <param name="n">
        /// New Comment
        /// </param>
        /// <returns></returns>
        [HttpPatch("{id}/comment")]
        public async Task<IActionResult> UpdateExperimentComment(int id, [FromBody] NewComment n)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments.FindAsync(id);
            
            if (exper == null || exper.OwnerId != userId)
            {
                return NotFound();
            }

            exper.Comment = n.Comment;
            _db.Experiments.Update(exper);
            await _db.SaveChangesAsync();
            return new JsonResult(exper);
        }

        /// <summary>
        /// Set Experiment to finish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/finish")]
        public async Task<IActionResult> FinishExperiment(int id)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments.FindAsync(id);
            if (exper == null || exper.OwnerId != userId)
            {
                return NotFound();
            }

            exper.Finished = true;
            _db.Update(exper);
            await _db.SaveChangesAsync();
            await SendExperimentFinish(id);
            return Ok();
        }

        /// <summary>
        /// Update experiment Progress, refresh last running time
        /// </summary>
        /// <param name="id">
        /// Experiment Id
        /// </param>
        /// <param name="progress">
        /// Experiment Progress
        /// </param>
        /// <returns></returns>
        [HttpPatch("{id}/progress")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] ExperimentProgress progress)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments.FindAsync(id);
            if (exper == null || exper.OwnerId != userId)
            {
                return new NotFoundResult();
            }

            exper.Total = progress.Total;
            exper.Progress = progress.Progress;
            exper.LastProgress = DateTime.Now.ToUniversalTime();
            await _db.SaveChangesAsync();
            await SendProgress(exper.Id, progress);
            return new JsonResult(progress);
        }

        [HttpPost("{id}/log")]
        public async Task<IActionResult> WriteLogToExperiment(int id, [FromBody] string log)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments.FindAsync(id);
            if (exper == null || exper.OwnerId != userId)
            {
                return new NotFoundResult();
            }

            // can't add log to a finished experiment
            if (exper.Finished)
            {
                return new BadRequestResult();
            }

            var logId = await _logManager.WriteLog(id.ToString(), log);
            var msg = new LogMessage
            {
                Id = logId,
                Msg = log
            };
            await SendLogToExperiment(id, log);
            return new JsonResult(msg);
        }

        /// <summary>
        /// Read all log of a experiment
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// LogMessage
        /// Id: serial number
        /// Msg: Log content
        /// </returns>
        [HttpGet("{id}/log")]
        public async Task<IActionResult> ReadLog(int id)
        {
            var userId = GetUserId();
            var exper = await _db.Experiments
                        .Where(e => e.Id == id && e.OwnerId == userId)
                        .FirstAsync();

            if (exper == null)
            {
                return NotFound();
            }
            
            try
            {
                var ret = await _logManager.ReadLog(id.ToString());
                return new JsonResult(ret);
            }
            catch (FileNotFoundException e)
            {
                // when a experiment don't have log
                return new NotFoundResult();
            }
        } 

        /// <summary>
        /// Add new experiment to project
        /// </summary>
        /// <param name="n">
        /// New Experiment Parameter
        /// </param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewExperiment(NewExperiment n)
        {
            var userId = GetUserId();
            var project = await _db.Projects.FindAsync(n.ProjectId);
            if (project == null || project.OwnerId != userId)
            {
                return NotFound();
            }

            var experiment = new Experiment
            {
                Name = n.Name,
                Project = project,
                ProjectId = n.ProjectId,
                ConfigName = n.ConfigName,
                ConfigContent = n.ConfigContent,
                Driver = n.Driver,
                Command = n.Command,
                GitHash = n.GitHash,
                Comment = n.Comment,
                Owner = await _userManager.GetUserAsync(User),
                OwnerId = userId
            };

            await _db.Experiments.AddAsync(experiment);
            await _db.SaveChangesAsync();
            await StartExperiment(n.ProjectId, experiment.Id);
            return new JsonResult(experiment);
        }
        
        /// <summary>
        /// Send progress to all signalR clients watching experiment
        /// </summary>
        /// <param name="experimentId"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        protected async Task SendProgress(int experimentId, ExperimentProgress progress)
        {
            await _hub.Clients.Group($"exper-{experimentId}").SendAsync("UpdateProgress", experimentId, progress);
        }

        /// <summary>
        /// Send experiment start to all signalR clients watching project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="experimentId"></param>
        /// <returns></returns>
        protected async Task StartExperiment(int projectId, int experimentId)
        {
            await _hub.Clients.Group($"project-{projectId}").SendAsync("StartExperiment", experimentId);
        }
        
        protected async Task SendLogToExperiment(int experimentId, string log)
        {
            await _hub.Clients.Group($"exper-{experimentId}").SendAsync("AppendLog", log);
        }

        protected async Task SendExperimentFinish(int experimentId)
        {
            await _hub.Clients.Group($"exper-{experimentId}").SendAsync("ExperimentFinish", experimentId);
        }

    }
}