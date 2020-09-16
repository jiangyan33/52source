using _52sourceService.BusinessService;
using CommonEntity.Business;
using CommonEntity.Common;
using Microsoft.AspNetCore.Mvc;

namespace _52source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        public ContentService _contentService;

        public ContentsController(ContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public Result Get([FromQuery] Content content)
        {
            var result = _contentService.List(content);
            return new Result(data: result);
        }

        [HttpGet("{id}")]
        public Result Get(int id)
        {
            var result = _contentService.List(new Content { Id = id, PageNum = -1 });
            var content = result.Data[0];
            var data = _contentService.GetNavigation(content);
            return new Result(data: data);
        }
    }
}