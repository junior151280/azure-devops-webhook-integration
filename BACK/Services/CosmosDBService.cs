using MongoDB.Bson;
using MongoDB.Driver;

namespace ADOWebhook.Back.Services
{
    public class CosmosDBService
    {
        public IConfiguration _configuration;
        private readonly ILogger<CosmosDBService> _logger;
        private readonly IMongoCollection<BsonDocument> _collection;

        public CosmosDBService(IConfiguration configuration, ILogger<CosmosDBService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var client = new MongoClient(_configuration["COSMOSDB_CONNECTIONSTRING"]);
            var database = client.GetDatabase(_configuration["COSMOSDB_DATABASENAME"]);
            _collection = database.GetCollection<BsonDocument>(_configuration["COSMOSDB_COLLECTIONNAME"]);
        }

        public void SaveWorkItemToCosmoDB(BsonDocument payload)
        {
            // Log de informação
            _logger.LogInformation($"Salvando WorkItem com payload: {payload}");
            // Adicionar a data de salvamento ao documento
            payload.Add("SavedAt", BsonValue.Create(DateTime.UtcNow));
            // Salvar o documento na coleção
            _collection.InsertOne(payload);
        }

        public async Task<string> GetWorkItems()
        {
            // Buscar todos os documentos da coleção
            var documents = await _collection.Find(_ => true).ToListAsync();
            // Retornar os documentos em formato JSON
            return documents.ToJson();
        }

        public async Task<string> GetWorkItem(int workItemId)
        {
            // Filtrar o documento pelo workItemId
            var filter = Builders<BsonDocument>.Filter.Eq("workItemId", workItemId);
            // Buscar o documento
            var document = await _collection.Find(filter).FirstOrDefaultAsync();
            // Se o documento não for encontrado, retornar uma string vazia
            if (document == null)
            {
                _logger.LogInformation($"WorkItem com ID {workItemId} não encontrado.");
                return string.Empty;
            }
            // Retornar o documento em formato JSON
            return document.ToJson();
        }

        public async Task DeleteWorkItemFromCosmosDB(int workItemId)
        {
            // Filtrar o documento pelo workItemId
            var filter = Builders<BsonDocument>.Filter.Eq("workItemId", workItemId);
            // Deletar o documento
            await _collection.DeleteOneAsync(filter);
        }
    }
}
