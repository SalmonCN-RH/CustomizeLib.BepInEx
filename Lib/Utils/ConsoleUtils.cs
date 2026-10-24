using System.Text;
using UnityEngine;

namespace CustomizeLib.BepInEx
{
    public static partial class Utils
    {
        public static class ConsoleUtils
        {
            /// <summary>
            /// 设置控制台输出编码
            /// </summary>
            /// <param name="encoding">编码</param>
            internal static void SetOutputEncoding(Encoding encoding)
            {
                if (Application.platform != RuntimePlatform.Android)
                    Console.OutputEncoding = encoding;
            }

            /// <summary>
            /// 设置控制台输出编码 (UTF8)
            /// </summary>
            internal static void SetOutputEncoding() => SetOutputEncoding(Encoding.UTF8);
        }
    }
}
