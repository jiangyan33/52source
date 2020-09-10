using System;

namespace CommonEntity.MySql
{
    /// <summary>
    /// 分页结果集扩展
    /// </summary>
    public class PageResult
    {
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _pageSize = 10;
        private long _totalRecords = 0;

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value < 1)
                    _currentPage = 1;
                else if (value > _totalPages)
                    _currentPage = _totalPages;
                else
                    _currentPage = value;
            }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (_totalPages == 0)
                    return 1;
                else
                    return _totalPages;
            }
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize <= 0)
                    _pageSize = 10;
                else
                    _pageSize = value;
                _totalPages = (int)Math.Ceiling(_totalRecords / (double)PageSize);
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalRecords
        {
            get { return _totalRecords; }
            set
            {
                _totalRecords = value;
                _totalPages = (int)Math.Ceiling(_totalRecords / (double)PageSize);
            }
        }

        /// <summary>
        /// 分页排序的自定义字段
        /// </summary>
        public string PageBy { get; set; }

        /// <summary>
        /// 是否时升序
        /// </summary>
        public bool IsAsc { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PageResult() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalRecords">总记录数</param>
        public PageResult(int pageSize, long totalRecords)
        {
            CurrentPage = 1;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            _totalPages = (int)Math.Ceiling(_totalRecords / (double)PageSize);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currPage">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalRecords">总记录数</param>
        public PageResult(int currPage, int pageSize, long totalRecords) :
            this(pageSize, totalRecords)
        {
            CurrentPage = currPage;
        }
    }
}