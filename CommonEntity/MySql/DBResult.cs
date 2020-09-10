using System;
using System.Data;

namespace CommonEntity.MySql
{
    /// <summary>
    /// 公共DBHelper返回结果
    /// </summary>
    public class DBResult : PageResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessed { get; set; }

        /// <summary>
        /// DataTable
        /// </summary>
        public DataTable Table { get; set; }

        /// <summary>
        /// DataSet
        /// </summary>
        public DataSet DataSet { get; set; }

        /// <summary>
        /// Object
        /// </summary>
        public Object DataObject { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 受影响行数
        /// </summary>
        public int AffectRowCount { get; set; } = 0;
    }
}