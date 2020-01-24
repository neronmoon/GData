namespace GData.Attribute {
    public class GTable : System.Attribute {
        public string TableName;

        public GTable(string tableName) {
            TableName = tableName;
        }
    }
}