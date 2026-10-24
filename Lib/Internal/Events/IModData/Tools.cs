using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IModData
    {
        /// <summary>
        /// 获取 <paramref name="type"/> 的所有子类
        /// </summary>
        /// <param name="type">基类</param>
        /// <returns>所有子类</returns>
        internal static Type[] GetAll(Type type) => 
            Utils.TypeUtils.GetAllDerivedTypes(Utils.TypeUtils.GetAllAssemblies(), type, false);

        /// <summary>
        /// 获取类型 <paramref name="type"/> 中名为 <paramref name="name"/> 的属性的值
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="name">属性名</param>
        /// <returns>值</returns>
        /// <exception cref="ArgumentException"></exception>
        internal static T GetData<T>(Type type, string name)
        {
            var obj = CreateInstance(type);

            var prop = obj.GetType().GetProperty(name, Utils.TypeUtils.DefaultDeclaredOnly);
            if (prop == null) throw new ArgumentException($"Can't find a property named {name} in type {type}");
            return (T)prop.GetValue(obj)!;
        }

        private static object CreateInstance(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (type == typeof(string)) return string.Empty;

            if (type.IsArray)
            {
                var elementType = type.GetElementType()!;
                return Array.CreateInstance(elementType, new int[type.GetArrayRank()]);
            }

            if (type.IsValueType)
                return Activator.CreateInstance(type)!;

            if (type.IsAbstract || type.IsInterface || typeof(Delegate).IsAssignableFrom(type))
                throw new InvalidOperationException($"Can't create an instance of type {type}");

            return RuntimeHelpers.GetUninitializedObject(type);
        }
    }
}
