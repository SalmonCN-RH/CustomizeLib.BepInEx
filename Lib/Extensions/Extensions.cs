using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Extensions.EventExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Extensions
{
    /// <summary>
    /// 通用扩展方法类, 封装了所有扩展方法
    /// </summary>
    public static class Extensions
    {
        #region BindingFlagExtensions
        /// <inheritdoc cref="BindingFlagExtensions.AddFlags(BindingFlags, BindingFlags)"/>
        public static BindingFlags AddFlags(this BindingFlags flags, BindingFlags add) => BindingFlagExtensions.AddFlags(flags, add);

        /// <inheritdoc cref="BindingFlagExtensions.AddPublic(BindingFlags)"/>
        public static BindingFlags AddPublic(this BindingFlags flags) => BindingFlagExtensions.AddPublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddNonPublic(BindingFlags)"/>
        internal static BindingFlags AddNonPublic(this BindingFlags flags) => BindingFlagExtensions.AddNonPublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddStatic(BindingFlags)"/>
        internal static BindingFlags AddStatic(this BindingFlags flags) => BindingFlagExtensions.AddStatic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddInstance(BindingFlags)"/>
        internal static BindingFlags AddInstance(this BindingFlags flags) => BindingFlagExtensions.AddInstance(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddDeclaredOnly(BindingFlags)"/>
        internal static BindingFlags AddDeclaredOnly(this BindingFlags flags) => BindingFlagExtensions.AddDeclaredOnly(flags);

        /// <inheritdoc cref="BindingFlagExtensions.RemoveFlags(BindingFlags, BindingFlags)"/>
        public static BindingFlags RemoveFlags(this BindingFlags flags, BindingFlags Remove) => BindingFlagExtensions.RemoveFlags(flags, Remove);

        /// <inheritdoc cref="BindingFlagExtensions.RemovePublic(BindingFlags)"/>
        public static BindingFlags RemovePublic(this BindingFlags flags) => BindingFlagExtensions.RemovePublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.RemoveNonPublic(BindingFlags)"/>
        internal static BindingFlags RemoveNonPublic(this BindingFlags flags) => BindingFlagExtensions.RemoveNonPublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.RemoveStatic(BindingFlags)"/>
        internal static BindingFlags RemoveStatic(this BindingFlags flags) => BindingFlagExtensions.RemoveStatic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.RemoveInstance(BindingFlags)"/>
        internal static BindingFlags RemoveInstance(this BindingFlags flags) => BindingFlagExtensions.RemoveInstance(flags);

        /// <inheritdoc cref="BindingFlagExtensions.RemoveDeclaredOnly(BindingFlags)"/>
        internal static BindingFlags RemoveDeclaredOnly(this BindingFlags flags) => BindingFlagExtensions.RemoveDeclaredOnly(flags);
        #endregion

        #region AssetBundleExtensions
        /// <inheritdoc cref="AssetBundleExtensions.TryGetAsset{T}(AssetBundle, string, out T?)"/>
        internal static bool TryGetAsset<T>(this AssetBundle ab, string name, out T? obj) where T : UnityEngine.Object =>
            AssetBundleExtensions.TryGetAsset(ab, name, out obj);

        /// <inheritdoc cref="AssetBundleExtensions.GetAsset{T}(AssetBundle, string)"/>
        internal static T GetAsset<T>(this AssetBundle ab, string name) where T : UnityEngine.Object =>
            AssetBundleExtensions.GetAsset<T>(ab, name);

        /// <inheritdoc cref="AssetBundleExtensions.GetAllAssets(AssetBundle)"/>
        internal static UnityEngine.Object[] GetAllAssets(this AssetBundle ab) => AssetBundleExtensions.GetAllAssets(ab);
        #endregion

        #region IntegerExtensions
        /// <inheritdoc cref="IntegerExtensions.ToEnum{T}(long)"/>
        public static T ToEnum<T>(this long value) where T : Enum => IntegerExtensions.ToEnum<T>(value);

        /// <inheritdoc cref="IntegerExtensions.ToEnum{T}(int)"/>
        public static T ToEnum<T>(this int value) where T : Enum => IntegerExtensions.ToEnum<T>(value);
        #endregion

        #region DictionaryExtensions
        /// <inheritdoc cref="Internal.Extensions.DictionaryExtensions.AddAndLogIfDup{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, string)"/>
        public static void AddAndLogIfDup<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, string message) =>
            Internal.Extensions.DictionaryExtensions.AddAndLogIfDup(dictionary, key, value, message);
        #endregion

        #region ObjectExtensions
        /// <inheritdoc cref="ObjectExtensions.ToIEnumerable{T}(T)"/>
        public static IEnumerable<T> ToIEnumerable<T>(this T item) =>
            ObjectExtensions.ToIEnumerable(item);

        /// <inheritdoc cref="ObjectExtensions.To{T}(object)"/>
        internal static T To<T>(this object obj) =>
            ObjectExtensions.To<T>(obj);

        /// <inheritdoc cref="ObjectExtensions.IsSubTypeOf(object, Type, bool)"/>
        internal static bool IsSubTypeOf(this object obj, Type type, bool containBase = true) =>
            ObjectExtensions.IsSubTypeOf(obj, type, containBase);

        /// <inheritdoc cref="ObjectExtensions.IsSubTypeOf{TBase}(object, bool)"/>
        internal static bool IsSubTypeOf<TBase>(this object obj, bool containBase = true) =>
            ObjectExtensions.IsSubTypeOf<TBase>(obj, containBase);
        #endregion

        #region IPlantEventExtensions
        /// <inheritdoc cref="IPlantEventExtensions.Register"/>
        internal static void Register(this IPlantEvent plantEvent) => IPlantEventExtensions.Register(plantEvent);

        /// <inheritdoc cref="IPlantEventExtensions.Unregister"/>
        internal static void Unregister(this IPlantEvent plantEvent) => IPlantEventExtensions.Unregister(plantEvent);

        /// <inheritdoc cref="IPlantEventExtensions.TryGetBindEvents(object, out List{IPlantEvent})"/>
        internal static bool TryGetBindEvents(this object obj, out List<IPlantEvent> plantEvents) =>
            IPlantEventExtensions.TryGetBindEvents(obj, out plantEvents);

        /// <inheritdoc cref="IPlantEventExtensions.Bind(object)"/>
        internal static void Bind(this IPlantEvent plantEvent, object obj) =>
            IPlantEventExtensions.Bind(plantEvent, obj);
        #endregion

        #region TypeExtensions
        /// <inheritdoc cref="Internal.Extensions.TypeExtensions.Il2CppName(Type, bool)"/>
        public static string Il2CppName(this Type type, bool addr = false) =>
            Internal.Extensions.TypeExtensions.Il2CppName(type, addr);

        /// <inheritdoc cref="Internal.Extensions.TypeExtensions.Il2CppType(Type)"/>
        internal static Il2CppSystem.Type Il2CppType(this Type type) =>
            Internal.Extensions.TypeExtensions.Il2CppType(type);
        #endregion

        #region IApplyDetourExtensions
        /// <inheritdoc cref="IApplyDetourExtensions.GetOriginals{TDelegate}(IDetourHook{TDelegate})"/>
        public static TDelegate[] GetOriginals<TDelegate>(this IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            IApplyDetourExtensions.GetOriginals(detour);

        /// <inheritdoc cref="IApplyDetourExtensions.GetDetours{TDelegate}(IDetourHook{TDelegate})"/>
        public static INativeDetour[] GetDetours<TDelegate>(IDetourHook<TDelegate> detour) where TDelegate : Delegate =>
            IApplyDetourExtensions.GetDetours(detour);
        #endregion

        #region IEnumerableExtensions
        /// <inheritdoc cref="IEnumerableExtensions.DoAll{T}(IEnumerable{T}, Action{T})"/>
        public static IEnumerable<T> DoAll<T>(this IEnumerable<T> enumerable, Action<T> action) =>
            IEnumerableExtensions.DoAll(enumerable, action);
        #endregion
    }
}
