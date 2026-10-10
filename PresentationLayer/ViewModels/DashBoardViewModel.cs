using DomainLayer.Dtos.Blog;
using DomainLayer.Dtos.Comment;

namespace PresentationLayer.ViewModels
{
    public class DashBoardViewModel
    {
        public int BlogCount { get; set; }
        public int CommentCount { get; set; }


        public List<BlogDto> Blogs { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
