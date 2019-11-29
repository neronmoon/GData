using System;
using System.Collections.Generic;
using System.Reflection;
using GData.Attribute;

namespace GData.Mapper {
    public class ColumnMapper {
        public Dictionary<string, string> Map(Type type) {
            Dictionary<string, string> map = new Dictionary<string, string>();

            foreach (var field in type.GetFields()) {
                GColumn columnAttr = (GColumn) field.GetCustomAttribute(typeof(GColumn));
                if (columnAttr != null) {
                    map[columnAttr.ColumnName] = field.Name;
                }
            }

            return map;
        }
    }
}