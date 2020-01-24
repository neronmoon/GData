using Newtonsoft.Json;
using Popcron.Sheets;

namespace GData {
    public class JsonSerializer : SheetsSerializer {
        public override T DeserializeObject<T>(string data) {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public override string SerializeObject(object data) {
            return JsonConvert.SerializeObject(data);
        }
    }
}