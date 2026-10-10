using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IService
{
    public interface IImageService
    {
        Task<string> GetPublicUrlAsync(string path);
        Task<string> UploadAsync(IFormFile file, string folder);
        Task DeleteAsync(string path);      
    }
}
