using DomainLayer.Mappers;
using DomainLayer.Models;
using DomainLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using ServiceLayer.IService;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly ICustomService<Blog> _blogService;
        private readonly ICustomService<Comment> _commentService;
        private readonly UserManager<AppUser> _userManager;
        public DashBoardController(
            ICustomService<Blog> blogService, 
            UserManager<AppUser> userManager, 
            ICustomService<Comment> commentService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }


        public async Task<IActionResult> Index(QueryObject query)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var blogs = await _blogService.GetAllAsync(query);
            var comments = await _commentService.GetAllAsync(query);

            var userBlogs = blogs.Select(b => b.ToBlogDto()).Where(b => b.AuthorId == currentUser.Id).ToList();
            var userComments = comments.Select(c => c.ToCommentDto()).Where(c => c.AppUserId == currentUser.Id).ToList();


            var dashboardViewModel = new DashBoardViewModel
            {
                BlogCount = userBlogs.Count,
                CommentCount = userComments.Count,

                Blogs = userBlogs,
                Comments = userComments
            };

            return View(dashboardViewModel);
        }
    }
}
