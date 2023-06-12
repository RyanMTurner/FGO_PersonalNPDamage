using GrandOrder;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class AtlasClient : JsonClient {

    public async Task<ServantDefinition> GetServantInfo(int id) {
        string requestPath = $"https://api.atlasacademy.io/nice/JP/servant/{id}";

        var response = await GetAsync(requestPath).ConfigureAwait(false);
        var deserializedResponse = await DeserializeJsonAsync<ServantDefinition>(response).ConfigureAwait(false);

        return deserializedResponse;
    }

    public async Task<List<ServantDefinition>> GetAllServants() {
        string requestPath = $"https://api.atlasacademy.io/export/JP/nice_servant_lang_en.json";

        var response = await GetAsync(requestPath).ConfigureAwait(false);
        var deserializedResponse = await DeserializeJsonAsync<List<ServantDefinition>>(response).ConfigureAwait(false);

        return deserializedResponse;
    }

    public async Task<HTTPSniffResponse> ServantInventoryFromFile(string fileName) {
        HTTPSniffResponse retVal = null;
        using (Stream s = File.OpenRead(fileName)) {
            retVal = await DeserializeJsonAsync<HTTPSniffResponse>(s).ConfigureAwait(false);
        }

        return retVal;
    }

    public async Task<QuestDefinition> GetQuest(int id) {
        string requestPath = $"https://api.atlasacademy.io/nice/JP/quest/{id}";

        var response = await GetAsync(requestPath).ConfigureAwait(false);
        var deserializedResponse = await DeserializeJsonAsync<QuestDefinition>(response).ConfigureAwait(false);

        return deserializedResponse;
    }

}