namespace NetDaemon.Client.Common.HomeAssistant.Model;

public record HaasEntityId
{
    private static readonly string[] MixedDomains = ["sensor"];

    private HaasEntityId(string domain, string objectId)
    {
        Domain = domain;
        ObjectId = objectId;
    }

    public static HaasEntityId Parse(string entityId)
    {
        // This code is a duplicate of the code in NetDaemon.Extensions.MqttEntityManager.Helpers.EntityIdParser.
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException($"{nameof(entityId)} cannot be null or whitespace", nameof(entityId));

        var components = entityId.Split('.', 2);
        if (components.Length != 2 ||
            string.IsNullOrWhiteSpace(components[0]) ||
            string.IsNullOrWhiteSpace(components[1]))
            throw new ArgumentException(
                $"The {nameof(entityId)} should be of the format 'domain.identifier'. The value was {entityId}");

        return new HaasEntityId(components[0], components[1]);
    }

    public string ObjectId { get; private init; }

    public string Domain { get; private init; }

    public bool IsMixedDomain => MixedDomains.Contains(Domain);
}
