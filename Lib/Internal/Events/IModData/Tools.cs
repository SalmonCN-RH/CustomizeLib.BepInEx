using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    public partial interface IModData
    {
        internal static Type[] GetAll(Type type) => 
            InternalTools.TypeTools_GetAllDerivedTypes(InternalTools.TypeTools_GetAllAssemblies(), type, false);

        internal static T GetData<T>(Type type, string name)
        {
            var prop = type.GetProperty(name, InternalTools.TypeTools_DefaultDeclaredOnly.RemoveInstance());
            if (prop != null && prop.CanRead)
                return (T)prop.GetValue(null)!;

            var field = type.GetField(name, InternalTools.TypeTools_DefaultDeclaredOnly.RemoveInstance());
            if (field != null)
                return (T)field.GetValue(null)!;

            throw new ArgumentException($"Can't find a field or property named {name} in type {type}");
        }
    }
}
