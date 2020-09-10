using CommonEntity.MySql;
using LogUtil;
using MySqlConnector;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace DbUtil.MySqlUtils
{
    /// <summary>
    /// 数据库访问对象
    /// </summary>
    public class MySqlDB : IDisposable
    {
        /// <summary>
        /// 数据库对象的唯一id
        /// </summary>
        public string ID { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 数据库对象的Key，可以用来区分连接池
        /// </summary>
        public string Key { get; set; } = "Default";

        /// <summary>
        /// 数据库对象连接池，一个数据库对象拥有一个连接池
        /// </summary>
        private readonly ConnectionPool connectionPool;

        private readonly MySqlCommand sqlCommand;

        private readonly ILogger _logger = LogHelper.Logger;

        #region //数据库访问

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public DBOptions MySqlDBOptions { get; set; } = new DBOptions();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mySqlDBOptions"></param>
        public MySqlDB(DBOptions mySqlDBOptions)
        {
            MySqlDBOptions = mySqlDBOptions;

            // 创建一个数据库连接
            if (MySqlDBOptions.UseConnectPool && connectionPool == null)
                connectionPool = ConnectionPool.CreateConnectionPoolInstance(mySqlDBOptions).CreatePool();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mySqlDBOptions"></param>
        public MySqlDB(string connStr, int timeout = 1800)
        {
            // 创建一个数据库连接
            MySqlDBOptions = new DBOptions { ConnectionString = connStr, CommandTimeout = timeout };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mySqlDBOptions"></param>
        public MySqlDB(MySqlCommand mySqlCommand)
        {
            sqlCommand = mySqlCommand;
        }

        /// <summary>
        /// 获取一个数据库连接
        /// <para>如果没有启用连接池则返回一个单连接</para>
        /// </summary>
        /// <param name="needCreate">是否强制创建</param>
        /// <returns></returns>
        private MySqlConnection GetConnection(bool needCreate = true)
        {
            try
            {
                if (!needCreate && sqlCommand != null)
                    return sqlCommand.Connection;

                if (MySqlDBOptions.UseConnectPool && connectionPool != null)
                    return connectionPool.GetConnection();

                if (sqlCommand != null)
                    return new MySqlConnection(sqlCommand.Connection.ConnectionString);

                return new MySqlConnection(MySqlDBOptions.ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 释放连接
        /// <para>如果使用了连接池则用连接池释放</para>
        /// </summary>
        /// <param name="conn"></param>
        private void CloseConnection(MySqlConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                if (connectionPool != null) connectionPool.ReturnConnection(conn);
                else { conn.Close(); conn.Dispose(); }
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="cmd">sqlCommand对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <returns></returns>
        public DBResult Execute(MySqlConnection conn, MySqlCommand cmd, string sql, params object[] param)
        {
            DBResult result = new DBResult();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                InitParameters(cmd, param);
                result.AffectRowCount = cmd.ExecuteNonQuery();//执行非查询SQL语句
                result.IsSuccessed = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            return result;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <returns></returns>
        public DBResult Execute(string sql, params object[] param)
        {
            DBResult result = new DBResult();
            // 执行语句，不强制创建conn对象，如果有则继承，这样才能保证事务的正常运行
            var conn = GetConnection(false);
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (sqlCommand == null)
                {
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        result = Execute(conn, cmd, sql, param);
                    };
                }
                else
                    result = Execute(conn, sqlCommand, sql, param);
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            finally
            {
                // 释放连接的时候，如果sqlCommand不为null，则为事务传递中的Command对象，在这里不做释放，否则正常释放
                if (sqlCommand == null)
                    CloseConnection(conn);
            }

            return result;
        }

        [Obsolete("可能在以后会移除此方法，请使用 ExecuteWithTransaction(Func<MySqlDB, DBResult> action) 代替")]
        /// <summary>
        /// 通过事务批量执行sql语句
        /// </summary>
        /// <param name="sql">sql语句集合</param>
        /// <returns></returns>
        public DBResult ExecuteWithTransaction(List<string> sql)
        {
            DBResult result = new DBResult();
            var conn = GetConnection();
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                    var transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        int i = 1;
                        int count = sql.Count;
                        _logger.Info($"本次事务共须执行{count}次.");
                        foreach (var item in sql)
                        {
                            cmd.CommandText = item;
                            result.AffectRowCount += cmd.ExecuteNonQuery();//执行非查询SQL语句
                            if (i % 500 == 0)
                                _logger.Info($"当前执行进度{i}/{count}次.");
                            i++;
                        }

                        _logger.Info($"本次事务执行完毕.");
                        transaction.Commit();
                        result.IsSuccessed = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.IsSuccessed = false;
                        result.Message = ex.Message;
                        _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
                    }
                }
            }
            catch (Exception e)
            {
                result.IsSuccessed = false;
                result.Message = e.Message;
            }
            finally
            {
                CloseConnection(conn);
            }

            return result;
        }

        /// <summary>
        /// 通过事务执行sql
        /// </summary>
        /// <param name="action">MySqlDB对象</param>
        /// <returns></returns>
        public DBResult ExecuteWithTransaction(Func<MySqlDB, DBResult> action)
        {
            DBResult result = new DBResult();
            var conn = GetConnection();
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                    var transaction = conn.BeginTransaction();
                    cmd.Transaction = transaction;
                    try
                    {
                        // 创建一个单链接的数据库对象
                        result = action(new MySqlDB(cmd));
                        if (result.IsSuccessed)
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.IsSuccessed = false;
                        result.Message = ex.Message;
                        _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            finally
            {
                CloseConnection(conn);
            }

            return result;
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="cmd">sqlCommand对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <returns></returns>
        public DBResult GetTopObject(MySqlConnection conn, MySqlCommand cmd, string sql, params object[] param)
        {
            DBResult result = new DBResult();
            try
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                InitParameters(cmd, param);
                result.DataObject = cmd.ExecuteScalar();//执行非查询SQL语句
                result.IsSuccessed = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }

            return result;
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql">sql语句模板</param>
        /// <param name="param">sql语句参数</param>
        /// <returns></returns>
        public DBResult GetTopObject(string sql, params object[] param)
        {
            DBResult result = new DBResult();
            var conn = GetConnection();
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    result = GetTopObject(conn, cmd, sql, param);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            finally
            {
                CloseConnection(conn);
            }

            return result;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql">sql语句模板</param>
        /// <param name="param">sql语句参数</param>
        /// <returns></returns>
        public DBResult GetDataTable(string sql, params object[] param)
        {
            DBResult result = new DBResult();
            var conn = GetConnection();
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    result = GetDataSet(conn, cmd, sql, param);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            finally
            {
                CloseConnection(conn);
            }

            return result;
        }

        /// <summary>
        /// 通过分页的方式获取DataTable
        /// </summary>
        /// <param name="currPage">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageBy">分页排序时用到的字段</param>
        /// <param name="isAsc">是否时升序</param>
        /// <param name="sql">sql语句模板</param>
        /// <param name="param">sql语句的参数</param>
        /// <returns></returns>
        public DBResult GetDataTable(int currPage, int pageSize, string pageBy, bool isAsc, string sql, params object[] param)
        {
            // 不执行分页
            if (currPage == -1) return GetDataTable(sql, param);

            if (currPage <= 0)
                throw new ArgumentOutOfRangeException("页码必须是大于0的整数!");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("每页数量必须是大于0的整数!");

            DBResult result = new DBResult
            {
                IsAsc = isAsc,
                PageBy = pageBy
            };
            var conn = GetConnection();
            try
            {
                if (conn == null)
                {
                    _logger.Error(result.Message = "数据库连接失败..");
                    result.IsSuccessed = false;
                    return result;
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                using (var DataAdapter = new MySqlDataAdapter())
                {
                    string order = "";
                    if (string.IsNullOrEmpty(pageBy))
                        order = " order by 1 ";
                    else
                        order = $" order by {pageBy} {(isAsc ? "asc" : "desc")},1 ";
                    sql = $@"select SQL_CALC_FOUND_ROWS * from ({sql}) tmpSql
                             {order}
                             limit {(currPage - 1) * pageSize},{pageSize};
                            select FOUND_ROWS() as totalRecords;";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                        InitParameters(cmd, param);
                        cmd.CommandType = CommandType.Text;
                        DataAdapter.SelectCommand = cmd;
                        var ds = new DataSet();
                        DataAdapter.Fill(ds);
                        result.Table = ds.Tables[0];
                        result.IsSuccessed = true;
                        result.PageSize = pageSize;
                        result.TotalRecords = Convert.ToInt64(ds.Tables[1].Rows[0]["totalRecords"]);
                        result.CurrentPage = currPage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }
            finally
            {
                CloseConnection(conn);
            }

            return result;
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="conn">数据库连接对象</param>
        /// <param name="cmd">SqlCommand对象</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <returns></returns>
        public DBResult GetDataSet(MySqlConnection conn, MySqlCommand cmd, string sql, params object[] param)
        {
            DBResult result = new DBResult();
            try
            {
                using (var DataAdapter = new MySqlDataAdapter())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = MySqlDBOptions.CommandTimeout;
                    InitParameters(cmd, param);
                    DataAdapter.SelectCommand = cmd;
                    var ds = new DataSet();
                    DataAdapter.Fill(ds);
                    result.DataSet = ds;
                    if (ds.Tables.Count > 0)
                        result.Table = ds.Tables[0];
                    result.IsSuccessed = true;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccessed = false;
                result.Message = ex.Message;
                _logger.Error(ex, ex.Message + LogHelper.CallingHistory());
            }

            return result;
        }

        /// <summary>
        /// 初始化SqlCommand的参数
        /// </summary>
        /// <param name="cmd">SqlCommand对象</param>
        /// <param name="param">参数数组</param>
        public static void InitParameters(MySqlCommand cmd, object[] param)
        {
            if (param == null || param.Length == 0)
                return;
            cmd.Parameters.Clear();
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] is KeyValuePair<string, object>)
                {
                    var p = (KeyValuePair<string, object>)param[i];
                    cmd.Parameters.AddWithValue($"@{p.Key.TrimStart('@')}", p.Value);
                }
                else
                    cmd.Parameters.AddWithValue($"@{i + 1}", param[i]);
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
                if (connectionPool != null) connectionPool.Dispose();
            }
        }

        #endregion //数据库访问
    }
}