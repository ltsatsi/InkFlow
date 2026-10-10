using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IService
{
    public interface IStorageService
    {
        Task DeleteImageAsync(string path);
        string GetImageUrlAsync(string path);
        Task<string> UploadImageAsync(IFormFile file, string folder);
    }
}
