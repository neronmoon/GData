using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GData.Attribute;

namespace GData.TypeResolving
{
    public class TypeGraphResolver
    {
        private readonly TypeResolver _typeResolver;

        public TypeGraphResolver(TypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public IEnumerable<Type> GetSortedList(Type type)
        {
            var (nodes, edges) = ResolveDependencies(type);
            return SortTypes(nodes, edges);
        }

        private Tuple<HashSet<Type>, HashSet<Tuple<Type, Type>>> ResolveDependencies(
            Type type, Tuple<HashSet<Type>, HashSet<Tuple<Type, Type>>> list = null
        )
        {
            if (list == null) {
                list = new Tuple<HashSet<Type>, HashSet<Tuple<Type, Type>>>(new HashSet<Type>(),
                    new HashSet<Tuple<Type, Type>>());
            }

            foreach (FieldInfo field in type.GetFields()) {
                Type trueType = TypeResolver.GetTrueType(field.FieldType);
                if (field.GetCustomAttribute(typeof(GTable)) != null ||
                    TypeResolver.IsTableType(trueType)) {
                    list.Item1.Add(type);
                    if (type != trueType) {
                        list.Item2.Add(new Tuple<Type, Type>(type, trueType));
                    }

                    if (!list.Item1.Contains(trueType)) {
                        list.Item1.Add(trueType);
                        list = ResolveDependencies(trueType, list);
                    }
                }
            }

            return list;
        }

        private static IEnumerable<Type> SortTypes(HashSet<Type> nodes, HashSet<Tuple<Type, Type>> edges)
        {
            var L = new List<Type>();
            var S = new HashSet<Type>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));
            while (S.Any()) {
                var n = S.First();
                S.Remove(n);
                L.Add(n);
                foreach (var e in edges.Where(e => e.Item1 == n).ToList()) {
                    var m = e.Item2;
                    edges.Remove(e);
                    if (edges.All(me => me.Item2 == m == false)) {
                        S.Add(m);
                    }
                }
            }

            if (edges.Any()) {
                throw new Exception("Graph has cycles");
            }

            L.Reverse();
            return L;
        }
    }
}