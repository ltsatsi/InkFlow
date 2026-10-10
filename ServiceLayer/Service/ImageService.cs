using DomainLayer.Dtos.Blog;
using Microsoft.AspNetCore.Http;
using ServiceLayer.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Service
{
    public class ImageService : IImageService
    {   
        private readonly IStorageService _storageService;
        public ImageService(IStorageService storageService)
        {
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task DeleteAsync(string path)
        {
            try
            {
                _storageService.DeleteImageAsync(path);
            } catch (Exception)
            {
                throw;
            }
        } // end method

        public async Task<string> GetPublicUrlAsync(string path)
        {
            try
            {
                return _storageService.GetImageUrlAsync(path);
            }
            catch (Exception)
            {
                throw;
            }
        } // end method

        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            try
            {
                // TODO: Validate the file
                return await _storageService.UploadImageAsync(file, folder);    
            } catch (Exception)
            {
                throw;
            }
        } // end method
    }
}
