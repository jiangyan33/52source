namespace CommonEntity.MySql
{
    /// <summary>
    /// 数据库连接选项
    /// </summary>
    public class DBOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 连接池最大连接数
        /// </summary>
        public int MaxConnections { get; set; } = 50;

        /// <summary>
        /// 连接池每次增长的连接数量
        /// </summary>
        public int IncrementalConnections { get; set; } = 5;

        /// <summary>
        /// 连接池初始连接数
        /// </summary>
        public int InitialConnections { get; set; } = 10;

        /// <summary>
        /// 数据库执行语句时的超时时间(s)
        /// </summary>
        public int CommandTimeout { get; set; } = 1800;

        /// <summary>
        /// 是否应用连接池
        /// </summary>
        public bool UseConnectPool { get; set; }

        public DBOptions()
        {
        }

        public DBOptions(string connStr)
        {
            ConnectionString = connStr;
        }
    }
}