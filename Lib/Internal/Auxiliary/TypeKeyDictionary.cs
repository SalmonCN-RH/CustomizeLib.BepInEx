using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Auxiliary
{
    internal class TypeKeyDictionary<TValue>
    {
        private static T ThrowIfNull<T>(object obj, Func<T> callback)
        {
            ArgumentNullException.ThrowIfNull(obj, "key");
            return callback.Invoke();
        }
        private static void ThrowIfNull(object obj, Action callback)
        {
            ArgumentNullException.ThrowIfNull(obj, "key");
            callback.Invoke();
        }

        private readonly Dictionary<Type, Bucket> Buckets = [];

        public int Count => Buckets.Sum(kvp => kvp.Value.Count);

        public void Add(object key, TValue value) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).Add(key, value));
        public bool Remove(object key) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).Remove(key));
        public void SetValue(object key, TValue value) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).SetValue(key, value));
        public TValue GetValue(object key) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).GetValue(key));
        public TValue GetValueOrDefault(object key, TValue defaultValue) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).GetValueOrDefault(key, defaultValue));
        public bool ContainsKey(object key) => ThrowIfNull(key, () => GetOrCreateBucket(key.GetType()).ContainsKey(key));

        public Dictionary<object, TValue> ToDictionary()
        {
            var res = new Dictionary<object, TValue>();
            foreach (var (_, bucket) in Buckets)
                foreach (var (key, value) in bucket.ToDictionary())
                    res.Add(key, value);
            return res;
        }

        private Bucket GetOrCreateBucket(Type type)
        {
            if (Buckets.TryGetValue(type, out var bucket))
                return bucket;

            var bucketType = typeof(Bucket<>).MakeGenericType(typeof(TValue), type); // 嵌套类外层的泛型参数也需要传参
            bucket = (Bucket)Activator.CreateInstance(bucketType)!;
            Buckets.Add(type, bucket);
            return bucket;
        }

        private abstract class Bucket
        {
            public abstract int Count { get; }

            public abstract void Add(object key, TValue value);
            public abstract bool Remove(object key);
            public abstract void SetValue(object key, TValue value);
            public abstract TValue GetValue(object key);
            public abstract TValue GetValueOrDefault(object key, TValue defaultValue);
            public abstract bool ContainsKey(object key);
            public abstract Dictionary<object, TValue> ToDictionary();
        }

        private class Bucket<TKey> : Bucket where TKey : notnull
        {
            private readonly Dictionary<TKey, TValue> Maps = new(new KeyComparer());

            public override int Count => Maps.Count;

            public override void Add(object key, TValue value) => Maps.Add((TKey)key, value);
            public override bool Remove(object key) => Maps.Remove((TKey)key);
            public override void SetValue(object key, TValue value) => Maps[(TKey)key] = value;
            public override TValue GetValue(object key) => Maps[(TKey)key];
            public override TValue GetValueOrDefault(object key, TValue defaultValue) => Maps.GetValueOrDefault((TKey)key, defaultValue);
            public override bool ContainsKey(object key) => Maps.ContainsKey((TKey)key);

            public override Dictionary<object, TValue> ToDictionary() => Maps.ToDictionary(kvp => (object)kvp.Key, kvp => kvp.Value);

            private sealed class KeyComparer : IEqualityComparer<TKey>
            {
                private readonly MethodInfo EqualsFunc = null!;
                private readonly MethodInfo GetHashCodeFunc = null!;

                internal KeyComparer()
                {
                    EqualsFunc = typeof(TKey).GetMethod(nameof(object.Equals), InternalTools.TypeTools_DefaultFlag.RemoveStatic(), [typeof(TKey)])!;
                    GetHashCodeFunc = typeof(TKey).GetMethod(nameof(object.GetHashCode), InternalTools.TypeTools_DefaultFlag.RemoveStatic(), [])!;
                }

                public bool Equals(TKey? x, TKey? y) => (bool)EqualsFunc.Invoke(x, [y])!;
                public int GetHashCode([DisallowNull] TKey obj) => (int)GetHashCodeFunc.Invoke(obj, null)!;
            }
        }
    }
}
