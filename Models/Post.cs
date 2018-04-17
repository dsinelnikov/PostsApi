using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]        
        public string Title { get; set; }

        [Required]    
        public DateTime PostDate { get; set; }

        [Required]
        [MinLength(10)]
        public string Content { get; set; }
    }
}
