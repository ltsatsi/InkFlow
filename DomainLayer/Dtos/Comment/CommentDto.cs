using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Dtos.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }    
        // Blog to which the comment belongs
        public Guid BlogId { get; set; }



        // Required Properties
        public string Content { get; set; }



        // User who created the comment
        public Guid AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
