using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using GData.Attribute;

namespace GData.TypeResolving
{
    public class TypeResolver
    {
        public static bool IsTableType(Type type)
        {
            return type != null && type.GetCustomAttribute(typeof(GTable)) != null;
        }

        public static string GetTableName(Type type)
        {
            if (IsTableType(type)) {
                return type.GetCustomAttribute<GTable>().TableName;
            }

            throw new Exception($"{type} is not GTable");
        }

        public static Type GetTrueType(Type type)
        {
            Type result = type;
            Type[] interfaces = result.GetInterfaces();
            if (interfaces.Contains(typeof(IList)) || interfaces.Contains(typeof(Array))) {
                result = result.GetElementType();
            }

            return result;
        }
    }
}