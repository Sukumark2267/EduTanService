using Azure.Storage.Blobs;
using PovVoyage.Services.Interfaces;
using PovVoyage.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Xabe.FFmpeg;
using PovVoyage.Data.Context;
using Microsoft.Extensions.Configuration;
using PovVoyage.Data.Entities;
using Xabe.FFmpeg;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace PovVoyage.Services.Implementations
{


    public class VideoService : IVideoService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly PovVoyageContext _context;
        private readonly ILogger<VideoService> _logger;
        private readonly string _containerName;

        public VideoService(IConfiguration configuration, PovVoyageContext context, ILogger<VideoService> logger)
        {
            var connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");
            _context = context;
            _logger = logger;

            _logger.LogInformation("VideoService initialized with Azure Blob Storage.");
        }



        public async Task<(string videoUrl, string thumbnailUrl)> UploadVideoAsync(IFormFile videoFile, IFormFile thumbnailFile,string title,string description, TimeSpan duration)
        {

            try
            {
                // Check if the video file size exceeds 100MB
                const long maxFileSize = 104857600; // 100MB in bytes
                if (videoFile.Length > maxFileSize)
                {
                    throw new Exception("Video file size exceeds the 100MB limit.");
                }
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                // Upload video
                var videoBlobName = $"{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
                var videoBlobClient = containerClient.GetBlobClient(videoBlobName);

                using (var stream = videoFile.OpenReadStream())
                {
                    await videoBlobClient.UploadAsync(stream, true);
                }

                // Upload thumbnail
                var thumbnailBlobName = $"thumbnails/{Path.GetFileNameWithoutExtension(videoBlobName)}.jpg";
                var thumbnailBlobClient = containerClient.GetBlobClient(thumbnailBlobName);

                using (var stream = thumbnailFile.OpenReadStream())
                {
                    await thumbnailBlobClient.UploadAsync(stream, true);
                }

                // Save metadata
                var metadata = new VideoMetadata
                {
                    VideoUrl = videoBlobClient.Uri.ToString(),
                    ThumbnailUrl = thumbnailBlobClient.Uri.ToString(),
                    Title = title,
                    Description = description,
                    UploadDate = DateTime.UtcNow,
                    Duration = duration
                };
                //if (string.IsNullOrEmpty(metadata.VideoUrl) || string.IsNullOrEmpty(metadata.ThumbnailUrl))
                //{
                //    _logger.LogError("❌ VideoUrl or ThumbnailUrl is empty!"+ metadata.ThumbnailUrl);
                //    throw new Exception("Invalid metadata: Video URL or Thumbnail is missing.");
                //}

                _context.VideoMetadata.Add(metadata);
                _logger.LogInformation("✅ Video metadata sql started!");
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogInformation("User uploaded a video: {VideoUrl}", metadata.VideoUrl, metadata.ThumbnailUrl, metadata.Duration, metadata.Title, metadata.Description);

                    _logger.LogError(ex, "Database update failed. Inner exception: {InnerException}", ex.InnerException?.Message);
                    throw;
                }
                _logger.LogInformation("✅ Video metadata saved successfully!");

                return (metadata.VideoUrl, metadata.ThumbnailUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($" Error saving metadata: {ex.Message}");
                throw;
            }
        }

        public async Task<VideoDto> GetVideoByIdAsync(int id)
        {
            // Implementation here
            throw new NotImplementedException();
        }

        public async Task<List<VideoDto>> GetAllVideosAsync()
        {
            try
            {
                var videosFromSql = await _context.VideoMetadata.ToListAsync();

                // Create tasks for each video DTO with async thumbnail URL generation
                var videoTasks = videosFromSql.Select(async v => new VideoDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    VideoUrl = v.VideoUrl,
                    ThumbnailUrl = await GetThumbnailUrlAsync(v.VideoUrl), // Now allowed
                    Description = v.Description,
                    UploadDate = v.UploadDate,
                    Duration = v.Duration,
                    Views   = v.Views,
                    Likes = v.Likes,   
                    Dislikes    = v.Dislikes,  
                }).ToList(); // Start all tasks

                // Wait for all tasks to complete
                var allVideos = await Task.WhenAll(videoTasks);

                return allVideos.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching videos.");
                throw;
            }
        }

        private string GenerateVideoUrl(string blobName)
        {
            return $"https://povvideostorage.blob.core.windows.net/{_containerName}/{blobName}";
        }

        private async Task<string> GetThumbnailUrlAsync(string videoUrl)
        {
            // Extract the video name without extension
            var videoName = Path.GetFileNameWithoutExtension(videoUrl);
            var thumbnailUrl = $"https://povvideostorage.blob.core.windows.net/{_containerName}/thumbnails/{videoName}.jpg";

            // Check if the thumbnail exists in Blob Storage
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var thumbnailBlob = blobContainerClient.GetBlobClient($"thumbnails/{videoName}.jpg");

            try
            {
                // Check if the thumbnail blob exists
                if (await thumbnailBlob.ExistsAsync())
                {
                    return thumbnailUrl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking thumbnail existence.");
            }

            // Return a default thumbnail URL if the thumbnail doesn't exist
            return "https://povvideostorage.blob.core.windows.net/default-thumbnail.jpg";
        }


        public async Task UploadVideoAsync(VideoDto video)
        {
            // Implementation here
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateVideoAsync(int id, VideoDto video)
        {
            // Implementation here
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteVideoAsync(int id)
        {
            // Implementation here
            throw new NotImplementedException();
        }
        public async Task<bool> IncrementViewCountAsync(int id)
        {
            var video = await _context.VideoMetadata.FindAsync(id);
            if (video == null) return false;

            video.Views++;  // Increment view count
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikeVideoAsync(int id)
        {
            var video = await _context.VideoMetadata.FindAsync(id);
            if (video == null) return false;

            video.Likes++;  // Increment like count
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DislikeVideoAsync(int id)
        {
            var video = await _context.VideoMetadata.FindAsync(id);
            if (video == null) return false;

            video.Dislikes++;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

