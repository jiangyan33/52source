using System;

namespace CommonEntity.Business
{
    public class Video : BaseEntity
    {
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Pic { get; set; }

        /// <summary>
        /// 统计
        /// </summary>
        public int Hot { get; set; }

        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateBy { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateBy { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int Enable { get; set; }
    }
}