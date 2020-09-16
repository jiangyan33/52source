using CommonEntity.Business;
using CommonTool;
using DbUtil.MySqlUtils;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

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

            string sqlWhere = "";

            if (content.Id != 0)
            {
                sqlWhere += $@" and id={content.Id} ";
            }
            else
            {
                sqlWhere += $@"and ifnull(parent_id,0) = {content.ParentId}";
            }

            if (!string.IsNullOrEmpty(content.Search))
            {
                sqlWhere += $@"and name like '%{content.Search}%'";
            }
            string sql = $@"select * from content where 1 {sqlWhere}";

            var res = DBHelper.MySqlDB.GetDataTable(pageNo, pageSize, content.Order, content.IsAsc, sql);
            return new PageResult<Content>() { Data = res.Table.SerializeToObject<Content>(), Pages = res.TotalPages, Rows = res.TotalRecords };
        }

        /// <summary>
        /// 只获取上一页、下一页、当前页信息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Navigation GetNavigation(Content content)
        {
            // 如果是单独的章节，上一章和下一章都是单独的章节，根据创建时间排序。
            // 如果是子章节，上一章和下一章是和该章节有相同的父章节

            if (content == null) return null;
            var navigation = new Navigation
            {
                CurrentPage = content
            };
            string sqlWhere = "";
            if (content.ParentId == null)
            {
                // 独立章节
                sqlWhere += $" and parent_id is null";
            }
            else if (content.ParentId > 0)
            {
                // 子集章节
                sqlWhere += $" and parent_id={content.ParentId}";
            }

            // 通过连开2条sql语句的方式搞定，已经存在一条数据。这样sort也不需要维护了。
            string sql = $@"SELECT * FROM content WHERE id !={content.Id}  {sqlWhere}
                            and create_date>='{content.CreateDate}'
                            ORDER BY create_date  limit 1";
            var res = DBHelper.MySqlDB.GetDataTable(sql);
            if (res.Table.Rows.Count > 0)
            {
                navigation.PreviousPage = res.Table.SerializeToObject<Content>()[0];
            }
            sql = $@"SELECT * FROM content WHERE id !={content.Id}  {sqlWhere} and create_date<='{content.CreateDate}'
                            ORDER BY
	                            create_date limit 1";
            res = DBHelper.MySqlDB.GetDataTable(sql);

            if (res.Table.Rows.Count > 0)
            {
                navigation.NextPage = res.Table.SerializeToObject<Content>()[0];
            }
            var appSetting = _configuration.GetSection("appSetting");
            content.Remark = File.ReadAllText(appSetting["textPath"] + "/" + content.Id + ".txt", Encoding.UTF8);

            return navigation;
        }
    }
}