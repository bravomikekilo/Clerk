using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Clerk.Controllers;
using Microsoft.EntityFrameworkCore;


namespace Clerk.Model {

    public class Experiment {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
                
        [JsonIgnore]
        [Required]
        public Project Project { get; set; }
        
        public string OwnerId { get; set; }
        
        [JsonIgnore]
        [Required]
        public ClerkUser Owner { get; set; }  
         
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Host { get; set; }
        
        [Required]
        public string ConfigName { get; set; }
        
        [Required]
        public string ConfigContent { get; set; }
        
        [DefaultValue(false)]
        public bool Finished { get; set; }
        public int Total { get; set; }
        
        [DefaultValue(0)]
        public int Progress { get; set; }
        
        [Required]
        public string Driver { get; set; }
        
        [Required]
        public string Command { get; set; }
        
        public DateTime StartTime { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime? LastProgress { get; set; }
        
        public string GitHash { get; set; }
        public string Comment { get; set; }
    }

    public class NewExperiment
    {
        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public string Host { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string ConfigName { get; set; }
        
        
        [Required]
        public string ConfigContent { get; set; }
        
        [Required]
        public string Driver { get; set; }
        
        [Required]
        public string Command { get; set; }
        public string GitHash { get; set; }
        public string Comment { get; set; }
    }

    public class ExperimentProgress
    {
        [Required]
        public int Total { get; set; }
        
        [Required]
        public int Progress { get; set; }
    }

}