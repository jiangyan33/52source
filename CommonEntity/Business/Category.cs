using System;

namespace CommonEntity.Business
{
    /// <summary>
    /// 图书分类
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 拼音
        /// </summary>
        public string Pinyin { get; set; }

        /// <summary>
        /// 是否启用 1:启用，2禁用
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 备注，预留字段
        /// </summary>
        public string Remark { get; set; }
    }
}