using System;
using System.Collections.Generic;
using System.Reflection;
using GData.Attribute;

namespace GData.Mapper
{
    public class ColumnMapper
    {
        public Dictionary<int, string> MapToColId(Type type, List<List<string>> data)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (var field in type.GetFields()) {
                GColumn columnAttr = (GColumn) field.GetCustomAttribute(typeof(GColumn));
                if (columnAttr != null) {
                    map[columnAttr.ColumnName] = field.Name;
                }
            }

            Dictionary<int, string> columnIdToFieldMap = new Dictionary<int, string>(map.Count);
            for (int colId = 0; colId < data[0].Count; colId++) {
                string columnName = data[0][colId];
                if (map.ContainsKey(columnName)) {
                    columnIdToFieldMap[colId] = map[columnName];
                }
            }

            return columnIdToFieldMap;
        }
    }
}