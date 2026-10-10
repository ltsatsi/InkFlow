using DomainLayer.Dtos.Blog;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Mappers
{
    public static class BlogMapper
    {
        public static BlogDto ToBlogDto(this Blog blog, AppUser? author = null)
        {
            return new BlogDto
            {
                Id = blog.Id,
                AuthorId = blog.AuthorId,
                Author = author,

                Title = blog.Title,
                Category = blog.Category,
                ImageUrl = blog.ImageUrl,
                Summary = blog.Summary,
                Content = blog.Content,
                ReadTime = blog.ReadTime,
                CreatedOn = blog.CreatedOn,

                Comments = blog.Comments.Select(c => c.ToCommentDto(appUser: c.AppUser)).ToList()
            };
        }


        public static Blog ToBlog(this BlogDto blogDto)
        {
            return new Blog
            {
                Title = blogDto.Title,
                Category = blogDto.Category,
                ImageUrl = blogDto.ImageUrl,
                Summary = blogDto.Summary,
                Content = blogDto.Content,
                ReadTime = blogDto.ReadTime,
                AuthorId = blogDto.AuthorId,

                Comments = blogDto.Comments.Select(c => c.ToComment(c.AppUser)).ToList()
            };
        }
    }
}
