using CommonEntity.Business;
using CommonTool;
using DbUtil.MySqlUtils;
using Microsoft.Extensions.Configuration;

namespace _52sourceService.BusinessService
{
    public class ContentService
    {
        private IConfiguration _configuration;

        public ContentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PageResult<Content> List(Content content, int pageNo = -1, int pageSize = 20)
        {
            if (content.PageNum != 0)
            {
                pageNo = content.PageNum;
            }
            if (content.PageSize != 0)
            {
                pageSize = content.PageSize;
            }
            string where = "";

            if (!string.IsNullOrEmpty(content.Search))
            {
                where += $@"and name like '%{content.Search}%'";
            }

            if (!string.IsNullOrEmpty(content.Id))
            {
                where += $@"and id='{content.Id}'";
            }
            else
            {
                where += "and ifnull(parent_id,'0') ='0'";
            }

            if (!string.IsNullOrEmpty(content.ParentId))
            {
                where += $@"and parent_id = '{content.Id}'";
            }

            string sql = $@"select * from content where 1 {where}";

            var res = DBHelper.MySqlDB.GetDataTable(pageNo, pageSize, content.Order, content.IsAsc, sql);
            return new PageResult<Content>() { Data = res.Table.SerializeToObject<Content>(), Pages = res.TotalPages, Rows = res.TotalRecords };
        }
    }
}