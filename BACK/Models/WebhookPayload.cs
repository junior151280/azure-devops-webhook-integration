
namespace AzureDevOpsWebhook.Models
{
    public class WebhookPayload
    {
        public string? SubscriptionId { get; set; }
        public int NotificationId { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string PublisherId { get; set; }
        public Message Message { get; set; }
        public Message DetailedMessage { get; set; }
        public Resource Resource { get; set; }
        public string ResourceVersion { get; set; }
        public ResourceContainers ResourceContainers { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Message
    {
        public string Text { get; set; }
        public string Html { get; set; }
        public string Markdown { get; set; }
    }

    public class Resource
    {
        public int Id { get; set; }
        public int WorkItemId { get; set; }
        public int Rev { get; set; }
        public RevisedBy? RevisedBy { get; set; }
        public DateTime RevisedDate { get; set; }
        public Dictionary<string, FieldChange>? Fields { get; set; }
        public Links? Links { get; set; }
        public string? Url { get; set; }
        public Revision? Revision { get; set; }
    }

    public class RevisedBy
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string Url { get; set; }
        public Links? Links { get; set; }
        public string UniqueName { get; set; }
        public string ImageUrl { get; set; }
        public string Descriptor { get; set; }
    }

    public class FieldChange
    {
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; }
        public Link Parent { get; set; }
        public Link WorkItemUpdates { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
    }

    public class Revision
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; }
        public string Url { get; set; }
    }

    public class ResourceContainers
    {
        public Container Collection { get; set; }
        public Container Account { get; set; }
        public Container Project { get; set; }
    }

    public class Container
    {
        public string Id { get; set; }
    }

    public class CreatedBy
    {
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public Links Links { get; set; }
    }

}