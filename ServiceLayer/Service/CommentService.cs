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
    public class CommentService : ICustomService<Comment>
    {
        private readonly IRepository<Comment> _commentRepo;
        public CommentService(IRepository<Comment> commentRepo)
        {
            _commentRepo = commentRepo ?? throw new ArgumentNullException(nameof(commentRepo));
        }


        public async Task CreateAsync(Comment entity)
        {
            try
            {
                await _commentRepo.CreateAsync(entity);
                await _commentRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Comment entity)
        {
            try
            {
                await _commentRepo.DeleteAsync(entity);
                await _commentRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Comment>> GetAllAsync(QueryObject query)
        {
            try
            {
                var comments = await _commentRepo.GetAllAsync();
                var usersIncluded = comments.Include(c => c.AppUser).ToList();

                return usersIncluded;
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task<Comment> GetByIdAsync(Guid id)
        {
            try
            {
                var comments = await _commentRepo.GetAllAsync();
                var userIncluded = await comments.Include(c => c.AppUser).FirstOrDefaultAsync(c => c.Id == id);

                return userIncluded;
            } catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateAsync(Comment entity)
        {
            try
            {
                await _commentRepo.UpdateAsync(entity);
                await _commentRepo.SaveChangesAsync();
            } catch (Exception)
            {
                throw;
            }
        }
    }
}
