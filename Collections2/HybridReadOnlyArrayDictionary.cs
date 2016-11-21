using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections2
{
    public class HybridReadOnlyArrayDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private const int MaxArrayItems = 6;

        private readonly TKey[] _keys;
        private readonly TValue[] _values;
        private readonly int _arrayCount;

        private readonly Dictionary<TKey, TValue> _dictionary;

        public HybridReadOnlyArrayDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            _keys = new TKey[MaxArrayItems];
            _values = new TValue[MaxArrayItems];

            int count = 0;
            foreach (var pair in pairs)
            {
                if (count < MaxArrayItems)
                {
                    _keys[count] = pair.Key;
                    _values[count] = pair.Value;
                }
                else if (count == MaxArrayItems)
                {
                    _dictionary = new Dictionary<TKey, TValue>(MaxArrayItems * 2);
                    for (int i = 0; i < MaxArrayItems; i++)
                    {
                        _dictionary.Add(_keys[i], _values[i]);
                    }

                    _keys = null;
                    _values = null;

                    _dictionary.Add(pair.Key, pair.Value);
                }
                else if (count > MaxArrayItems)
                {
                    _dictionary.Add(pair.Key, pair.Value);
                }

                count++;
            }

            _arrayCount = _keys == null ? -1 : count;
            if (_arrayCount != -1 && _arrayCount < MaxArrayItems - 1)
            {
                var tKeys = new TKey[_arrayCount];
                Array.Copy(_keys, tKeys, _arrayCount);
                _keys = tKeys;

                var tValues = new TValue[_arrayCount];
                Array.Copy(_values, tValues, _arrayCount);
                _values = tValues;
            }
        }

        private bool IsArrayUsed
        {
            get { return _keys != null; }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                if (IsArrayUsed)
                {
                    return _keys;
                }

                return _dictionary.Keys;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                if (IsArrayUsed)
                {
                    return _values;
                }

                return _dictionary.Values;
            }
        }

        public int Count
        {
            get
            {
                if (IsArrayUsed)
                {
                    return _arrayCount;
                }

                return _dictionary.Count;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (IsArrayUsed)
                {
                    for (int i = 0; i < _arrayCount; i++)
                    {
                        if (key.Equals(_keys[i]))
                        {
                            return _values[i];
                        }
                    }

                    throw new KeyNotFoundException("Key not found: " + key);
                }

                return _dictionary[key];
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (IsArrayUsed)
            {
                for (int i = 0; i < _arrayCount; i++)
                {
                    if (key.Equals(_keys[i]))
                    {
                        return true;
                    }
                }

                return false;
            }

            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (IsArrayUsed)
            {
                for (int i = 0; i < _arrayCount; i++)
                {
                    if (key.Equals(_keys[i]))
                    {
                        value = _values[i];
                        return true;
                    }
                }

                value = default(TValue);
                return false;
            }

            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (IsArrayUsed)
            {
                return GetArrayEnumerator();
            }

            return _dictionary.GetEnumerator();
        }

        private IEnumerator<KeyValuePair<TKey, TValue>> GetArrayEnumerator()
        {
            for (int i = 0; i < _arrayCount; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Console.WriteLine("GetEnumerator");
            return GetEnumerator();
        }
    }
}
