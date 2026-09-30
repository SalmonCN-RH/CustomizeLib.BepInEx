using CustomizeLib.BepInEx.Internal.Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx
{
    #region TypeKeyDictionary<TValue>
    /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}"/>
    public class TypeKeyDictionary<TValue>
    {
        private readonly Internal.Auxiliary.TypeKeyDictionary<TValue> Dictionary = new();

        public TypeKeyDictionary() => Dictionary = new();

        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.Count"/>
        public int Count => Dictionary.Count;


        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.Add(object, TValue)"/>
        public void Add(object key, TValue value) => Dictionary.Add(key, value);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.Remove(object)"/>
        public bool Remove(object key) => Dictionary.Remove(key);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.SetValue(object, TValue)"/>
        public void SetValue(object key, TValue value) => Dictionary.SetValue(key, value);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.GetValue(object)"/>
        public TValue GetValue(object key) => Dictionary.GetValue(key);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.GetValueOrDefault(object, TValue)"/>
        public TValue GetValueOrDefault(object key, TValue defaultValue) => Dictionary.GetValueOrDefault(key, defaultValue);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.ContainsKey(object)"/>
        public bool ContainsKey(object key) => Dictionary.ContainsKey(key);
        /// <inheritdoc cref="Internal.Auxiliary.TypeKeyDictionary{TValue}.ToDictionary"/>
        public Dictionary<object, TValue> ToDictionary() => Dictionary.ToDictionary();
    }
    #endregion
}
