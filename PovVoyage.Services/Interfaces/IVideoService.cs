using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PovVoyage.Data.Context;
using PovVoyage.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PovVoyage.Services.Interfaces
{
    public interface IVideoService
    {


        Task<List<VideoDto>> GetAllVideosAsync();
        Task<VideoDto> GetVideoByIdAsync(int id);

        Task<(string videoUrl, string thumbnailUrl)> UploadVideoAsync(IFormFile videoFile, IFormFile thumbnailFile, string title, string description, TimeSpan duration);

        Task<bool> UpdateVideoAsync(int id, VideoDto video);
        Task<bool> DeleteVideoAsync(int id);
        Task<bool> IncrementViewCountAsync(int id);
        Task<bool> LikeVideoAsync(int id);
        Task<bool> DislikeVideoAsync(int id);
    }
}
