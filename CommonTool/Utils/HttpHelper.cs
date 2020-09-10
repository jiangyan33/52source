using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace CommonTool.Utils
{
    public static class HttpHelper
    {
        /// <summary>
        ///
        /// 发送简单的http get请求
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <returns></returns>
        public static string Get(string url, string charset = "UTF-8", string cookies = null, int Timeout = 10000)
        {
            try
            {
                var enc = LocalMethod.GetEncoding(charset);
                // 获取目录页
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = $"text/html;charset={charset}";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
                request.Timeout = Timeout;
                if (!string.IsNullOrEmpty(cookies))
                {
                    var cookieContainer = new CookieContainer();
                    var cookieArr = cookies.Split(';');
                    foreach (var item in cookieArr)
                    {
                        var keyValue = item.Trim().Split('=');
                        cookieContainer.Add(new Cookie(keyValue[0], keyValue[1]));
                    }

                    request.CookieContainer = cookieContainer;
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader reader = null;
                if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress), enc);
                else
                    reader = new StreamReader(response.GetResponseStream(), enc);
                string html = reader.ReadToEnd();
                return html;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return "";
            }
        }

        /// 创建POST方式的HTTP请求
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, int timeout = 10000)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
            request.Timeout = timeout;
            //发送POST数据
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// post请求接口可以携带文件，和参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileKeyName"></param>
        /// <param name="stream"></param>
        /// <param name="stringDict"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static string HttpPostData(string url, string fileKeyName, string fileName, byte[] bytes, NameValueCollection stringDict, int timeOut = 10000)
        {
            var (Body, Boudary) = ToFormDataWithStream(fileKeyName, bytes, stringDict, fileName);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + Boudary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Timeout = timeOut;
            request.Credentials = CredentialCache.DefaultCredentials;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(Body, 0, Body.Length);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogHelper.Logger.Error(ex.Message);
                return null;
            }
        }

        public static (byte[] Body, string Boudary) ToFormDataWithStream(string name, byte[] value, NameValueCollection nameValueCollection, string fileName = "", string contentType = "application/octet-stream")
        {
            var boundary = $"----WebkitBoudary{DateTime.Now.Ticks:x}";
            var encoding = Encoding.UTF8;

            var formDataStream = new MemoryStream();
            // 发送每个字段:
            if (nameValueCollection != null)
            {
                foreach (string item in nameValueCollection.Keys)
                {
                    string collectionData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                       boundary,
                       item,
                       nameValueCollection[item]);
                    formDataStream.Write(encoding.GetBytes(collectionData), 0, encoding.GetByteCount(collectionData));
                }
            }

            // 发送文件:
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
               boundary,
               name,
               string.IsNullOrEmpty(fileName) ? name : fileName,
               contentType);

            formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
            // Write the file data directly to the Stream, rather than serializing it to a string.
            formDataStream.Write(value, 0, value.Length);

            string footer = $"\r\n--{boundary}--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return (formData, boundary);
        }

        /// <summary>
        /// 将文件转换成byte[] 数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]</returns>
        public static byte[] GetFileData(string fileUrl)
        {
            FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                return buffur;
            }
            catch (Exception ex)
            {
                LogUtil.LogHelper.Logger.Error(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    //关闭资源
                    fs.Close();
                }
            }
        }
    }
}