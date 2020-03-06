using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Clerk.Model {

    public class Project {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string OwnerId { get; set; }
        
        [JsonIgnore]
        [Required]
        public ClerkUser Owner { get; set; }
        
        public string Comment { get; set; }
        public DateTime CreateTime { get; set; }
        
        [JsonIgnore]
        public List<Experiment> Experiments { get; set; }
        
    }

    public class NewProject
    {
        [Required]
        public string Name { get; set; }
        
        public string Comment { get; set; }
    }
}