using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GData.Converter
{
    public class DefaultConverter
    {
        private readonly Index _index;
        private readonly char _listSeparator;

        public DefaultConverter(Index index, char listSeparator)
        {
            _index = index;
            _listSeparator = listSeparator;
        }

        public dynamic Convert(string value, Type type)
        {
            if (isEnumerableType(type) && value != null) {
                var childType = type.GetElementType();
                var values = value.Split(_listSeparator).Select(x => Convert(x.Trim(), childType)).ToArray();
                return Convert(values, type);
            }

            if (type == typeof(String)) {
                return value;
            }

            if (type == typeof(int)) {
                return Int32.Parse(value);
            }

            if (type == typeof(bool)) {
                return value.ToLower() == "true";
            }

            if (type == typeof(float)) {
                return float.Parse(value.Replace(".", ","));
            }

            if (type == typeof(double)) {
                return double.Parse(value);
            }

            if (type.IsEnum) {
                return Enum.Parse(type, value);
            }

            var indexKey = new Tuple<Type, string>(type, value);
            if (_index.HasKey(indexKey)) {
                return _index.GetValue(indexKey);
            }

            if (String.IsNullOrEmpty(value)) {
                return null;
            }

            return System.Convert.ChangeType(value, type);
        }

        public dynamic Convert(dynamic values, Type type)
        {
            Type childType = type.GetElementType();
            Type dataType = values.GetType();
            int length = dataType.IsArray ? values.Length : ((IList) values).Count;
            if (type.IsArray) {
                Array result = Array.CreateInstance(childType, length);
                for (int i = 0; i < length; i++) {
                    if (values[i] is string) {
                        var indexKey = new Tuple<Type, string>(childType, values[i]);
                        result.SetValue(_index.HasKey(indexKey) ? _index.GetValue(indexKey) : values[i], i);
                    } else {
                        result.SetValue(values[i], i);
                    }
                }

                return result;
            } else {
                IList result = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(childType));
                for (int i = 0; i < length; i++) {
                    if (values[i] is string) {
                        var indexKey = new Tuple<Type, string>(childType, values[i]);
                        result.Add(_index.HasKey(indexKey) ? _index.GetValue(indexKey) : values[i]);
                    } else {
                        result.Add(values[i]);
                    }
                }

                return result;
            }
        }

        private bool isEnumerableType(Type type)
        {
            Type[] interfaces = type.GetInterfaces();
            return interfaces.Contains(typeof(IList)) || interfaces.Contains(typeof(Array));
        }
    }
}