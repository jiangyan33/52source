using _52sourceService.BusinessService;
using CommonEntity.Business;
using CommonEntity.Common;
using Microsoft.AspNetCore.Mvc;

namespace _52source.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        public ContentService _contentService;

        public ContentController(ContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public Result Get([FromQuery] Content content)
        {
            var result = _contentService.List(content);
            return new Result(data: result);
        }
    }
}