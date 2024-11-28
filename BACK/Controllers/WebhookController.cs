using ADOWebhook.Back.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

namespace ADOWebhook.Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController(ILogger<WebhookController> logger, IConfiguration configuration, AzureDevOpsService serviceADO, CosmosDBService cosmosDBService) : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger = logger;
        public IConfiguration _configuration = configuration;
        private readonly AzureDevOpsService _serviceADO = serviceADO;
        private readonly CosmosDBService _cosmosDBService = cosmosDBService;

        private bool IsValidToken(string token)
        {
            //Validando do token.
            var xToken = _configuration["XTOKEN"];
            return token == xToken;
        }

        [HttpPost]
        public IActionResult Receive([FromHeader(Name = "X-TOKEN")] string token, [FromBody] JsonDocument payload)
        {
            // Validar o token enviado no cabeçalho
            if (!IsValidToken(token))
            {
                _logger.LogWarning("Token inválido");
                return Unauthorized("Invalid token");
            }

            // Enviar bad request com mensagem se o payload for nulo 
            if (payload == null) { return BadRequest("Payload não pode ser Nulo."); }

            //filtrar o payload para salvar apenas nó Resource
            var resource = payload.RootElement.GetProperty("resource");

            //Valida se o payload contem um WorkItem para ser aprovado caso contrario retorna NoContent
            if (!_serviceADO.AnalyzeWorkItem(resource)) { return Ok("WorkItem inválido ou não precisa de aprovação."); }

            //Converter o Resource para BsonDocument e salvar no CosmosDB
            var bsonDocument = BsonDocument.Parse(resource.GetRawText());
            _cosmosDBService.SaveWorkItemToCosmoDB(bsonDocument);

            return Ok();
        }
    }
}