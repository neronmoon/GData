using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GData.Attribute;
using GData.Converter;
using GData.Mapper;
using GData.TypeResolving;

namespace GData
{
    public class EntityLoader
    {
        private DataSource _source;
        private ConverterService _converterService;
        private TypeGraphResolver _graphResolver;

        Index _index = new Index();

        public EntityLoader(DataSource source)
        {
            _source = source;
            _graphResolver = new TypeGraphResolver(new TypeResolver());
            _converterService = new ConverterService(_index);
        }

        public void SetConverterService(ConverterService service)
        {
            _converterService = service;
        }

        public void LoadToInstance<T>(T instance)
        {
            Load(typeof(T), _source, instance);
        }

        public dynamic Load(Type type)
        {
            return Load(type, _source, Activator.CreateInstance(type));
        }

        private dynamic Load(Type type, DataSource source, object instance)
        {
            foreach (var t in _graphResolver.GetSortedList(type)) {
                if (TypeResolver.IsTableType(t)) {
                    LoadTable(t, source, TypeResolver.GetTableName(t));
                }
            }

            if (TypeResolver.IsTableType(type)) {
                return LoadTable(type, source, TypeResolver.GetTableName(type));
            }

            // if passed any other class with table fields
            foreach (FieldInfo field in type.GetFields()) {
                Type searchType = field.FieldType;
                if (searchType.GetInterfaces().Contains(typeof(IEnumerable))) {
                    searchType = field.FieldType.GetElementType();
                }

                if (TypeResolver.IsTableType(searchType)) {
                    dynamic value = Load(searchType, source, instance);
                    field.SetValue(instance, ConvertFieldValue(value, field));
                }
            }

            return instance;
        }

        private dynamic ConvertFieldValue(dynamic value, FieldInfo field)
        {
            Type converter = null;
            if (field.GetCustomAttribute<GConverter>() != null) {
                converter = field.GetCustomAttribute<GConverter>().ConverterType;
            }

            if (field.FieldType.GetCustomAttribute<GConverter>() != null) {
                converter = field.FieldType.GetCustomAttribute<GConverter>().ConverterType;
            }

            return _converterService.Convert(value, field.FieldType, converter);
        }

        private IList LoadTable(Type type, DataSource source, string tableName)
        {
            List<List<string>> data = source.GetTable(tableName);
            Dictionary<int, string> columnIdToFieldMap = new ColumnMapper().MapToColId(type, data);

            IList result = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            for (int rowId = 1; rowId < data.Count; rowId++) {
                Dictionary<string, string> fieldToValueMap = new Dictionary<string, string>();
                foreach (KeyValuePair<int, string> pair in columnIdToFieldMap) {
                    fieldToValueMap[pair.Value] = data[rowId][pair.Key];
                }

                if (fieldToValueMap.Values.All(s => s == null)) {
                    continue;
                }

                result.Add(BuildInstance(type, fieldToValueMap));
            }

            return result;
        }

        private dynamic BuildInstance(Type targetType, Dictionary<string, string> data)
        {
            object instance = Activator.CreateInstance(targetType);
            foreach (var field in targetType.GetFields()) {
                string fieldRawValue = data[field.Name];

                if (fieldRawValue != null) {
                    Type trueType = TypeResolver.GetTrueType(field.FieldType);
                    if (targetType == trueType) {
                        throw new Exception($"Circular dependency on field {targetType} -> {field.Name}");
                    }

                    var indexKey = new Tuple<Type, string>(trueType, fieldRawValue);
                    if (!_index.HasKey(indexKey) && TypeResolver.IsTableType(trueType)) {
                        LoadTable(field.FieldType, _source, TypeResolver.GetTableName(trueType));
                    }

                    if (_index.HasKey(indexKey)) {
                        field.SetValue(instance, _index.GetValue(indexKey));
                        continue;
                    }
                }

                if (data.ContainsKey(field.Name)) {
                    field.SetValue(instance, ConvertFieldValue(fieldRawValue, field));
                }
            }

            foreach (var field in targetType.GetFields()) {
                if (field.GetCustomAttribute(typeof(GIndex)) != null) {
                    Tuple<Type, string> indexKey = new Tuple<Type, string>(targetType, data[field.Name]);
                    if (!_index.HasKey(indexKey)) {
                        _index.Add(indexKey, instance);
                    }
                }
            }

            return instance;
        }
    }
}