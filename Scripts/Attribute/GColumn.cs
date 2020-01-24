namespace GData.Attribute {
    public class GColumn : System.Attribute {
        public string ColumnName;

        public GColumn(string columnName) {
            ColumnName = columnName;
        }
    }
}