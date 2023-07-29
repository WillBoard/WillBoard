using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WillBoard.Core.Classes
{
    public class MemoryTempDataDictionary : ITempDataDictionary
    {
        private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

        public object this[string key]
        {
            get => _data.TryGetValue(key, out object value) ? value : null;
            set => _data[key] = value;
        }

        public ICollection<string> Keys => _data.Keys;

        public ICollection<object> Values => _data.Values;

        public int Count => _data.Count;

        public bool IsReadOnly => _data.IsReadOnly;

        public void Add(string key, object value)
        {
            _data.Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _data.Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _data.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public void Keep()
        {
        }

        public void Keep(string key)
        {
        }

        public void Load()
        {
        }

        public object Peek(string key)
        {
            if (_data.TryGetValue(key, out object value))
            {
                return value;
            }

            return null;
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _data.Remove(item);
        }

        public void Save()
        {
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}