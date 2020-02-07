using System;
using System.Collections.Generic;

namespace GData
{
    public class Index
    {
        private Dictionary<Tuple<Type, string>, object> _index = new Dictionary<Tuple<Type, string>, object>(100);

        public bool HasKey(Tuple<Type, string> key)
        {
            return _index.ContainsKey(key);
        }

        public dynamic GetValue(Tuple<Type, string> key)
        {
            dynamic value;
            if (!_index.TryGetValue(key, out value)) {
                return null;
            }

            return value;
        }

        public void Add(Tuple<Type, string> key, dynamic value)
        {
            _index.Add(key, value);
        }
    }
}