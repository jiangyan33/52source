using CommonEntity.MySql;

namespace DbUtil.MySqlUtils
{
    public class DBHelper
    {
        /// <summary>
        /// Mysql数据库操作对象
        /// </summary>
        public static MySqlDB MySqlDB { get; set; }

        private static DBOptions _dbOptions;

        /// <summary>
        /// 通过配置实例化DBHelper
        /// </summary>
        /// <param name="dbOptions">配置项</param>
        public DBHelper(DBOptions dbOptions)
        {
            _dbOptions = dbOptions;
            if (MySqlDB == null)
                MySqlDB = new MySqlDB(_dbOptions);
        }
    }
}