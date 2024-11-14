using AzureDevOpsWebhook.Models;
using AzureDevOpsWebhook.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureDevOpsWebhook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly AzureDevOpsService _service;

        public WebhookController(ILogger<WebhookController> logger, AzureDevOpsService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public IActionResult Receive([FromHeader(Name = "X-TOKEN")] string token, [FromBody] WebhookPayload payload)
        {
            // Validar o token enviado no cabeçalho
            if (!_service.IsValidToken(token))
            {
                _logger.LogWarning("Token inválido");
                return Unauthorized("Invalid token");
            }

            // Processar o payload e salvar o WorkItem
            if (payload?.Resource != null)
            {
                _service.AnalyzeWorkItem(payload.Resource); 
                _service.SaveWorkItem(payload.Resource);
                _service.UpdateWorkItem(payload.Resource);
            }
            return Ok();
        }
    }
}