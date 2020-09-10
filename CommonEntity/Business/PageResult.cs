using System.Collections.Generic;

namespace CommonEntity.Business
{
    public class PageResult<T>
    {
        public int Pages { get; set; }

        public long Rows { get; set; }

        public List<T> Data { get; set; }
    }
}