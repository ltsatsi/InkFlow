using DomainLayer.Dtos.Comment;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment comment, AppUser? appUser = null)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                BlogId = comment.BlogId,
                AppUserId = comment.AppUserId,

                AppUser = appUser
            };
        }


        public static Comment ToComment(this CommentDto commentDto, AppUser? appUser = null)
        {
            return new Comment
            {
                Content = commentDto.Content,
                BlogId = commentDto.BlogId,
                AppUserId = commentDto.AppUserId,

                AppUser = appUser
            };
        }
    }
}
