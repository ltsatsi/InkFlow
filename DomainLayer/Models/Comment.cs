using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }


        // blog to which the comment belongs
        public Guid BlogId { get; set; }
        public Blog? Blog { get; set; }


        // user who created the comment
        public Guid AppUserId { get; set; }
        public AppUser? AppUser { get; set; }    
    }
}
