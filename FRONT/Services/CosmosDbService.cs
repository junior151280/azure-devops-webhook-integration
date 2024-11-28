using System.Dynamic;

namespace ADOWebhook.Front.Services
{
    public class CosmosDbService
    {
        private readonly HttpClient _httpClient;

        public CosmosDbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDocumentsAsync()
        {
            var response = await _httpClient.GetAsync("api/CosmosDB");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public async Task<ExpandoObject?> GetDocumentAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ExpandoObject>($"api/CosmosDB/{id}");
        }

        public async Task UpdateDocumentAsync(string id, ExpandoObject document)
        {
            await _httpClient.PutAsJsonAsync($"api/CosmosDB/{id}", document);
        }

        public async Task<bool> UpdateWorkItemAsync(string workItemId, string fieldName, string fieldValue)
        {
            var payload = new { fieldName, fieldValue };
            var response = await _httpClient.PutAsJsonAsync($"/api/CosmosDB/{workItemId}", payload);
            return response.IsSuccessStatusCode;
        }

    }
}
