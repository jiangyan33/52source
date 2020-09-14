using NLog;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LogUtil
{
    public class LogHelper
    {
        /// <summary>
        /// 日期对象
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                return LogManager.GetLogger(GetCurrentMethodFullName());
            }
        }

        /// <summary>
        /// 获取当前执行日志的方法名称(包括命名空间，类名，方法名)
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentMethodFullName()
        {
            StackFrame frame;
            string str;
            string str1;
            bool flag;
            try
            {
                int num = 2;
                StackTrace stackTrace = new StackTrace();
                int length = stackTrace.GetFrames().Length;
                do
                {
                    int num1 = num;
                    num = num1 + 1;
                    frame = stackTrace.GetFrame(num1);
                    str = frame.GetMethod().DeclaringType.ToString();
                    flag = (str.EndsWith("Exception") && num < length);
                }
                while (flag);
                string name = frame.GetMethod().Name;
                str1 = string.Concat(str, ".", name);
            }
            catch
            {
                str1 = null;
            }

            return str1;
        }

        /// <summary>
        /// 加载配置信息，不需要主动调用，日志框架会自动读取项目启动的根目录下的NLog.config文件
        /// </summary>
        /// <param name="path"></param>
        public static void LoadNLogConfig(string path)
        {
            try
            {
                LogManager.LoadConfiguration(path);
            }
            catch
            {
            }
        }

        public static string CallingHistory()
        {
            var ret = new StackTrace(true).GetFrames()?.ToList();
            if (ret == null || ret.Count == 0) return "";

            // 移除当前方法的帧
            ret.RemoveAt(0);
            ret.Reverse();
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("调用历史");
            foreach (var item in ret)
            {
                sb.AppendLine($"调用方法: {item.GetMethod().DeclaringType}.{item.GetMethod().Name}, 行号: {item.GetFileLineNumber()}, 列号: {item.GetFileColumnNumber()}");
            }
            return sb.ToString();
        }
    }
}