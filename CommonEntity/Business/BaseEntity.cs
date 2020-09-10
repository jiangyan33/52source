using CommonTool;
using System.Text.Json.Serialization;

namespace CommonEntity.Business
{
    public class BaseEntity
    {
        [JsonIgnore]
        public string Search { get; set; }

        [JsonIgnore]
        public int PageNum { get; set; } = 0;

        [JsonIgnore]
        public int PageSize { get; set; } = 0;

        [JsonIgnore]
        public bool IsAsc { get; set; } = true;

        [JsonIgnore]
        public string Sort
        {
            get
            {
                return _sort;
            }
            set
            {
                var temp = value.ChangeAttrToLineName();
                _sort = temp;
            }
        }

        private string _sort = "create_date";
    }
}