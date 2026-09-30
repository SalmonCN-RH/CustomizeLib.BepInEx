using CustomizeLib.BepInEx.Internal;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IModTask
    {
        #region 缓存数据
        internal static Dictionary<Type, object> CachedDatas = [];
        #endregion

        public const int Low = 0;
        public const int Default = 400;
        public const int High = 800;

        protected static IEnumerable<T> GetAllTask<T>() where T : IModTask
        {
            return InternalTools.TypeTools_GetAllDerivedTypes<T>(InternalTools.TypeTools_GetAllAssemblies(), false).Select(t =>
            {
                if (t.IsGenericType) return default!;
                if (t.IsInterface) return default!;
                return (T)Activator.CreateInstance(t)!;
            }).Where(task => task != null).OrderBy(task => task.GetOrder());
        }

        protected static void DoAll<T>() where T : IModTask
        {
            foreach (var task in GetAllTask<T>()) task.Do();
        }
    }
}
