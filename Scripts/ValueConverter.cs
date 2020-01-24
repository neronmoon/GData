using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GData {
    public class ValueConverter {
        public dynamic Convert(string value, Type type) {
            if (isEnumerableType(type) && value != null) {
                var childType = type.GetElementType();
                var values = value.Split(',').Select(x => Convert(x.Trim(), childType)).ToArray();
                return ConvertEnumerable(values, type);
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

            return System.Convert.ChangeType(value, type);
        }

        public dynamic ConvertEnumerable(dynamic values, Type type) {
            var childType = type.GetElementType();
            Type dataType = values.GetType();
            int length = dataType.IsArray ? values.Length : ((IList) values).Count;
            if (type.IsArray) {
                Array result = Array.CreateInstance(childType, length);
                for (int i = 0; i < length; i++) {
                    result.SetValue(values[i], i);
                }

                return result;
            } else {
                IList result = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(childType));
                for (int i = 0; i < length; i++) {
                    result.Add(values[i]);
                }

                return result;
            }
        }

        private bool isEnumerableType(Type type) {
            Type[] interfaces = type.GetInterfaces();
            return interfaces.Contains(typeof(IList)) || interfaces.Contains(typeof(Array));
        }
    }
}
