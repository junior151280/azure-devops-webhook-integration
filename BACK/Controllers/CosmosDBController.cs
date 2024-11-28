using Microsoft.AspNetCore.Mvc;
using ADOWebhook.Back.Services;
using ADOWebhook.Back.Models;
using Newtonsoft.Json;

namespace ADOWebhook.Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmosDBController(CosmosDBService cosmosDBService, AzureDevOpsService serviceADO) : ControllerBase
    {
        private readonly CosmosDBService _cosmosDBService = cosmosDBService;
        private readonly AzureDevOpsService _serviceADO = serviceADO;

        // GET: api/<CosmosDBController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //retorna todos os documentos do CosmosDB
            var documents = await _cosmosDBService.GetWorkItems();
            //retornar o json
            return Content(documents, "application/json");
        }

        // GET api/<CosmosDBController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            //Buscar o documento do CosmosDB pelo workItemId
            var workItem = await _cosmosDBService.GetWorkItem(id);
            //validar se retornou um documento se não retornar um erro 404
            if (workItem == null)
            {
                return NotFound();
            }
            //converter o BsonDocument para um json
            var json = JsonConvert.SerializeObject(workItem);
            //retorna o json
            return Ok(json);
        }

        // PUT api/<CosmosDBController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateFieldRequest request)
        {
            // Atualiza um documento do CosmosDB pelo id
            var workItem = await _cosmosDBService.GetWorkItem(id);
            if (workItem == null)
            {
                return NotFound();
            }

            //Atualiza o campo no Azure Boards
            if(_serviceADO.UpdateWorkItem(id, request))
            {
                //deleta o Workitem do CosmosDB
                await _cosmosDBService.DeleteWorkItemFromCosmosDB(id);
                return Ok();
            }
            else {
                return BadRequest();
            }
        }
    }
}