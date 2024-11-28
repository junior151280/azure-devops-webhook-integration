using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Common;
using ADOWebhook.Back.Models;
using System.Text.Json;

namespace ADOWebhook.Back.Services
{
    public class AzureDevOpsService
    {
        public IConfiguration _configuration;
        private readonly ILogger<AzureDevOpsService> _logger;
        private readonly string _personalAccessToken;
        private readonly string _organizationUrl;
        private bool _workItemUpdated = false;

        public AzureDevOpsService(IConfiguration configuration, ILogger<AzureDevOpsService> logger)
        {
            _configuration = configuration;
            _personalAccessToken = $"{_configuration["AZUREDEVOPSRESTAPI_PERSONALACCESSTOKEN"]}";
            _organizationUrl = $"{_configuration["AZUREDEVOPSRESTAPI_AZUREDEVOPSAPIURL"]}{_configuration["AZUREDEVOPSRESTAPI_ORGANIZATION"]}";
            _logger = logger;
        }
        
        public bool AnalyzeWorkItem(JsonElement workItem)
        {
            //validar se o workItemId é um número inteiro
            if (!workItem.TryGetProperty("workItemId", out JsonElement workItemIdElement) ||
                !workItemIdElement.TryGetInt32(out int workItemId))
            {
                _logger.LogError("workItemId is missing or invalid.");
                return false;
            }

            _logger.LogInformation($"Analisando WorkItem {workItemId}");

            var revion = workItem.GetProperty("revision");

            //validar se o workItem contem os campos necessários
            if (!revion.TryGetProperty("fields", out JsonElement fields) ||
                !fields.TryGetProperty("Custom.Approval", out JsonElement approvalElement))
            {
                _logger.LogError($"WorkItem {workItemId} is missing required fields.");
                return false;
            }

            //validar se o campo Custom.Approval é igual a "SendToApprove"
            var approval = approvalElement.GetString();
            if (!String.Equals(approval,"SendToApproval"))
            {
                _logger.LogInformation($"WorkItem {workItemId} não precisa ser aprovado.");
                return false;
            }

            //WorkItem precisa ser aprovado
            _logger.LogInformation($"WorkItem {workItemId} precisa ser aprovado.");
            return true;
        }


        public bool UpdateWorkItem(int workItemId, UpdateFieldRequest request)
        {
            //validar se o campo e valor não estão nulos ou vazios
            if (string.IsNullOrEmpty(request.FieldName) || request.FieldValue == null)
            {
                _logger.LogError("FieldName ou FieldValue esta nulo ou vazio.");
                throw new ArgumentException("FieldName e FieldValue não pode ser nulo ou vazio.");
            }

            _logger.LogInformation($"Atualizando o campo {request.FieldName} do WorkItem Id {workItemId} para {request.FieldValue}");

            //Atualizar o WorkItem no Azure Boards.
            _ = UpdateWorkItem(workItemId,
            [
                new Dictionary<string, object> { { request.FieldName, request.FieldValue } },
                new Dictionary<string, object> { { "Custom.statusApproval", "Approved" } }
            ]);
            return _workItemUpdated;
        }

        public async Task UpdateWorkItem(int workItemId, List<Dictionary<string, object>> fieldsToUpdateList)
        {
            _logger.LogInformation($"Atualizando o WorkItem {workItemId} no Azure DevOps {_organizationUrl}.");
            var credentials = new VssBasicCredential(string.Empty, _personalAccessToken);
            var connection = new VssConnection(new Uri(_organizationUrl), credentials);
            var workItemClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var document = new Microsoft.VisualStudio.Services.WebApi.Patch.Json.JsonPatchDocument();

            //Iterar sobre a lista de campos a serem atualizados
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
            //Atualizar o WorkItem
            await workItemClient.UpdateWorkItemAsync(document, workItemId);

            _logger.LogInformation($"WorkItem {workItemId} atualizado, aguardando validação.");
            //valida se o WorkItem foi atualizado com sucesso
            var workItem = await workItemClient.GetWorkItemAsync(workItemId);
            var workItemFields = workItem.Fields;
            foreach (var fieldsToUpdate in fieldsToUpdateList)
            {
                foreach (var field in fieldsToUpdate)
                {
                    var workItemFieldValue = workItemFields[field.Key];
                    if (!String.Equals(workItemFieldValue, field.Value))
                    {
                        _logger.LogError($"Campo {field.Key} do WorkItem {workItemId} não foi atualizado com sucesso.");
                        _workItemUpdated = false;
                        break;
                    }
                    _logger.LogInformation($"WorkItem {workItemId} atualizado com sucesso.");
                    _workItemUpdated = true;
                }
            }
        }
    }
}