using DomainLayer.Dtos.Comment;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Dtos.Blog
{
    public class BlogDto
    {
        // Primary Properties
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }



        // Required Properties
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public Category Category { get; set; }
        [Required(ErrorMessage = "Summary is required")]
        public string Summary { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }



        // Optional Properties
        public int ReadTime { get; set; }
        public string? ImageUrl { get; set; }
        public AppUser? Author { get; set; }
        public DateTimeOffset CreatedOn { get; set; } 
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
