using System.Threading.Tasks;
using Clerk.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clerk.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ClerkUser> _userManager;
        private readonly SignInManager<ClerkUser> _signInManager;

        public UserController(
            UserManager<ClerkUser> userManager,
            SignInManager<ClerkUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListUser()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(ClerkLogin login)
        {
            var result = await _signInManager.PasswordSignInAsync(
                login.Username, login.Password,
                true, true
            );

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(NewClerkUser n)
        {
            var user = new ClerkUser
            {
                UserName = n.Username,
                Email = n.Email,
            };
            var result = await _userManager.CreateAsync(
                user, n.Password
            );

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!_signInManager.IsSignedIn(User)) return BadRequest();
            
            var user = await _userManager.GetUserAsync(User);
            await _signInManager.RefreshSignInAsync(user);
            var userInfo = new UserInfo
            {
                Username = user.UserName
            };
            return Ok(userInfo);
        }
    }
}