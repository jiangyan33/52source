using CommonEntity.MySql;
using System.Collections;

namespace DbUtil.MySqlUtils
{
    /// <summary>
    /// 数据库连接池集合
    /// </summary>
    public class ConnectionPoolList
    {
        private readonly Hashtable _poolList;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConnectionPoolList()
        {
            _poolList = new Hashtable();
        }

        /// <summary>
        /// 创建一个连接池
        /// </summary>
        /// <param name="key"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool CreateConnectionPool(string key, DBOptions option)
        {
            if (!_poolList.ContainsKey(key))
            {
                _poolList.Add(key, ConnectionPool.CreateConnectionPoolInstance(option).CreatePool());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除一个连接池
        /// </summary>
        /// <param name="key"></param>
        public void RemoveConnectionPool(string key)
        {
            if (_poolList.ContainsKey(key))
                _poolList.Remove(key);
        }

        /// <summary>
        /// 获取一个连接池
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ConnectionPool GetConnectionPool(string key)
        {
            if (_poolList.ContainsKey(key))
                return (ConnectionPool)_poolList[key];
            else
                return null;
        }
    }
}