using System;

namespace CommonEntity.Business
{
    public class Content : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 章节名称
        /// </summary>
        public string Chapter { get; set; }

        /// <summary>
        /// 章节内容
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 同一本图书排序
        /// </summary>
        public int Sort { get; set; }

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
        /// 创建时间
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 校正次数
        /// </summary>
        public int CheckCount { get; set; }

        /// <summary>
        /// 存储内容
        /// </summary>
        public string Remark { get; set; }
    }
}