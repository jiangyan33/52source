using System.Collections.Generic;
using System.Linq;

namespace CommonTool.Utils
{
    /// <summary>
    /// 字符串模板 仅支持简单的替换操作
    /// </summary>
    public class StringTemplate
    {
        private string TempString { get; set; }

        public StringTemplate(string tempString)
        {
            TempString = tempString;
        }

        public StringTemplate SetAttribute(string key, object value)
        {
            if (value.GetType() == typeof(string))
            {
                // 字符串执行简单的替换
                TempString.Replace($"${key}$", value.ToString());
            }
            else
            {
                // 简单的对象处理;过滤出非空的数据
                var props = value.GetType().GetProperties().ToList().FindAll(it => it.GetValue(value, null) != null);
                // 替换实体类中所有的字符串属性
                foreach (var it in props)
                {
                    if (it.PropertyType == typeof(string))
                    {
                        // 字符串属性直接替换
                        TempString = TempString.Replace($"${key}.{it.Name}$", it.GetValue(value, null).ToString());
                    }
                    else if (it.PropertyType == typeof(List<string>))
                    {
                        // 这里只处理List<string>集合这一种情况，如有需要可以自行扩展
                        var list = it.GetValue(value, null) as List<string>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            TempString = TempString.Replace($"${key}.{it.Name}[{i}]$", list[i]);
                        }
                    }
                }
            }
            return this;
        }

        public override string ToString()
        {
            return TempString;
        }
    }
}