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
            _videoService.SetData();
            return new Result(data: null);
        }

        [HttpGet("{id}")]
        public Result Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Result(ResultCode.ArgumentError);
            }
            var result = _videoService.List(new Video() { Id = id });
            return null;
        }
    }
}