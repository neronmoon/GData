using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GData.Attribute;
using GData.Mapper;

namespace GData {
    public class EntityLoader {
        private DataSource _source;
        private ValueConverter _converter;

        public EntityLoader(DataSource source) {
            _source = source;
            _converter = new ValueConverter();
        }

        public void SetValueConverter(ValueConverter converter) {
            _converter = converter;
        }

        public void LoadToInstance<T>(T instance) {
            Load(typeof(T), _source, instance);
        }
        
        public dynamic Load(Type type) {
            return Load(type, _source, Activator.CreateInstance(type));
        }

        public dynamic Load(Type type, DataSource source, object instance) {
            if (HasAttribute<GTable>(type)) { // if passed table class
                GTable tableAttr = (GTable) GetAttribute<GTable>(type);
                return LoadTable(type, source, tableAttr.TableName);
            }

            // if passed any other class with table fields
            foreach (FieldInfo field in type.GetFields()) {
                Type searchType = field.FieldType;
                if (searchType.GetInterfaces().Contains(typeof(IEnumerable))) {
                    searchType = field.FieldType.GetElementType();
                }

                if (searchType != null && HasAttribute<GTable>(searchType)) {
                    dynamic value = Load(searchType, source, instance);
                    field.SetValue(instance, _converter.ConvertEnumerable(value, field.FieldType));
                }
            }

            return instance;
        }

        private IList LoadTable(Type type, DataSource source, string tableName) {
            ColumnMapper mapper = new ColumnMapper();
            Dictionary<string, string> columnToFieldMap = mapper.Map(type);
            List<List<string>> data = source.GetTable(tableName);

            Dictionary<int, string> columnIdToFieldMap = new Dictionary<int, string>(columnToFieldMap.Count);
            for (int colId = 0; colId < data[0].Count; colId++) {
                string columnName = data[0][colId];
                if (columnToFieldMap.ContainsKey(columnName)) {
                    columnIdToFieldMap[colId] = columnToFieldMap[columnName];
                }
            }


            IList result = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            for (int rowId = 1; rowId < data.Count; rowId++) {
                Dictionary<string, string> fieldToValueMap = new Dictionary<string, string>(columnToFieldMap.Count);
                foreach (KeyValuePair<int, string> pair in columnIdToFieldMap) {
                    fieldToValueMap[pair.Value] = data[rowId][pair.Key];
                }

                if (fieldToValueMap.Values.All(s => s == null)) {
                    continue;
                }

                result.Add(LoadRow(type, fieldToValueMap));
            }

            return result;
        }

        private dynamic LoadRow(Type type, Dictionary<string, string> data) {
            object instance = Activator.CreateInstance(type);
            foreach (var field in type.GetFields()) {
                if (data.ContainsKey(field.Name)) {
                    field.SetValue(instance, _converter.Convert(data[field.Name], field.FieldType));
                }
            }

            return instance;
        }

        private bool HasAttribute<A>(Type type) {
            return type.GetCustomAttribute(typeof(A)) != null;
        }

        private System.Attribute GetAttribute<A>(Type type) {
            System.Attribute attribute = type.GetCustomAttribute(typeof(A));
            if (attribute != null) {
                return attribute;
            }

            throw new Exception($"No attribute {typeof(A)} on class {typeof(A)}");
        }
    }
}