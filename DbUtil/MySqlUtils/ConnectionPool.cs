using CommonEntity.MySql;
using LogUtil;
using MySqlConnector;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace DbUtil.MySqlUtils
{
    /// <summary>
    /// 数据库连接池
    /// </summary>
    public class ConnectionPool : IDisposable
    {
        private List<PooledConnection> _mPooledconnections = null; // 存放连接池中数据库连接的向量
        private readonly string _mySqlConnection = "";//ConfigurationManager.ConnectionStrings["ConnectSql"].ConnectionString;
        private static readonly object objectLocker = new object();
        private readonly ILogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库连接字配置</param>
        private ConnectionPool(DBOptions options)
        {
            _logger = LogHelper.Logger;
            _mySqlConnection = options.ConnectionString;
            InitialConnections = options.InitialConnections;
            IncrementalConnections = options.IncrementalConnections;
            MaxConnections = options.MaxConnections;
        }

        /// <summary>
        /// 创建一个数据库连接池，连接池中的可用连接的数量采用类成员 initialConnections 中设置的值
        /// </summary>
        public ConnectionPool CreatePool()
        {
            // 如果己经创建，则返回
            if (_mPooledconnections != null)
                return this;

            _mPooledconnections = new List<PooledConnection>();
            // 根据 initialConnections 中设置的值，创建连接。
            CreateConnections(InitialConnections);

            return this;
        }

        public static ConnectionPool CreateConnectionPoolInstance(DBOptions options)
        {
            return new ConnectionPool(options);
        }

        /// <summary>
        /// 连接池名称
        /// </summary>
        public string PoolName { get; set; }

        /// <summary>
        /// 获取或设置连接池的初始大小
        /// </summary>
        public int InitialConnections { get; set; } = 10;

        /// <summary>
        /// 获取或设置连接池自动增加的大小
        /// </summary>
        public int IncrementalConnections { get; set; } = 5;

        /// <summary>
        /// 获取或设置连接池中最大的可用连接数量
        /// </summary>
        public int MaxConnections { get; set; } = 50;

        /// <summary>
        /// 获取或设置测试数据库表的名字
        /// </summary>
        public String TestTable { get; set; } = "";

        /// <summary>
        /// 获取工作中的连接数量
        /// </summary>
        /// <returns></returns>
        public int GetBusyConnectionsCount()
        {
            return _mPooledconnections.Count(x => x.IsBusy);
        }

        /// <summary>
        /// 获取空闲的连接数量
        /// </summary>
        /// <returns></returns>
        public int GetFreeConnectionsCount()
        {
            return _mPooledconnections.Count(x => !x.IsBusy);
        }

        /// <summary>
        /// 创建由 numConnections 指定数目的数据库连接 , 并把这些连接 放入 m_Pooledconnections
        /// </summary>
        /// <param name="numConnections"></param>
        private void CreateConnections(int numConnections)
        {
            // 循环创建指定数目的数据库连接
            for (int x = 0; x < numConnections; x++)
            {
                if (MaxConnections > 0 && _mPooledconnections.Count >= MaxConnections)
                {
                    _logger.Trace($"连接数已经达到最大值无法追加.");
                    break;
                }
                _logger.Trace($"追加了 1 个连接.");
                try
                {
                    MySqlConnection tmpConnect = NewConnection();
                    _mPooledconnections.Add(new PooledConnection(ref tmpConnect));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"追加连接 {x + 1} 异常");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 创建一个新的数据库连接并返回它
        /// </summary>
        /// <returns></returns>
        private MySqlConnection NewConnection()
        {
            // 创建一个数据库连接
            MySqlConnection conn = new MySqlConnection(_mySqlConnection);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 通过调用 getFreeConnection() 函数返回一个可用的数据库连接 , 如果当前没有可用的数据库连接，并且更多的数据库连接不能创
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnection()
        {
            // 确保连接池己被创建
            if (_mPooledconnections == null)
                return null; // 连接池还没创建，则返回 null

            lock (objectLocker)
            {
                MySqlConnection conn = GetFreeConnection(); // 获得一个可用的数据库连接
                                                            // 如果目前没有可以使用的连接，即所有的连接都在使用中
                while (conn == null)
                {
                    Thread.Sleep(250);
                    conn = GetFreeConnection(); // 重新再试，直到获得可用的连接，如果
                }
                return conn;
            }
        }

        /// <summary>
        /// 本函数从连接池向量 connections 中返回一个可用的的数据库连接，如果 当前没有可用的数据库连接，本函数则根据 incrementalConnections 设置
        /// 的值创建几个数据库连接，并放入连接池中。 如果创建后，所有的连接仍都在使用中，则返回 null
        /// </summary>
        /// <returns></returns>
        private MySqlConnection GetFreeConnection()
        {
            // 从连接池中获得一个可用的数据库连接
            MySqlConnection conn = FindFreeConnection();
            if (conn == null)
            {
                CreateConnections(IncrementalConnections);
                // 重新从池中查找是否有可用连接
                conn = FindFreeConnection();
            }
            _logger.Trace($"分配了一个数据库连接.");
            _logger.Trace($"当前工作中的连接数：{GetBusyConnectionsCount()}, 空闲的连接数：{GetFreeConnectionsCount()}, 最大连接数：{MaxConnections}");
            return conn;
        }

        /// <summary>
        /// 查找连接池中所有的连接，查找一个可用的数据库连接， 如果没有可用的连接，返回 null
        /// </summary>
        /// <returns></returns>
        private MySqlConnection FindFreeConnection()
        {
            lock (objectLocker)
            {
                MySqlConnection conn = null;
                // 遍历所有的对象，看是否有可用的连接
                for (int i = 0; i < _mPooledconnections.Count; ++i)
                {
                    if (!_mPooledconnections[i].IsBusy)
                    {
                        conn = _mPooledconnections[i].GetConnection();
                        _mPooledconnections[i].IsBusy = true;
                        // 测试此连接是否可用
                        if (!TestConnection(ref conn))
                        {
                            // 如果此连接不可再用了，则创建一个新的连接， 并替换此不可用的连接对象，如果创建失败，返回 null
                            try
                            {
                                conn = NewConnection();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            _mPooledconnections[i].SetConnection(ref conn);
                        }
                        break; // 己经找到一个可用的连接，退出
                    }
                }
                return conn;// 返回找到到的可用连接
            }
        }

        /// <summary>
        /// 测试一个连接是否可用，如果不可用，关掉它并返回 false 否则可用返回 true
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private bool TestConnection(ref MySqlConnection conn)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    return false;
                //查询系统表
                string sql = "select 1;";
                using (MySqlCommand dbComm = new MySqlCommand(sql, conn))
                {
                    dbComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "数据库连接失败.." + ex.Message);
                // 上面抛出异常，此连接己不可用，关闭它，并返回 false;
                CloseConnection(conn);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 此函数返回一个数据库连接到连接池中，并把此连接置为空闲。 所有使用连接池获得的数据库连接均应在不使用此连接时返回它。
        /// </summary>
        /// <param name="conn"></param>
        public void ReturnConnection(MySqlConnection conn)
        {
            // 确保连接池存在，如果连接没有创建（不存在），也负责关闭指定连接
            if (_mPooledconnections == null || !_mPooledconnections.Any(x => x.GetConnection() == conn))
            {
                conn.Close();
                conn.Dispose();
                return;
            }
            lock (objectLocker)
            {
                var temp = _mPooledconnections.Find(x => x.GetConnection() == conn);
                if (temp != null)
                {
                    _logger.Trace($"回收了一个数据库连接.");
                    temp.IsBusy = false;
                }
            }
            _logger.Trace($"当前工作中的连接数：{GetBusyConnectionsCount()}, 空闲的连接数：{GetFreeConnectionsCount()}, 最大连接数：{MaxConnections}");
        }

        /// <summary>
        /// 刷新连接池中所有的连接对象
        /// </summary>
        public void RefreshConnections()
        {
            // 确保连接池己创新存在
            if (_mPooledconnections == null)
                return;

            lock (objectLocker)
            {
                for (int i = 0; i < _mPooledconnections.Count; ++i)
                {
                    if (_mPooledconnections[i].IsBusy)
                        Thread.Sleep(5000); //等待5s

                    // 关闭此连接，用一个新的连接代替它。
                    CloseConnection(_mPooledconnections[i].GetConnection());
                    MySqlConnection tmpConnect = NewConnection();
                    _mPooledconnections[i].SetConnection(ref tmpConnect);
                    _mPooledconnections[i].IsBusy = false;
                }
            }
        }

        /// <summary>
        /// 关闭连接池中所有的连接，并清空连接池。
        /// </summary>
        public void CloseConnectionPool()
        {
            // 确保连接池己创新存在
            if (_mPooledconnections == null)
                return;

            for (int i = 0; i < _mPooledconnections.Count; ++i)
            {
                if (_mPooledconnections[i].IsBusy)
                    Thread.Sleep(5000); //等待5s

                // 关闭此连接，用一个新的连接代替它。
                CloseConnection(_mPooledconnections[i].GetConnection());
                _mPooledconnections.RemoveAt(i);
            }
            _mPooledconnections.Clear();
            _mPooledconnections = null;
        }

        /// <summary>
        /// 关闭一个数据库连接
        /// </summary>
        /// <param name="conn"></param>
        private void CloseConnection(MySqlConnection conn)
        {
            try
            {
                lock (objectLocker)
                {
                    _logger.Trace($"关闭了一个数据库连接.");
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"关闭连接异常.");
                throw ex;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="isDisposing">挂起</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _mPooledconnections.ForEach(x => x.GetConnection().Close());
            }
        }
    }
}