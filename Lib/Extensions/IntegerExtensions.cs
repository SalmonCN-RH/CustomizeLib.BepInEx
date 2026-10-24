using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public static partial class Extensions
    {
        /// <summary>
        /// 将 <paramref name="value"/> 转换为枚举类型 <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>枚举值</returns>
        public static T ToEnum<T>(this long value) where T : Enum =>
            (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// 将 <paramref name="value"/> 转换为枚举类型 <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>枚举值</returns>
        public static T ToEnum<T>(this int value) where T : Enum => ToEnum<T>((long)value);
    }
}
