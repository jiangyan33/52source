using CommonEntity.Business;
using CommonTool;
using DbUtil.MySqlUtils;
using System.Collections.Generic;

namespace _52sourceService.BusinessService
{
    public class CategoryService
    {
        public List<Category> List(Category category, int pageSize = 20)
        {
            string sqlWhere = "where 1";
            if (category.Id != 0)
            {
                sqlWhere += $" and id='{category.Id}'";
            }
            if (category.Enable != 0)
            {
                sqlWhere += $" and enable={category.Enable}";
            }
            string sql = $"select * from category {sqlWhere} order by create_date limit {pageSize}";
            var res = DBHelper.MySqlDB.GetDataTable(sql).Table.SerializeToObject<Category>();
            return res;
        }
    }
}