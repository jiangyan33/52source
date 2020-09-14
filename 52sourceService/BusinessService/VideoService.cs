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

            if (!string.IsNullOrEmpty(video.Id))
            {
                where += $@" and id='{video.Id}'";
            }

            if (!string.IsNullOrEmpty(video.CategoryId))
            {
                where += $@" and category_id='{video.CategoryId}'";
            }
            string sql = $@"select * from video where 1 {where}";

            var res = DBHelper.MySqlDB.GetDataTable(pageNo, pageSize, video.Order, video.IsAsc, sql);
            return new PageResult<Video>() { Data = res.Table.SerializeToObject<Video>(), Pages = res.TotalPages, Rows = res.TotalRecords };
        }

        //public void SetData()
        //{
        //    var videosPath = _configuration.GetSection("appSetting")["videosPath"];
        //    var files = Directory.GetFiles(videosPath);
        //    var videoList = new List<Video>();
        //    foreach (var item in files)
        //    {
        //        var video = new Video
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Name = Path.GetFileName(item),
        //            CategoryId = "754f2033-f32f-11ea-82af-8c164597623f",
        //            CategoryName = "编程语言",
        //            CreateBy = "8249c886-b07f-11ea-a064-00163e10d0a2",
        //            UpdateBy = "8249c886-b07f-11ea-a064-00163e10d0a2",
        //            CreateDate = DateTime.Now,
        //            UpdateDate = DateTime.Now,
        //            Info = "暂无",
        //            Path = item,
        //            Pic = "http://120.79.185.158:3999/upload/img_dev/202096/c6af2448-9689-4042-a5c0-89a744474da6.jpg",
        //            Hot = 0,
        //            Enable = 1
        //        };
        //        var sql = @"INSERT INTO video (
        //                                 id,
        //                                 NAME,
        //                                 info,
        //                                 path,
        //                                 pic,
        //                                 hot,
        //                                 category_id,
        //                                 category_name,
        //                                 create_date,
        //                                 create_by,
        //                                 update_date,
        //                                 update_by,
        //                                 remark,
        //                                 ENABLE
        //                                 )
        //                                VALUES
        //                                 (
        //                                  @Id,
        //                                  @NAME,
        //                                  @Info,
        //                                  @Path,
        //                                  @Pic,
        //                                  @Hot,
        //                                  @CategoryId,
        //                                  @CategoryName,
        //                                  @CreateDate,
        //                                  @CreateBy,
        //                                  @UpdateDate,
        //                                  @UpdateBy,
        //                                  @Remark,
        //                                     @ENABLE
        //                                 )";
        //        DBHelper.MySqlDB.Execute(sql, ConvertHelper.ConvertObjToKeyPairObject(video));
        //    }
        //}
    }
}