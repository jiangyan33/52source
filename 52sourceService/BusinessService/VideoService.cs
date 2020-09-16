using CommonEntity.Business;
using CommonTool;
using DbUtil.MySqlUtils;
using Microsoft.Extensions.Configuration;

namespace _52sourceService.BusinessService
{
    public class VideoService
    {
        public readonly IConfiguration _configuration;

        public VideoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PageResult<Video> List(Video video, int pageNo = -1, int pageSize = 20)
        {
            if (video.PageNum != 0)
            {
                pageNo = video.PageNum;
            }
            if (video.PageSize != 0)
            {
                pageSize = video.PageSize;
            }
            string where = "";
            if (video.Enable != 0)
            {
                where += $@" and enable={video.Enable}";
            }

            if (!string.IsNullOrEmpty(video.Search))
            {
                where += $@" and (name like '%{video.Search}%' or category_name like '%{video.Search}%')";
            }

            if (video.Id != 0)
            {
                where += $@" and id='{video.Id}'";
            }

            if (video.CategoryId != 0)
            {
                where += $@" and category_id='{video.CategoryId}'";
            }
            string sql = $@"select * from video where 1 {where}";

            var res = DBHelper.MySqlDB.GetDataTable(pageNo, pageSize, video.Order, video.IsAsc, sql);
            return new PageResult<Video>() { Data = res.Table.SerializeToObject<Video>(), Pages = res.TotalPages, Rows = res.TotalRecords };
        }

        public void SetData()
        {
            string sql = "select * from video";

            var res = DBHelper.MySqlDB.GetDataTable(sql).Table.SerializeToObject<Video>();

            foreach (var item in res)
            {
                sql = $"update video set path='/videoSource/{item.Name}' where id='{item.Id}'";
                DBHelper.MySqlDB.Execute(sql);
            }
        }
    }
}