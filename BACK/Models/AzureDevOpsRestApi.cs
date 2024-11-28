namespace ADOWebhook.Back.Models
{
    public class AzureDevOpsRestApi
    {
        public string? PersonalAccessToken { get; set; }
        public string? Organization { get; set; }
        public string? Project { get; set; }
        public string? AzureDevOpsUrl { get; set; }
    }
}
