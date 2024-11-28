
namespace ADOWebhook.Back.Models
{
    public class WebhookPayload
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public int NotificationId { get; set; }
        public string Id { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string PublisherId { get; set; } = string.Empty;
        public Message Message { get; set; } = new Message();
        public Message DetailedMessage { get; set; } = new Message();
        public Resource Resource { get; set; } = new Resource();
        public string ResourceVersion { get; set; } = string.Empty;
        public ResourceContainers ResourceContainers { get; set; } = new ResourceContainers();
        public DateTime CreatedDate { get; set; }
    }

    public class Message
    {
        public string Text { get; set; } = string.Empty;
        public string Html { get; set; } = string.Empty;
        public string Markdown { get; set; } = string.Empty;
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
        public string Url { get; set; } = string.Empty;
        public Revision? Revision { get; set; }
    }

    public class RevisedBy
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Links? Links { get; set; }
        public string UniqueName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Descriptor { get; set; } = string.Empty;
    }

    public class FieldChange
    {
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }

    public class Links
    {
        public Link Self { get; set; } = new Link();
        public Link Parent { get; set; } = new Link();
        public Link WorkItemUpdates { get; set; } = new Link();
    }

    public class Link
    {
        public string Href { get; set; } = string.Empty;
    }

    public class Revision
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public string Url { get; set; } = string.Empty;
    }

    public class ResourceContainers
    {
        public Container Collection { get; set; } = new Container();
        public Container Account { get; set; } = new Container();
        public Container Project { get; set; } = new Container();
    }

    public class Container
    {
        public string Id { get; set; } = string.Empty;
    }

    public class CreatedBy
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Links Links { get; set; } = new Links();
    }

}