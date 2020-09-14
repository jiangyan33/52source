using _52sourceService.BusinessService;
using CommonEntity.Business;
using CommonEntity.Common;
using Microsoft.AspNetCore.Mvc;

namespace _52source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        public VideoService _videoService;

        public VideosController(VideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet]
        public Result Get([FromQuery] Video video)
        {
            video.Enable = 1;
            video.IsAsc = false;
            var result = _videoService.List(video);
            return new Result(data: result);
        }

        [HttpGet("{id}")]
        public Result Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Result(ResultCode.ArgumentError);
            }
            var result = _videoService.List(new Video() { Id = id });
            return new Result(data: result);
        }
    }
}