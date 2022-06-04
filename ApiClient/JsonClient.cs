using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

public abstract class JsonClient : ApiClientBase {
    protected Task<string> SerializeJson(object value) {
        if (value == null)
            return Task.FromResult<string>(null);

        return Task.Run(() => JsonConvert.SerializeObject(value));
    }

    protected async Task<T> DeserializeJsonAsync<T>(Stream data) where T : class {
        if (data == null)
            return null;

        using (data)
        using (StreamReader sr = new StreamReader(data))
        using (var reader = new JsonTextReader(sr)) {
            var serializer = new JsonSerializer();

            return await Task.Run(() => serializer.Deserialize<T>(reader));
        }
    }
}