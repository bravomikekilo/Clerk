using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Clerk.Model
{
    public class ClerkUser: IdentityUser
    {
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        
        [JsonIgnore]
        public List<Experiment> Experiments { get; set; }
    }
    
    public class UserInfo {
        public string Username { get; set; }
    }
    

    public class NewClerkUser
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Email { get; set; }
    }

    public class ClerkLogin
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}