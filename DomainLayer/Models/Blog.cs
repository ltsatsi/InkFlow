using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public enum Category
    {
        Reading = 1,
        Finance,
        Health,
        Lifestyle,
        Technology,
        Innovation,
    }
        
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public Category Category { get; set; } = Category.Reading;
        public string? ImageUrl { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public int ReadTime { get; set; }


        public Guid AuthorId { get; set; }  
        public AppUser? Author { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
