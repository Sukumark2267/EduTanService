using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PovVoyage.Services.Interfaces;
using PovVoyage.Services.Models;
using Microsoft.Extensions.Logging;

namespace PovVoyage.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly ILogger<VideosController> _logger;
        private readonly IVideoService _videoService;

        public VideosController(ILogger<VideosController> logger, IVideoService videoService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _videoService = videoService;
        }

        [HttpGet("GetVideos")]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return Ok(video);
        }

        [HttpPost("UploadVideo")]
        [RequestSizeLimit(524288000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 524288000)]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile videoFile, [FromForm] IFormFile thumbnailFile, [FromForm] string title, [FromForm] string description, [FromForm] string duration)
        {
            if (videoFile == null || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                return BadRequest("Video file, title, and description are required.");
            }
            if (!TimeSpan.TryParse(duration, out TimeSpan parsedDuration))
            {
                return BadRequest("Invalid duration format. Please use hh:mm:ss or total seconds.");
            }
            // ✅ Log request details
            _logger.LogInformation($"Received video upload request: Title={title}, Description={description}");
            _logger.LogInformation($"Video File: {videoFile?.FileName}, Size: {videoFile?.Length}");
            _logger.LogInformation($"Thumbnail File: {thumbnailFile?.FileName}, Size: {thumbnailFile?.Length}");

            try
            {
                 (string videoUrl,string  thumbnailUrl) = await _videoService.UploadVideoAsync(videoFile, thumbnailFile, title, description, parsedDuration);
                var videoDto = new VideoDto
                {
                    VideoUrl = videoUrl,
                    ThumbnailUrl = thumbnailUrl,
                    Title = title,
                    Description = description,
                    UploadDate = DateTime.UtcNow,

                };

                return CreatedAtAction(nameof(GetVideoById), new { id = videoDto.Id }, videoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while uploading the video controller.");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVideo(int id, [FromBody] VideoDto video)
        {
            var updated = await _videoService.UpdateVideoAsync(id, video);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var deleted = await _videoService.DeleteVideoAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{id}/views")]
        public async Task<IActionResult> IncrementView(int id)
        {
            bool success = await _videoService.IncrementViewCountAsync(id);
            if (!success) return NotFound("Video not found");
            return Ok("View count updated");
        }

        // Like a video
        [HttpPost("{id}/likes")]
        public async Task<IActionResult> LikeVideo(int id)
        {
            bool success = await _videoService.LikeVideoAsync(id);
            if (!success) return NotFound("Video not found");
            return Ok("Like count updated");
        }
        [HttpPost("{id}/dislikes")]
        public async Task<IActionResult> DislikeVideo(int id)
        {
            bool success = await _videoService.DislikeVideoAsync(id);
            if (!success) return NotFound("Video not found");
            return Ok("Dislike count updated");
        }
    }

}
