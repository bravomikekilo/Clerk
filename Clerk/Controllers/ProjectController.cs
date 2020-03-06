using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Clerk.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clerk.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private ClerkContext _db;
        private readonly UserManager<ClerkUser> _userManager;

        public ProjectController(ClerkContext context, UserManager<ClerkUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            return _userManager.GetUserId(User);
        }

        /// <summary>
        /// Return Project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var userId = GetUserId();

            var project = await _db.Projects
                .SingleAsync(p => p.OwnerId == userId && p.Id == id);

            if (project == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(project);
        }

        /// <summary>
        /// Update Project comment by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newComment"></param>
        /// <returns></returns>
        [HttpPatch("{id}/comment")]
        public async Task<IActionResult> UpdateProjectComment(int id, [FromBody] NewComment newComment)
        {
            var userId = GetUserId();
            var project = await _db.Projects
                .SingleAsync(p => p.OwnerId == userId && p.Id == id);

            if (project == null)
            {
                return new NotFoundResult();
            }

            project.Comment = newComment.Comment;
            await _db.SaveChangesAsync();
            return new JsonResult(project);
        }

        /// <summary>
        /// Create new Project by name and comment(optional)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewProject(NewProject n)
        {
            var user = await _userManager.GetUserAsync(User);
            Project project = new Project
            {
                Name = n.Name,
                Comment = n.Comment,
                Owner = user,
                OwnerId = user.Id,
            };

            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();
            return new JsonResult(project);
        }

        /// <summary>
        /// Get all project in list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProject()
        {
            var userId = GetUserId();

            var projects = await _db.Projects
                .Where(p => p.OwnerId == userId)
                .ToListAsync();

            // var projects = await _db.Projects.ToListAsync();
            return new JsonResult(projects);
        }

        /// <summary>
        /// Get all running experiments from Project
        /// </summary>
        /// <param name="id">
        /// Project Id
        /// </param>
        /// <returns></returns>
        [HttpGet("{id}/running")]
        public async Task<IActionResult> GetAllRunningExperiment(int id)
        {
            var userId = GetUserId();
            var project = await _db.Projects.FindAsync(id);

            if (project == null || project.OwnerId != userId)
            {
                return NotFound();
            }

            var running = await _db.Experiments
                .Where(e => e.ProjectId == project.Id && !e.Finished)
                .ToListAsync();

            return new JsonResult(running);
        }

        /// <summary>
        /// Get all experiments from Project
        /// </summary>
        /// <param name="id">
        /// Project Id
        /// </param>
        /// <returns></returns>
        [HttpGet("{id}/experiment")]
        public async Task<IActionResult> GetAllExperiment(int id)
        {
            var userId = GetUserId();
            var project = await _db.Projects.FindAsync(id);
            
            if (project == null || project.OwnerId != userId)
            {
                return new NotFoundResult();
            }

            var experiments = await _db.Experiments
                .Where(e => e.ProjectId == project.Id)
                .ToListAsync();

            return new JsonResult(experiments);
        }
    }
}