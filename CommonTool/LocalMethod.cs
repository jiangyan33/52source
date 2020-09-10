using LogUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonTool
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class LocalMethod
    {
        #region validation

        /// <summary>
        /// 返回一个字符串是否是double类型
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static bool IsDouble(this string src)
        {
            return double.TryParse(src, out _);
        }

        /// <summary>
        /// 是否是数值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumber(this string str)
        {
            return int.TryParse(str, out _) || double.TryParse(str, out _);
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(this string str)
        {
            return int.TryParse(str, out _);
        }

        /// <summary>
        /// 是否是小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDoubleRegex(this string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsIntRegex(this string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        #endregion validation

        #region util

        /// <summary>
        /// 将DataTable序列化成对象，如果DataTable中和对象中存在相同的字段名，就会赋值，否则不会赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="ignoreTableUnderline">是否忽略数据表中的下划线</param>
        /// <returns></returns>
        public static List<T> SerializeToObject<T>(this System.Data.DataTable dataTable, bool ignoreTableUnderline = true) where T : new()
        {
            if (dataTable == null)
                return null;

            var type = typeof(T);
            var props = type.GetProperties();
            // 如果数据表字段有下滑线会去掉
            var columns = dataTable.Columns.Cast<System.Data.DataColumn>().Select(c =>
                new
                {
                    ProptyName = ignoreTableUnderline ?
                        c.ColumnName.Replace("_", "").ToLower() :
                        c.ColumnName.ToLower(),
                    c.ColumnName
                }
            );
            if (columns.Count() == 0)
                return new List<T>();

            if (dataTable == null || dataTable.Rows.Count == 0)
                return new List<T>();

            var getDefaultValue = new Func<Type, object>(propType =>
            {
                // 如果是可空类型，直接返回null
                if (Nullable.GetUnderlyingType(propType) != null)
                    return null;
                // 如果时非可空类型 直接获取默认值
                else
                {
                    // 如果不是值类型，直接返回null
                    return propType.IsValueType ? Activator.CreateInstance(propType) : null;
                }
            });

            var convertTypeToProperty = new Func<Type, object, object>((prop, value) =>
            {
                try
                {
                    // 当datatable中字段的值为空时，需要根据实体类的属性类型来初始化默认值
                    if (value == null || value == DBNull.Value)
                    {
                        return getDefaultValue(prop);
                    }
                    // 当datatable中字段的值不为空时，将其转换为属性类型，如果失败，则抛出异常。
                    else
                    {
                        var t = Nullable.GetUnderlyingType(prop) ?? prop;
                        return Convert.ChangeType(value, t);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });

            var ret = new List<T>();
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                var obj = new T();
                foreach (var prop in props)
                {
                    var c = columns.Where(x => x.ProptyName.ToLower() == prop.Name.ToLower()).FirstOrDefault();
                    // 如果对象属性在DataTable中存在相应的列，就赋值
                    if (c != null)
                    {
                        try
                        {
                            // 类型相同直接赋值
                            if (row[c.ColumnName].GetType().FullName == prop.PropertyType.FullName)
                                prop.SetValue(obj, row[c.ColumnName], null);
                            // 类型不同，将table的类型转换为属性的类型，转换失败时赋值为当前类型的默认值
                            else
                                prop.SetValue(obj, convertTypeToProperty(prop.PropertyType, row[c.ColumnName]), null);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                ret.Add(obj);
            }

            return ret;
        }

        /// <summary>
        /// 动态给对象中的属性赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">对象实例</param>
        /// <param name="arrtName">属性名称</param>
        /// <param name="value">属性值，会根据属性值的属性去执行转换</param>
        /// <returns></returns>
        public static T DynamicAssignmentAttr<T>(T instance, string arrtName, object value)
        {
            var props = instance.GetType().GetProperties();
            int pos = props.ToList().FindIndex(it => it.Name == arrtName);
            switch (props[pos].PropertyType.FullName)
            {
                // double类型
                case "System.Double":
                    props[pos].SetValue(instance, Convert.ToDouble(value), null);
                    break;
                // int类型
                case "System.Int32":
                    props[pos].SetValue(instance, Convert.ToInt32(value), null);
                    break;
                // 字符串
                default:
                    props[pos].SetValue(instance, value.ToString(), null);
                    break;
            }
            return instance;
        }

        /// <summary>
        /// 动态获取实体类中的属性值，忽略大小写
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">实体类对象</param>
        /// <param name="AttrName">属性名称</param>
        /// <returns></returns>
        public static object DynamicGetAttrValue<T>(T instance, string AttrName)
        {
            var prop = instance.GetType().GetProperties().ToList().Find(it => it.Name.ToLower() == AttrName.ToLower());
            if (prop == null) return null;
            return prop.GetValue(instance, null);
        }

        public static string ReadFile(string path)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾
                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line).Append("\n");
                }
            }
            return sb.ToString();
        }

        public static string HttpUpload(string url, string data)
        {
            try
            {
                WebClient wc = new WebClient();
                var res = wc.UploadFile(url, data);
                return Encoding.UTF8.GetString(res);
            }
            catch (Exception e)
            {
                LogHelper.Logger.Error($"http请求异常:" + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 从服务器中获取资源
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            try
            {
                WebClient client = new WebClient
                {
                    Encoding = Encoding.GetEncoding("GB2312")
                };
                var res = client.DownloadData(url);
                return Encoding.UTF8.GetString(res);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 默认为utf8
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(string code = "utf8")
        {
            int codepage = 65001;

            switch (code.ToLower())
            {
                case "gbk": codepage = 936; break;
                default: break;
            }
            return Encoding.GetEncoding(codepage);
        }

        /// <summary>
        /// 下划线转大驼峰
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static string ChangeAttrName(string attrName)
        {
            if (attrName.Length <= 1) return attrName;
            StringBuilder sb = new StringBuilder();
            var strArr = attrName.Split('_');
            for (int i = 0; i < strArr.Length; i++)
            {
                sb.Append(strArr[i].ToUpper()[0]).Append(strArr[i].ToLower().Substring(1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 通过反射深克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T instance) where T : new()
        {
            var res = new T();

            var props = instance.GetType().GetProperties();
            foreach (var item in props)
            {
                item.SetValue(res, item.GetValue(instance, null), null);
            }
            return res;
        }

        /// <summary>
        /// 移除字符串结束处的换行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static string RemoveLastBreak(this string line)
        {
            if (line.EndsWith("\n"))
            {
                var index = line.LastIndexOf('\n');
                line = line.Substring(0, index);
            }
            return line;
        }

        /// <summary>
        /// 将驼峰格式调整为下划线
        /// </summary>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static string ChangeAttrToLineName(this string attrName)
        {
            if (attrName.Length <= 1) return attrName;
            StringBuilder sb = new StringBuilder();
            sb.Append(attrName.ToLower()[0]);
            for (int i = 1; i < attrName.Length; i++)
            {
                if ('A' <= attrName[i] && attrName[i] <= 'Z')
                {
                    sb.Append($"_{attrName[i]}".ToLower());
                }
                else
                {
                    sb.Append(attrName[i]);
                }
            }
            return sb.ToString();
        }

        #endregion util

        public static void RecordDate(int count)
        {
            Console.WriteLine(count);
        }
    }
}