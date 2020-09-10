using CommonEntity.MySql;
using DbUtil.MySqlUtils;

namespace _52sourceService
{
    public class IndexService
    {
        /// <summary>
        /// 初始化service模块的数据库链接信息
        /// </summary>
        /// <param name="options"></param>
        public static void InitDbUtils(DBOptions options)
        {
            new DBHelper(options);
        }
    }
}