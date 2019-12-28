using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Popcron.Sheets;

namespace GData {
    public class DataSource {
        private string _apiKey;
        private string _spreadsheetId;

        private Spreadsheet _cache;

        public DataSource(string apiKey, string spreadsheetId) {
            _apiKey = apiKey;
            _spreadsheetId = spreadsheetId;
        }

        public List<List<string>> GetTable(string tableName) {
            Spreadsheet spreadsheet = GetSpreadsheet();
            foreach (var sheet in spreadsheet.Sheets) {
                if (sheet.Title == tableName) {
                    return handleRows(sheet.Data);
                }
            }

            throw new Exception($"No table {tableName} found");
        }

        private List<List<string>> handleRows(Cell[,] data) {
            List<List<string>> result = new List<List<string>>(data.GetLength(1));

            for (int rowId = 0; rowId < data.GetLength(1); rowId++) {
                List<string> row = new List<string>(data.GetLength(0));
                for (int colId = 0; colId < data.GetLength(0); colId++) {
                    string cell = data[colId, rowId].Value?.Trim();
                    row.Add(!string.IsNullOrEmpty(cell) ? cell : null);
                }

                if (row.Any()) {
                    result.Add(row);
                }
            }

            return result;
        }

        private Spreadsheet GetSpreadsheet() {
            if (_cache == null) {
                JsonSerializer serializer = new JsonSerializer();

                string url = $"https://sheets.googleapis.com/v4/spreadsheets/{_spreadsheetId}?key={_apiKey}&includeGridData=true";
                
                WebClient client = new WebClient();
                string rawString = client.DownloadString(url);
                SpreadsheetRaw raw = serializer.DeserializeObject<SpreadsheetRaw>(rawString);
                
                _cache = new Spreadsheet(raw);
            }

            return _cache;
        }
    }
}