using _52sourceService.BusinessService;
using CommonEntity.Business;
using CommonEntity.Common;
using Microsoft.AspNetCore.Mvc;

namespace _52source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private VideoService _videoService;
        public CategoryService _categoryService;

        public CategoriesController(VideoService videoService, CategoryService categoryService)
        {
            _videoService = videoService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public Result Get([FromForm] Category category)
        {
            category.Enable = 1;
            var result = _categoryService.List(category);
            return new Result(data: result);
        }

        // 某一个分类下的资源列表
        [HttpGet("{id}/videos")]
        public Result Get(int id, [FromQuery] int pageNum = 1, [FromQuery] int pageSize = 24, [FromQuery] string order = "hot", [FromQuery] bool isAsc = false)
        {
            if (id == 0)
            {
                return new Result(ResultCode.ArgumentError);
            }
            var video = new Video
            {
                PageNum = pageNum,
                PageSize = pageSize,
                Order = order,
                IsAsc = isAsc,
                CategoryId = id
            };

            var pageResult = _videoService.List(video);
            return new Result(data: pageResult);
        }
    }
}