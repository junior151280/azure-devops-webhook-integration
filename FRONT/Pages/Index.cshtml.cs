using ADOWebhook.Front.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Dynamic;

namespace ADOWebhook.Front.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly CosmosDbService _cosmosDbService;
        public List<Dictionary<string, object>> DocumentosJson { get; private set; } = new();
        public List<Dictionary<string, object>> Fields { get; private set; } = new();
        public IndexModel(ILogger<IndexModel> logger, CosmosDbService cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }
        public async Task OnGetAsync()
        {
            var json = await _cosmosDbService.GetDocumentsAsync();

            var workItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            if (workItems == null) return;
            foreach (var workItem in workItems)
            {
                Dictionary<string, object> keyValuePairs = [];
                var WorkItemRevision = workItem.Select(x => x).Where(x => x.Key == "revision").Select(x => x.Value).FirstOrDefault();
                var revisionFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(WorkItemRevision?.ToString() ?? string.Empty);
                if (revisionFields == null) continue;
                var fields = revisionFields.Select(x => x).Where(x => x.Key == "fields").Select(x => x.Value).ToList();
                var workItemId = revisionFields.Select(x => x).Where(x => x.Key == "id").Select(x => x.Value).FirstOrDefault();

                if (workItemId != null) keyValuePairs.Add("workItemId", workItemId);

                foreach (var field in fields)
                {
                    var fieldDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(field?.ToString() ?? string.Empty);
                    if (fieldDict != null)
                    {
                        foreach (var kvp in fieldDict)
                        {
                            keyValuePairs.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                Fields.Add(keyValuePairs);
            }
        }
    }
}
