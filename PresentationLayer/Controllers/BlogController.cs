using DomainLayer.Dtos.Blog;
using DomainLayer.Mappers;
using DomainLayer.Models;
using DomainLayer.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.IService;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static NuGet.Packaging.PackagingConstants;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly ICustomService<Blog> _blogService;
        private readonly IImageService _imageService;
        private readonly UserManager<AppUser> _userManager;
        public BlogController(
            ICustomService<Blog> blogService, 
            IImageService imageService,
            UserManager<AppUser> userManager)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }



        public async Task<IActionResult> Index([FromQuery] QueryObject query)       
        {
            var blogs = await _blogService.GetAllAsync(query);
            var blogsDto = blogs.Select(b => b.ToBlogDto()).OrderByDescending(b => b.CreatedOn).ToList();

            ViewData["CurrentSearch"] = query.Search;

            return View(blogsDto);
        }



        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            var author = await _userManager.FindByIdAsync(blog.AuthorId.ToString());


            var path = blog.ImageUrl;
            blog.ImageUrl = await _imageService.GetPublicUrlAsync(path!);

            return View(blog.ToBlogDto(author: author));
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Unauthorized();

            var blogDto = new BlogDto
            {
                AuthorId = user.Id,
            };

            return View(blogDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlogDto blogDto, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(blogDto);

            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select an image.");
                return View(blogDto);
            }

            blogDto.ReadTime = EstimatedReadTime(blogDto.Content);
            blogDto.ImageUrl = await _imageService.UploadAsync(imageFile, "posts");

            var blog = blogDto.ToBlog();

            blog.IsActive = true;
            blog.CreatedOn = DateTimeOffset.UtcNow;

            await _blogService.CreateAsync(blog);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var blog = await _blogService.GetByIdAsync(id);

            if (currentUser.Id != blog.AuthorId)
                return RedirectToAction(nameof(Index));


            return View(blog.ToBlogDto());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BlogDto blogDto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                var existingBlog = await _blogService.GetByIdAsync(blogDto.Id);
                blogDto.ImageUrl = existingBlog.ImageUrl;
                return View(blogDto);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var blog = await _blogService.GetByIdAsync(blogDto.Id);

            if (currentUser.Id != blogDto.AuthorId)
                return RedirectToAction(nameof(Index));

            blog.Title = blogDto.Title;
            blog.Category = blogDto.Category;


            if (imageFile != null && imageFile.Length > 0)
            {
                var oldImageUrl = blog.ImageUrl;

                var newImageUrl = await _imageService.UploadAsync(
                    imageFile,
                    "images/uploads");

                blog.ImageUrl = newImageUrl;

                await _imageService.DeleteAsync(oldImageUrl);
            }


            blog.Summary = blogDto.Summary;
            blog.Content = blogDto.Content;
            blog.ModifiedOn = DateTimeOffset.Now;
            blog.ReadTime = EstimatedReadTime(blogDto.Content);

            await _blogService.UpdateAsync(blog);

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            var author = await _userManager.FindByIdAsync(blog.AuthorId.ToString());

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            if (currentUser.Id != blog.AuthorId && currentUser.RoleName != "Admin")
                return RedirectToAction(nameof(Index));

            return View(blog.ToBlogDto(author: author));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(BlogDto blogDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized();

            var blog = await _blogService.GetByIdAsync(blogDto.Id);

            if (currentUser.Id != blog.AuthorId && currentUser.RoleName != "Admin")
                return RedirectToAction(nameof(Index));

            var path = blog.ImageUrl;

            await _blogService.DeleteAsync(blog);

            if (!string.IsNullOrEmpty(path))
            {
                await _imageService.DeleteAsync(path);
            }

            return RedirectToAction(nameof(Index));
        }




        // Helper method
        private int EstimatedReadTime(string content)
        {
            int wordsPerMinute = 200;
            string plainText = Regex.Replace(content, "<.*?>", "");
            int wordCount = plainText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
        }
    }
}
