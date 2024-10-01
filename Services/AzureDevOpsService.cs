using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using AzureDevOpsWebhook.Models;

namespace AzureDevOpsWebhook.Services
{
    public class AzureDevOpsService
    {
        public IConfiguration _configuration;
        private readonly string _personalAccessToken;
        private readonly string _organizationUrl;
        private readonly string _xToken;
        private readonly ILogger<AzureDevOpsService> _logger;

        public AzureDevOpsService(IConfiguration configuration, ILogger<AzureDevOpsService> logger)
        {
            _configuration = configuration;
            _personalAccessToken = _configuration["AzureDevOpsRestApi:PersonalAccessToken"];
            _organizationUrl = $"{_configuration["AzureDevOpsRestApi:AzureDevOpsApiUrl"]}{_configuration["AzureDevOpsRestApi:Organization"]}";
            _xToken = _configuration["XToken"];
            _logger = logger;
        }

        public async Task UpdateWorkItem(int workItemId, List<Dictionary<string, object>> fieldsToUpdateList)
        {
            var credentials = new VssBasicCredential(string.Empty, _personalAccessToken);
            var connection = new VssConnection(new Uri(_organizationUrl), credentials);

            var workItemClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var document = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();

            foreach (var fieldsToUpdate in fieldsToUpdateList)
            {
                foreach (var field in fieldsToUpdate)
                {
                    document.Add(
                        new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchOperation()
                        {
                            Operation = Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add,
                            Path = $"/fields/{field.Key}",
                            Value = field.Value
                        });
                }
            }
            await workItemClient.UpdateWorkItemAsync(document, workItemId);
        }

        public void SaveWorkItem(Resource workItem)
        {
            // Aqui você pode salvar o WorkItem em um banco de dados.
            _logger.LogInformation($"Salvando WorkItem com Id {workItem.WorkItemId} e título {workItem.Revision.Fields["System.Title"]}");

            if (workItem.Revision.Fields.ContainsKey("System.Description"))
            {
                var description = workItem.Revision.Fields["System.Description"].ToString();
                var textToCheck = "<div>ADOwebhook 22 </div>";

                if (!description.Contains(textToCheck))
                {
                    _ = UpdateWorkItem(workItem.WorkItemId,
                    [
                        new Dictionary<string, object> { { "WEF_0241EEE4F1594FA9A7E5DB4B4AF40B80_Kanban.Column", "Committed" } },
                        new Dictionary<string, object> { { "System.Description", description + textToCheck } }
                    ]);
                }
            }
            // Salvar na tabela do banco de dados.
        }

        public void AnalyzeWorkItem(Resource workItem)
        {
            // Chame um serviço que irá processar o WorkItem.
            _logger.LogInformation($"Analisando WorkItem {workItem.WorkItemId}");
            // Depois de analisar, pode atualizar o WorkItem via Azure DevOps API.
        }

        public bool IsValidToken(string token)
        {
            // Simulação de validação do token. Adapte conforme necessário.
            return token == _xToken;
        }
    }
}