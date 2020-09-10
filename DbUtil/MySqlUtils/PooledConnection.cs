using MySqlConnector;

namespace DbUtil.MySqlUtils
{
    /// <summary>
    /// 数据库连接池管理对象
    /// </summary>
    public class PooledConnection
    {
        private MySqlConnection _mConnection = null;// 数据库连接

        /// <summary>
        /// 构造函数，根据一个 Connection 构告一个 PooledConnection 对象
        /// </summary>
        /// <param name="connection">MySqlConnection</param>
        public PooledConnection(ref MySqlConnection connection)
        {
            _mConnection = connection;
        }

        /// <summary>
        /// 返回此对象中的连接
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection()
        {
            return _mConnection;
        }

        /// <summary>
        /// 设置此对象的，连接
        /// </summary>
        /// <param name="connection">MySqlConnection</param>
        public void SetConnection(ref MySqlConnection connection)
        {
            _mConnection = connection;
        }

        /// <summary>
        /// 获得对象连接是否忙
        /// </summary>
        public bool IsBusy { get; set; } = false;
    }
}