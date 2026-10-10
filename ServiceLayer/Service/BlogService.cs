using DomainLayer.Models;
using DomainLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepository;
using ServiceLayer.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service
{
    public class BlogService : ICustomService<Blog>
    {
        private readonly IRepository<Blog> _blogRepo;
        public BlogService(IRepository<Blog> blogRepo)
        {
            _blogRepo = blogRepo ?? throw new ArgumentNullException(nameof(blogRepo));
        }

        public async Task CreateAsync(Blog entity)
        {
            try
            {
                await _blogRepo.CreateAsync(entity);
                await _blogRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Blog entity)
        {
            try
            {
                await _blogRepo.DeleteAsync(entity);
                await _blogRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Blog>> GetAllAsync(QueryObject query)
        {
            try
            {
                var blogs = await _blogRepo.GetAllAsync();

                var commentsIncluded = blogs
                    .Include(b => b.Author)
                    .Include(b => b.Comments)
                    .ThenInclude(b => b.AppUser)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(query.Search))
                {
                    commentsIncluded = blogs.Where(b => b.Title.ToLower().Contains(query.Search.ToLower())).AsQueryable();
                }

                if (!string.IsNullOrEmpty(query.Category))
                {
                    commentsIncluded = blogs.Where(b => b.Category.ToString().ToLower() == query.Category.ToLower()).AsQueryable();
                }

                return commentsIncluded;
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<Blog> GetByIdAsync(Guid id)
        {
            try
            {
                var blogs = await _blogRepo.GetAllAsync();
                var commentsIncluded = blogs
                    .Include(b => b.Author)
                    .Include(b => b.Comments)
                    .ThenInclude(b => b.AppUser)
                    .FirstOrDefault(b => b.Id == id);

                return commentsIncluded;
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Blog entity)
        {
            try
            {
                await _blogRepo.UpdateAsync(entity);
                await _blogRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }
    }
}
