using DomainLayer.Dtos.Comment;
using DomainLayer.Mappers;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.IService;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICustomService<Comment> _commentService;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(ICustomService<Comment> commentService, UserManager<AppUser> userManager)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }


        [HttpPost]
        public async Task<IActionResult> Create(CommentDto commentDto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Blog", new { id = commentDto.BlogId });


            // Find the currently logged-in user
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Unauthorized();

            commentDto.AppUser = user;
            commentDto.AppUserId = user.Id;
            var comment = commentDto.ToComment();

            comment.IsActive = true;
            comment.CreatedOn = DateTimeOffset.UtcNow;

            await _commentService.CreateAsync(comment);

            return RedirectToAction("Details", "Blog", new { id = commentDto.BlogId });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var comment = await _commentService.GetByIdAsync(id);
            var blogId = comment.BlogId;

            if (currentUser.Id != comment.AppUserId && currentUser.RoleName != "Admin")
                return RedirectToAction("Details", "Blog", new { id = blogId });

            await _commentService.DeleteAsync(comment);
            return RedirectToAction("Details", "Blog", new { id = blogId });
        }
    }
}
